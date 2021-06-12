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
            public RegexPatternKeys(string name, string pattern, Action<Monster, Match> action)
            {
                Name = name;
                Pattern = @$"\|\s+{name}\s+=\s+{pattern}\s*";
                Action = action;
            }

            public string Name { get; }
            public string Pattern { get; }
            public Action<Monster, Match> Action { get; }

            public override string ToString()
            {
                return $"{Name} {Pattern}";
            }
        }

        private RegexPatternKeys[] monparams = new RegexPatternKeys[] {
            new RegexPatternKeys("name", "(?<name>[A-Za-z'ñ.() -]*)", (mon, m) => mon.RegisteredName = mon.FileName = m.Groups["name"].Value),
            new RegexPatternKeys("actualname", "(?<actualname>[A-Za-z'ñ. -]*)", (mon, m) => mon.Name = m.Groups["actualname"].Value),
            new RegexPatternKeys("article", "(?<article>[A-Za-z ]*)", ParseArticle),
            new RegexPatternKeys("hp", @"(?<hp>\d+)", (mon, m) => { if (m.Groups["hp"].Success) mon.Health = uint.Parse(m.Groups["hp"].Value); }),
            new RegexPatternKeys("exp", @"(?<exp>\d+)", (mon, m) => { if (m.Groups["exp"].Success) mon.Experience = uint.Parse(m.Groups["exp"].Value); }),
            new RegexPatternKeys("armor", @"(?<armor>\d+)", (mon, m) => { if (m.Groups["armor"].Success) mon.TotalArmor = mon.Shielding = uint.Parse(m.Groups["armor"].Value); }),
            new RegexPatternKeys("speed", @"(?<speed>\d+)", (mon, m) => { if (m.Groups["speed"].Success) mon.Speed = uint.Parse(m.Groups["speed"].Value) * 2; }),
            new RegexPatternKeys("runsat", @"(?<runsat>\d+)", (mon, m) => { if (m.Groups["runsat"].Success) mon.RunOnHealth = uint.Parse(m.Groups["runsat"].Value); }),
            new RegexPatternKeys("summon", @"(?<summon>\d+)", (mon, m) => { if (m.Groups["summon"].Success) mon.SummonCost = uint.Parse(m.Groups["summon"].Value); }),
            new RegexPatternKeys("convince", @"(?<convince>\d+)", (mon, m) => { if (m.Groups["convince"].Success) mon.ConvinceCost = uint.Parse(m.Groups["convince"].Value); }),
            new RegexPatternKeys("illusionable", @"(?<illusionable>((Y|y)(E|e)(S|s)))", (mon, m) => mon.Illusionable = m.Groups["illusionable"].Success),
            new RegexPatternKeys("isboss", @"(?<isboss>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IsBoss = m.Groups["isboss"].Success),
            new RegexPatternKeys("priamrtype", @"(?<hidehealth>trap)", (mon, m) => mon.HideHealth = m.Groups["hidehealth"].Success),
            new RegexPatternKeys("pushable", @"(?<pushable>((Y|y)(E|e)(S|s)))", (mon, m) => mon.Pushable = m.Groups["pushable"].Success),
            // In cipbia ability to push objects means ability to push creatures too
            new RegexPatternKeys("pushobjects", @"(?<pushobjects>((Y|y)(E|e)(S|s)))", (mon, m) => mon.PushItems = mon.PushCreatures = m.Groups["pushobjects"].Success),
            new RegexPatternKeys("senseinvis", @"(?<senseinvis>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IgnoreInvisible = m.Groups["senseinvis"].Success),
            new RegexPatternKeys("paraimmune", @"(?<paraimmune>((Y|y)(E|e)(S|s)))", (mon, m) => mon.IgnoreParalyze = m.Groups["paraimmune"].Success),
            new RegexPatternKeys("walksaround", @"(?<walksaround>\w+(, \w+)*)", ParseWalksAround),
            new RegexPatternKeys("walksthrough", @"(?<walksthrough>\w+(, \w+)*)", ParseWalksThrough),
            new RegexPatternKeys("physicalDmgMod", @"(?<physicaldmgmod>\d+)%", (mon, m) => { if (m.Groups["physicaldmgmod"].Success) mon.Physical = double.Parse(m.Groups["physicaldmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("earthDmgMod", @"(?<earthdmgmod>\d+)%", (mon, m) => { if (m.Groups["earthdmgmod"].Success) mon.Earth = double.Parse(m.Groups["earthdmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("fireDmgMod", @"(?<firedmgmod>\d*)%", (mon, m) => { if (m.Groups["firedmgmod"].Success) mon.Fire = double.Parse(m.Groups["firedmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("deathDmgMod", @"(?<deathdmgmod>\d*)%", (mon, m) => { if (m.Groups["deathdmgmod"].Success) mon.Death = double.Parse(m.Groups["deathdmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("energyDmgMod", @"(?<energydmgmod>\d+)%", (mon, m) => { if (m.Groups["energydmgmod"].Success) mon.Energy = double.Parse(m.Groups["energydmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("holyDmgMod", @"(?<holydmgmod>\d+)%", (mon, m) => { if (m.Groups["holydmgmod"].Success) mon.Holy = double.Parse(m.Groups["holydmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("iceDmgMod", @"(?<icedmgmod>\d+)%", (mon, m) => { if (m.Groups["icedmgmod"].Success) mon.Ice = double.Parse(m.Groups["icedmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("hpdrainDmgMod", @"(?<hpdraindmgmod>\d+)%", (mon, m) => { if (m.Groups["hpdraindmgmod"].Success) mon.LifeDrain = double.Parse(m.Groups["hpdraindmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("drownDmgMod", @"(?<drowndmgmod>\d+)%", (mon, m) => { if (m.Groups["drowndmgmod"].Success) mon.Drown = double.Parse(m.Groups["drowndmgmod"].Value) / 100.0; }),
            new RegexPatternKeys("sounds", @"{{Sound List\|(?<sounds>[a-zA-Z !?.'*]+(\|[a-zA-Z !?.'*]+)*)", ParseSoundList),
            // TibiaWiki generally doesn't provide distance so we default to 4. In TFS monster pack 70 of 77 monsters which use distance attack stand at a range of 4.
            new RegexPatternKeys("behavior", @"(?<behavior>((D|d)(I|i)(S|s)(A|a)(N|n)(C|c)(|s)(E|e)))", (mon, m) => { mon.TargetDistance = m.Groups["behavior"].Success ? (uint)4 : (uint)1; }),
            new RegexPatternKeys("abilities", @"(?<abilities>.*)", ParseAbilities)
        };

        /// <summary>
        /// Boss monsters and single appear monsters don't use article
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseArticle(Monster mon, Match m)
        {
            if (m.Groups["article"].Success)
            {
                mon.Description = string.Format("{0} {1}", m.Groups["article"].Value, mon.Name).Trim();
            }
            else
            {
                mon.Description = mon.Name;
            }
        }

        private static void ParseWalksAround(Monster mon, Match m)
        {
            foreach (string field in m.Groups["walksaround"].Value.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
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

        private static void ParseWalksThrough(Monster mon, Match m)
        {
            foreach (string field in m.Groups["walksthrough"].Value.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = false;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = false;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = false;
                }
            }
        }

        private static void ParseSoundList(Monster mon, Match m)
        {
            foreach (string sound in m.Groups["sounds"].Value.Split("|"))
            {
                if (!string.IsNullOrWhiteSpace(sound))
                {
                    mon.Voices.Add(new Voice() { Sound = sound, SoundLevel = SoundLevel.Say });
                }
            }
        }

        /// <summary>
        /// Parsing abilties is implemented like this instead of a using the RegexPatternKeys so we can easily print a list of abilties which fail to be parsed
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseAbilities(Monster mon, Match m)
        {
            // Abilities should be parsed for summons, melee, attacks, and defenses. Each ability is seperated by a comma
            //   We should be able to get summons (count could be tough), melee (max hit could be tough), healing, haste, and maybe more
            string abilities = m.Groups["abilities"].Value.ToLower();
            if (string.IsNullOrWhiteSpace(abilities) || abilities.Contains("none") || abilities.Contains("unknown") || abilities == "?")
                return;

            // Splitting by comma doesn't work sometimes but with everything related to abilities for tibiawiki there is no standard
            // Better logic could be to split by comma that are outside of parentheses
            foreach (string ability in abilities.Split(","))
            {
                string cleanedAbility = ability.Trim().TrimEnd('.');
                switch (cleanedAbility)
                {
                    case var _ when new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
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
                            var match = new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Offensive, DamageElement = CombatDamage.Physical, Interval = 2000, Chance = 1, Range = 7, ShootEffect = TibiaWikiToAnimation(match.Groups["effect"].Value) };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
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
                            var match = new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                            if (ParseNumericRange(match.Groups["damage"].Value, out int min, out int max))
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

                    // Next most likely ability to parse is summons, see notes https://git.io/JGZco

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

        public override ConvertResult ReadMonster(string filename, out Monster monster)
        {
            string resultMessage = "Blood type, look type data, and abilities are not parsed.";

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
                    var match = new Regex(x.Pattern).Match(element);
                    try
                    {
                        x.Action.Invoke(monster, match);
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
                resultMessage += "Guessed creature name";
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

                    var killsmatch = new Regex(@"\|kills=(?<kills>\d+)").Match(loots);
                    double.TryParse(killsmatch.Groups["kills"].Value, out double kills);
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

            return new ConvertResult(filename, ConvertError.Warning, resultMessage);
        }

        public override ConvertResult WriteMonster(string directory, ref Monster monster)
        {
            string[] lines =
            {
                "{{Infobox Creature|List={{{1|}}}|GetValue={{{GetValue|}}}",
                $"| name           = {monster.RegisteredName}",
                $"| hp             = {monster.Health}",
                $"| actualname     = {monster.Name}",
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
                $"| physicalDmgMod = {monster.Physical * 100:0}%",
                $"| earthDmgMod    = {monster.Earth * 100:0}%",
                $"| fireDmgMod     = {monster.Fire * 100:0}%",
                $"| deathDmgMod    = {monster.Death * 100:0}%",
                $"| energyDmgMod   = {monster.Energy * 100:0}%",
                $"| holyDmgMod     = {monster.Holy * 100:0}%",
                $"| iceDmgMod      = {monster.Ice * 100:0}%",
                $"| hpDrainDmgMod  = {monster.LifeDrain * 100:0}%",
                $"| drownDmgMod    = {monster.Drown * 100:0}%",
                $"| sounds         = {GenericToTibiaWikiVoice(ref monster)}",
                $"| runsat         = {monster.RunOnHealth}",
                $"| speed          = {monster.Speed}"
            };
            string fileName = Path.Combine(directory, monster.FileName);
            File.WriteAllLines(fileName, lines);

            return new ConvertResult(fileName, ConvertError.Warning, "Loot information is not written.");
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
