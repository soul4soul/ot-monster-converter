using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace MonsterConverterTibiaWiki
{
    [Export(typeof(IMonsterConverter))]
    public class TibiaWikiConverter : MonsterConverter
    {
        public override string ConverterName { get => "TibiaWiki"; }

        public override FileSource FileSource { get => FileSource.Web; }

        public override string FileExt { get => "html"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => true; }

        private class RegexPatternKeys
        {
            public RegexPatternKeys(string name, string pattern, Action<Monster, MatchCollection> action)
            {
                Name = name;
                Pattern = @$"\|\s+{name}\s+=\s+{pattern}\s*";
                Action = action;
            }

            public string Name { get; }
            public string Pattern { get; }
            public Action<Monster, MatchCollection> Action { get; }

            public override string ToString()
            {
                return $"{Name} {Pattern}";
            }
        }

        private RegexPatternKeys[] monparams = new RegexPatternKeys[] {
            new RegexPatternKeys("name", "(?<name>[A-Za-z'ñ.() -]*)", (mon, mc) => mon.FileName = mc.FindNamedGroupValue("name")),
            new RegexPatternKeys("actualname", "(?<actualname>[A-Za-z'ñ. -]*)", (mon, mc) => mon.Name = mc.FindNamedGroupValue("actualname")),
            new RegexPatternKeys("article", "(?<article>[A-Za-z ]*)", ParseArticle),
            new RegexPatternKeys("hp", @"(?<hp>\d+)", (mon, mc) => { if (mc.Count > 0) mon.Health = uint.Parse(mc.FindNamedGroupValue("hp")); }),
            new RegexPatternKeys("exp", @"(?<exp>\d+)", (mon, mc) => { if (mc.Count > 0) mon.Experience = uint.Parse(mc.FindNamedGroupValue("exp")); }),
            new RegexPatternKeys("armor", @"(?<armor>\d+)", (mon, mc) => { if (mc.Count > 0) mon.TotalArmor = mon.Shielding = uint.Parse(mc.FindNamedGroupValue("armor")); }),
            new RegexPatternKeys("speed", @"(?<speed>\d+)", (mon, mc) => { if (mc.Count > 0) mon.Speed = uint.Parse(mc.FindNamedGroupValue("speed")) * 2; }),
            new RegexPatternKeys("runsat", @"(?<runsat>\d+)", (mon, mc) => { if (mc.Count > 0) mon.RunOnHealth = uint.Parse(mc.FindNamedGroupValue("runsat")); }),
            new RegexPatternKeys("summon", @"(?<summon>\d+)", (mon, mc) => { if (mc.Count > 0) mon.SummonCost = uint.Parse(mc.FindNamedGroupValue("summon")); }),
            new RegexPatternKeys("convince", @"(?<convince>\d+)", (mon, mc) => { if (mc.Count > 0) mon.ConvinceCost = uint.Parse(mc.FindNamedGroupValue("convince")); }),
            new RegexPatternKeys("illusionable", @"(?<illusionable>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.Illusionable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("illusionable")); }),
            new RegexPatternKeys("isboss", @"(?<isboss>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.IsBoss = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("isboss")); }),
            new RegexPatternKeys("priamrtype", @"(?<hidehealth>trap)", (mon, mc) => { if (mc.Count > 0) mon.HideHealth = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("hidehealth")); }),
            new RegexPatternKeys("pushable", @"(?<pushable>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.Pushable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushable")); }),
            // In cipbia ability to push objects means ability to push creatures too
            new RegexPatternKeys("pushobjects", @"(?<pushobjects>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.PushItems = mon.PushCreatures = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushobjects")); }),
            new RegexPatternKeys("senseinvis", @"(?<senseinvis>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.IgnoreInvisible = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("senseinvis")); }),
            new RegexPatternKeys("paraimmune", @"(?<paraimmune>((Y|y)(E|e)(S|s)))", (mon, mc) => { if (mc.Count > 0) mon.IgnoreParalyze = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("paraimmune")); }),
            new RegexPatternKeys("walksaround", @"(?<walksaround>\w+(, \w+)*)", ParseWalksAround),
            new RegexPatternKeys("physicalDmgMod", @"(?<physicaldmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Physical = double.Parse(mc.FindNamedGroupValue("physicaldmgmod")) / 100.0; }),
            new RegexPatternKeys("earthDmgMod", @"(?<earthdmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Earth = double.Parse(mc.FindNamedGroupValue("earthdmgmod")) / 100.0; }),
            new RegexPatternKeys("fireDmgMod", @"(?<firedmgmod>\d*)%", (mon, mc) => { if (mc.Count > 0) mon.Fire = double.Parse(mc.FindNamedGroupValue("firedmgmod")) / 100.0; }),
            new RegexPatternKeys("deathDmgMod", @"(?<deathdmgmod>\d*)%", (mon, mc) => { if (mc.Count > 0) mon.Death = double.Parse(mc.FindNamedGroupValue("deathdmgmod")) / 100.0; }),
            new RegexPatternKeys("energyDmgMod", @"(?<energydmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Energy = double.Parse(mc.FindNamedGroupValue("energydmgmod")) / 100.0; }),
            new RegexPatternKeys("holyDmgMod", @"(?<holydmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Holy = double.Parse(mc.FindNamedGroupValue("holydmgmod")) / 100.0; }),
            new RegexPatternKeys("iceDmgMod", @"(?<icedmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Ice = double.Parse(mc.FindNamedGroupValue("icedmgmod")) / 100.0; }),
            new RegexPatternKeys("hpdrainDmgMod", @"(?<hpdraindmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.LifeDrain = double.Parse(mc.FindNamedGroupValue("hpdraindmgmod")) / 100.0; }),
            new RegexPatternKeys("drownDmgMod", @"(?<drowndmgmod>\d+)%", (mon, mc) => { if (mc.Count > 0) mon.Drown = double.Parse(mc.FindNamedGroupValue("drowndmgmod")) / 100.0; }),
            new RegexPatternKeys("sounds", @"{{Sound List\|(?<sounds>[a-zA-Z !?.']+(\|[a-zA-Z !?.']+)*)", ParseSoundList),
            // TibiaWiki generally doesn't provide distance so we default to 4. In TFS monster pack 70 of 77 monsters which use distance attack stand at a range of 4.
            new RegexPatternKeys("behavior", @"(?<behavior>((D|d)(I|i)(S|s)(A|a)(N|n)(C|c)(|s)(E|e)))", (mon, mc) => { if (mc.Count > 0) mon.TargetDistance = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("behavior")) ? (uint)4 : (uint)1; }),
            new RegexPatternKeys("abilities", @"(?<abilities>.*)", ParseAbilities)
        };

        /// <summary>
        /// Boss monsters and single appear monsters don't use article
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="mc"></param>
        private static void ParseArticle(Monster mon, MatchCollection mc)
        {
            if (mc.Count > 0)
            {
                mon.Description = string.Format("{0} {1}", mc.FindNamedGroupValue("article"), mon.Name).Trim();
            }
            else
            {
                mon.Description = mon.Name;
            }
        }

        private static void ParseWalksAround(Monster mon, MatchCollection mc)
        {
            if (mc.Count == 0)
                return;

            string walksaround = mc.FindNamedGroupValue("walksaround");

            mon.AvoidFire = false;
            mon.AvoidEnergy = false;
            mon.AvoidPoison = false;
            foreach (string field in walksaround.Split(","))
            {
                string fieldtrim = field.Trim().ToLower();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = true;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = true;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = true;
                }
            }
        }

        private static void ParseSoundList(Monster mon, MatchCollection mc)
        {
            if (mc.Count == 0)
                return;

            string sounds = mc.FindNamedGroupValue("sounds");
            foreach (string sound in sounds.Split("|"))
            {
                mon.Voices.Add(new Voice() { Sound = sound, SoundLevel = SoundLevel.Say });
            }
        }

        /// <summary>
        /// Parsing abilties is implemented like this instead of a using the RegexPatternKeys so we can easily print a list of abilties which fail to be parsed
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="mc"></param>
        private static void ParseAbilities(Monster mon, MatchCollection mc)
        {
            if (mc.Count == 0)
                return;

            // Abilities should be parsed for summons, melee, attacks, and defenses. Each ability is seperated by a comma
            //   We should be able to get summons (count could be tough), melee (max hit could be tough), healing, haste, and maybe more
            string abilities = mc.FindNamedGroupValue("abilities").ToLower();
            if (abilities.Contains("none") || abilities == "?")
                return;

            // Splitting by , doesn't work sometimes but with everything related to abilities for tibiawiki there is no standard
            // Better logic could be to split by , that are outside of parentheses
            foreach (string ability in abilities.Split(","))
            {
                string cleanedAbility = ability.Trim().TrimEnd('.');
                switch (cleanedAbility)
                {
                    case var _ when new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var matches = new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").Matches(cleanedAbility);
                            var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                            if (ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    // Effect might need to be optional
                    case var _ when new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").IsMatch(cleanedAbility):
                        {
                            var matches = new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").Matches(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Offensive, DamageElement = CombatDamage.Physical, Interval = 2000, Chance = 1, Range = 7, ShootEffect = TibiaWikiToAnimation(matches.FindNamedGroupValue("effect")) };
                            if (ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 300, MaxSpeedChange = 300, AreaEffect = Effect.MagicRed, Duration = 7000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[strong haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 450, MaxSpeedChange = 450, AreaEffect = Effect.MagicRed, Duration = 4000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var matches = new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").Matches(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                            if (ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                spell.MinDamage = min;
                                spell.MaxDamage = max;
                            }
                            else
                            {
                                // Guess defaults based on creature HP
                                spell.MinDamage = (int?)(mon.Health * 0.1);
                                spell.MaxDamage = (int?)(mon.Health * 0.25);
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    // Next most likely condition to parse is summons, see notes https://git.io/JGZco

                    default:
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{cleanedAbility}\"");
                        break;
                }
            }
        }

        private static Animation TibiaWikiToAnimation(string effect)
        {
            if ((effect == "spear") || (effect == "spears"))
            {
                return Animation.Spear;
            }
            else if ((effect == "throwing knives") || (effect == "throwing knife"))
            {
                return Animation.ThrowingKnife;
            }
            else if ((effect == "bolt") || (effect == "bolts"))
            {
                return Animation.Bolt;
            }
            else if ((effect == "arrow") || (effect == "arrows"))
            {
                return Animation.Arrow;
            }
            else if (effect.Contains("boulder"))
            {
                return Animation.LargeRock;
            }
            else if (effect.Contains("stone"))
            {
                return Animation.SmallStone;
            }
            else
            {
                return Animation.None;
            }
        }

        /// <summary>
        /// Converts a string representing a numeric range to two intergers
        /// Example numeric ranges which can be parsed are "500", "0-500", and "0-500?"
        /// </summary>
        /// <param name="range">String to parse</param>
        /// <param name="min">lower bound value in range, defaults to 0</param>
        /// <param name="max">high bonund value in the range, will be set when the range only has a single number</param>
        /// <returns>returns false when no numeric values can be parsed</returns>
        private static bool ParseNumericRange(string range, out int min, out int max)
        {
            Regex rgx = new Regex(@"(?<first>\d+)(([ -]?)(?<second>\d+))?");
            var match = rgx.Match(range);

            min = 0;
            if (int.TryParse(match.Groups["second"].Value, out max))
            {
                int.TryParse(match.Groups["first"].Value, out min);
                return true;
            }
            else if (int.TryParse(match.Groups["first"].Value, out max))
            {
                return true;
            }
            return false;
        }

        public override string[] GetFilesForConversion(string directory)
        {
            // directory parameter is ignored for this format...
            string monsterlisturl = $"https://tibia.fandom.com/wiki/List_of_Creatures_(Ordered)";
            IList<string> names = new List<string>();

            ScrapingBrowser browser = new ScrapingBrowser();
            browser.Encoding = Encoding.UTF8;
            WebPage monstersPage = browser.NavigateToPage(new Uri(monsterlisturl));
            var monsterTable = monstersPage.Html.CssSelect(".mw-parser-output").FirstOrDefault();

            // Links are HTML encoded
            // %27 is HTML encode for ' character
            // %27%C3% is HTML encode for ñ character
            var namematches = new Regex("/wiki/(?<name>[[a-zA-Z.()_%27%C3%B1-]+)").Matches(monsterTable.InnerHtml);
            foreach (Match match in namematches)
            {
                names.Add(match.Groups["name"].Value.Replace("%27", "'").Replace("%C3%B1", "ñ"));
            }

            return names.ToArray();
        }

        public override bool ReadMonster(string filename, out Monster monster)
        {
            string monsterurl = $"https://tibia.fandom.com/wiki/{filename}?action=edit";
            string looturl = $"https://tibia.fandom.com/wiki/Loot_Statistics:{filename}?action=edit";

            monster = new Monster() { Name = "" };
            // Have to explicitly set the encoding, AutoDetectCharsetEncoding set to true doesn't do it
            ScrapingBrowser browser = new ScrapingBrowser() { Encoding = Encoding.UTF8 };

            WebPage monsterpage = browser.NavigateToPage(new Uri(monsterurl));
            var monsterElement = monsterpage.Html.CssSelect("#wpTextbox1").FirstOrDefault();
            if (monsterElement != null)
            {
                string element = monsterElement.InnerHtml;
                foreach (var x in monparams)
                {
                    var matches = new Regex(x.Pattern).Matches(element);
                    try
                    {
                        x.Action.Invoke(monster, matches);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"{filename} Pattern \"{x.Pattern}\" failed with \"{ex.Message}\"");
                    }
                }
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (string.IsNullOrWhiteSpace(monster.Name) && !string.IsNullOrWhiteSpace(monster.FileName))
            {
                // Better then nothing guess
                monster.Name = monster.FileName;
            }
            monster.Name = textInfo.ToTitleCase(monster.Name);

            WebPage lootpage = browser.NavigateToPage(new Uri(looturl));
            var statsElement = lootpage.Html.CssSelect("#wpTextbox1").FirstOrDefault();
            if (statsElement != null)
            {
                string elements = statsElement.InnerHtml.ToLower();
                var lootsectionsRegEx = new Regex("{{loot2(?<loots>.*)}}", RegexOptions.Singleline);
                if (lootsectionsRegEx.IsMatch(elements))
                {
                    var lootsection = lootsectionsRegEx.Match(elements);
                    string loots = lootsection.Captures[0].Value;

                    var killsmatches = new Regex(@"\|kills=(?<kills>\d+)").Matches(loots);
                    double.TryParse(killsmatches.FindNamedGroupValue("kills"), out double kills);
                    // sometimes TibiaWiki doesn't show the amount field
                    var lootregex = new Regex(@"\|\s*(?<itemname>[a-z'.() ]*),\s*times:\s*(?<times>\d+)(, amount:\s*(?<amount>[0-9-]+))?");
                    var matches = lootregex.Matches(loots);
                    foreach (Match loot in matches)
                    {
                        string item = loot.Groups["itemname"].Value;
                        double.TryParse(loot.Groups["times"].Value, out double times);
                        string amount = loot.Groups["amount"].Value;

                        if (item != "empty")
                        {
                            double percent = times / kills;

                            if (!double.TryParse(amount, out double count))
                            {
                                var amounts = amount.Split("-");
                                if (amounts.Length >= 2)
                                {
                                    double.TryParse(amounts[1], out count);
                                }
                            }
                            count = (count > 0) ? count : 1;

                            monster.Items.Add(new Loot()
                            {
                                Item = item,
                                Chance = (decimal)percent,
                                Count = (decimal)count
                            });
                        }
                    }
                }
            }

            return true;
        }

        public override bool WriteMonster(string directory, ref Monster monster)
        {
            string[] lines =
            {
                "{{Infobox Creature|List={{{1|}}}|GetValue={{{GetValue|}}}",
                $"| name           = {monster.FileName}",
                $"| hp             = {monster.Health}",
                $"| exp            = {monster.Experience}",
                $"| armor          = {monster.TotalArmor}",
                string.Format("| summon         = {0}", monster.SummonCost > 0 ? monster.SummonCost.ToString() : "--"),
                string.Format("| convince         = {0}", monster.ConvinceCost > 0 ? monster.ConvinceCost.ToString() : "--"),
                string.Format("| convince         = {0}", monster.Illusionable ? "yes" : "no"),
                string.Format("| isboss         = {0}", monster.IsBoss ? "yes" : "no"),
                string.Format("| pushable         = {0}", monster.Pushable ? "yes" : "no"),
                string.Format("| pushobjects         = {0}", monster.PushItems ? "yes" : "no"),
                $"| walksaround    = {GenericToTibiaWikiWalkAround(ref monster)}",
                $"| walksthrough   = {GenericToTibiaWikiWalkThrough(ref monster)}",
                string.Format("| paraimmune         = {0}", monster.IgnoreParalyze ? "yes" : "no"),
                string.Format("| senseinvis         = {0}", monster.IgnoreInvisible ? "yes" : "no"),
                $"| physicalDmgMod = {monster.Physical * 100}%",
                $"| earthDmgMod    = {monster.Earth * 100}%",
                $"| fireDmgMod     = {monster.Fire * 100}%",
                $"| deathDmgMod    = {monster.Death * 100}%",
                $"| energyDmgMod   = {monster.Energy * 100}%",
                $"| holyDmgMod     = {monster.Holy * 100}%",
                $"| iceDmgMod      = {monster.Ice * 100}%",
                $"| hpDrainDmgMod  = {monster.LifeDrain * 100}%",
                $"| drownDmgMod    = {monster.Drown * 100}%",
                $"| sounds         = {GenericToTibiaWikiVoice(ref monster)}",
                $"| runsat         = {monster.RunOnHealth}",
                $"| speed          = {monster.Speed}"
            };
            string fileName = Path.Combine(directory, monster.FileName);
            File.WriteAllLines(fileName, lines);

            return true;
        }

        private string GenericToTibiaWikiWalkAround(ref Monster monster)
        {
            string walks = "";
            if (monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private string GenericToTibiaWikiWalkThrough(ref Monster monster)
        {
            string walks = "";
            if (!monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (!monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (!monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private string GenericToTibiaWikiVoice(ref Monster monster)
        {
            string voice = "";
            foreach (var v in monster.Voices)
            {
                if (string.IsNullOrWhiteSpace(voice))
                {
                    voice = v.Sound;
                }
                else
                {
                    voice = $"{voice}|{v.Sound}";
                }
            }
            return $"{{{{Sound List|{voice}}}}}";
        }
    }
}
