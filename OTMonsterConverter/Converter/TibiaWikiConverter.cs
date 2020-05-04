using OTMonsterConverter.MonsterTypes;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OTMonsterConverter.Converter
{
    public class TibiaWikiConverter : IMonsterConverter
    {
        public string FileExtRegEx => throw new NotImplementedException();

        public class RegexPatternKeys
        {
            public RegexPatternKeys(string name, string pattern, Func<Monster, MatchCollection, object> action)
            {
                Name = name;
                Pattern = @$"\|\s+{name}\s+=\s+{pattern}\s*";
                Action = action;
            }

            public string Name { get; }
            public string Pattern { get; }
            public Func<Monster, MatchCollection, object> Action { get; }
        }

        RegexPatternKeys[] monparams = new RegexPatternKeys[] {
            new RegexPatternKeys("name", "(?<name>[A-Za-z'ñ.() -]*)", (mon, mc) => mon.FileName = mc.FindNamedGroupValue("name")),
            new RegexPatternKeys("actualname", "(?<actualname>[a-z'ñ. -]*)", (mon, mc) => mon.Name = mc.FindNamedGroupValue("actualname")),
            new RegexPatternKeys("article", "(?<article>[A-Za-z ]*)", (mon, mc) =>
            {
                if (mc.Count > 0)
                {
                    mon.Description = string.Format("{0} {1}", mc.FindNamedGroupValue("article"), mon.Name).Trim();
                }
                else
                {
                    mon.Description = mon.Name;
                }
                return true;
            }),
            new RegexPatternKeys("hp", @"(?<hp>\d+)", (mon, mc) => mon.Health = uint.Parse(mc.FindNamedGroupValue("hp"))),
            new RegexPatternKeys("exp", @"(?<exp>\d+)", (mon, mc) => mon.Experience = uint.Parse(mc.FindNamedGroupValue("exp"))),
            new RegexPatternKeys("armor", @"(?<armor>\d+)", (mon, mc) => mon.TotalArmor = mon.Shielding = uint.Parse(mc.FindNamedGroupValue("armor"))),
            new RegexPatternKeys("speed", @"(?<speed>\d+)", (mon, mc) => mon.Speed = uint.Parse(mc.FindNamedGroupValue("speed")) * 2),
            new RegexPatternKeys("runsat", @"(?<runsat>\d+)", (mon, mc) => mon.RunOnHealth = uint.Parse(mc.FindNamedGroupValue("runsat"))),
            new RegexPatternKeys("summon", @"(?<summon>\d+)", (mon, mc) => mon.SummonCost = uint.Parse(mc.FindNamedGroupValue("summon"))),
            new RegexPatternKeys("convince", @"(?<convince>\d+)", (mon, mc) => mon.ConvinceCost = uint.Parse(mc.FindNamedGroupValue("convince"))),
            new RegexPatternKeys("illusionable", @"(?<illusionable>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.Illusionable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("illusionable"))),
            new RegexPatternKeys("isboss", @"(?<isboss>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.IsBoss = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("isboss"))),
            new RegexPatternKeys("priamrtype", @"(?<hidehealth>trap)", (mon, mc) => mon.HideHealth = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("hidehealth"))),
            new RegexPatternKeys("pushable", @"(?<pushable>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.Pushable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushable"))),
            // In cipbia ability to push objects means ability to push creatures too
            new RegexPatternKeys("pushobjects", @"(?<pushobjects>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.PushItems = mon.PushCreatures = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushobjects"))),
            new RegexPatternKeys("senseinvis", @"(?<senseinvis>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.IgnoreInvisible = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("senseinvis"))),
            new RegexPatternKeys("paraimmune", @"(?<paraimmune>((Y|y)(E|e)(S|s)))", (mon, mc) => mon.IgnoreParalyze = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("paraimmune"))),
            new RegexPatternKeys("walksaround", @"(?<walksaround>\w+(, \w+)*)", (mon, mc) =>
            {
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
                return true; // to satisfy func
            }),
            new RegexPatternKeys("physicalDmgMod", @"(?<physicaldmgmod>\d+)%", (mon, mc) => mon.Physical = double.Parse(mc.FindNamedGroupValue("physicaldmgmod")) / 100.0),
            new RegexPatternKeys("earthDmgMod", @"(?<earthdmgmod>\d+)%", (mon, mc) => mon.Earth = double.Parse(mc.FindNamedGroupValue("earthdmgmod")) / 100.0),
            new RegexPatternKeys("fireDmgMod", @"(?<firedmgmod>\d*)%", (mon, mc) => mon.Fire = double.Parse(mc.FindNamedGroupValue("firedmgmod")) / 100.0),
            new RegexPatternKeys("deathDmgMod", @"(?<deathdmgmod>\d*)%", (mon, mc) => mon.Death = double.Parse(mc.FindNamedGroupValue("deathdmgmod")) / 100.0),
            new RegexPatternKeys("energyDmgMod", @"(?<energydmgmod>\d+)%", (mon, mc) => mon.Energy = double.Parse(mc.FindNamedGroupValue("energydmgmod")) / 100.0),
            new RegexPatternKeys("holyDmgMod", @"(?<holydmgmod>\d+)%", (mon, mc) => mon.Holy = double.Parse(mc.FindNamedGroupValue("holydmgmod")) / 100.0),
            new RegexPatternKeys("iceDmgMod", @"(?<icedmgmod>\d+)%", (mon, mc) => mon.Ice = double.Parse(mc.FindNamedGroupValue("icedmgmod")) / 100.0),
            new RegexPatternKeys("hpdrainDmgMod", @"(?<hpdraindmgmod>\d+)%", (mon, mc) => mon.LifeDrain = double.Parse(mc.FindNamedGroupValue("hpdraindmgmod")) / 100.0),
            new RegexPatternKeys("drownDmgMod", @"(?<drowndmgmod>\d+)%", (mon, mc) => mon.Drown = double.Parse(mc.FindNamedGroupValue("drowndmgmod")) / 100.0),
            new RegexPatternKeys("sounds", @"{{Sound List\|(?<sounds>[a-zA-Z !?.']+(\|[a-zA-Z !?.']+)*)", (mon, mc) =>
            {
                string sounds = mc.FindNamedGroupValue("sounds");
                foreach (string sound in sounds.Split("|"))
                {
                    mon.Voices.Add(new Voice(){ Sound = sound, SoundLevel = SoundLevel.Say });
                }
                return true; // to satisfy func
            }),
            // TibiaWiki generally doesn't provide distance so we default to 4. In TFS monster pack 70 of 77 monsters which use distance attack stand at a range of 4.
            new RegexPatternKeys("behavior", @"(?<behavior>((D|d)(I|i)(S|s)(A|a)(N|n)(C|c)(|s)(E|e)))", (mon, mc) => mon.TargetDistance = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("behavior")) ? (uint)4 : (uint)1),
            new RegexPatternKeys("abilities", @"(?<abilities>.*)", (mon, mc) => ParseAbilities(mon, mc))
        };

        // Temp abilities for testing
        // [[melee]] (0-500), [[fire damage|great fireball]] on target (area of effect: [http://img1.wikia.nocookie.net/__cb20080107125550/tibia/en/images/c/c8/hell%27s_core1.gif]) (150-250), [[great energy beam]] ([[life drain]]: 300-480), [[energy damage|energy strike]] (only from close. (210-300), [[mana drain]] (30-120), [[self-healing]] (80-250), [[haste]], [[fire field]], [[paralysis|distance paralyze]], [[summon]]s up to 1 [[fire elemental]].

        /// <summary>
        /// Parsing abilties is implemented like this instead of a using the RegexPatternKeys so we can easily print a list of abilties which fail to be parsed
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="mc"></param>
        /// <returns></returns>
        private static bool ParseAbilities(Monster mon, MatchCollection mc)
        {
            // Abilities should be parsed for summons, melee, attacks, and defenses. Each ability is seperated by a comma
            //   We should be able to get summons (count could be tough), melee (max hit could be tough), healing, haste, and maybe more
            string abilities = mc.FindNamedGroupValue("abilities").ToLower();
            foreach (string ability in abilities.Split(","))
            {
                string cleanability = ability.Trim();
                switch (cleanability)
                {
                    case var _ when new Regex(@"\[\[melee\]\]\s*\((?<damage>[0-9-]+)\)").IsMatch(cleanability):
                        {
                            var matches = new Regex(@"\[\[melee\]\]\s*\((?<damage>[0-9-]+)\)").Matches(cleanability);
                            var spell = new Spell() { Name = "melee" };
                            if (!ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                // TODO guess defaults based on creature HP
                            }
                            spell.MinDamage = min;
                            spell.MaxDamage = max;
                            mon.Attacks.Add(spell);
                            break;
                        }

                    // Effect might need to be optional
                    case var _ when new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9-]+)\)").IsMatch(cleanability):
                        {
                            var matches = new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9-]+)\)").Matches(cleanability);
                            var spell = new Spell() { Name = "physical", Range = 7, ShootEffect = TibiaWikiToAnimation(matches.FindNamedGroupValue("effect")) };
                            if (!ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                // TODO guess defaults based on creature HP
                            }
                            spell.MinDamage = min;
                            spell.MaxDamage = max;
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[haste\]\]").IsMatch(cleanability):
                        {
                            var spell = new Spell() { Name = "speed", SpeedChange = 300, AreaEffect = Effect.MagicRed };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[strong haste\]\]").IsMatch(cleanability):
                        {
                            var spell = new Spell() { Name = "speed", SpeedChange = 450, AreaEffect = Effect.MagicRed };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[(self-? ?healing)\]\]\s*\((?<damage>[0-9-]+)\)").IsMatch(cleanability):
                        {
                            var matches = new Regex(@"\[\[(self-? ?healing)\]\]\s*\((?<damage>[0-9-]+)\)").Matches(cleanability);
                            var spell = new Spell() { Name = "healing" };
                            if (!ParseNumericRange(matches.FindNamedGroupValue("damage"), out int min, out int max))
                            {
                                // TODO guess defaults based on creature HP
                            }
                            spell.MinDamage = min;
                            spell.MaxDamage = max;
                            mon.Attacks.Add(spell);
                            break;
                        }

                    default:
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{cleanability}\"");
                        break;
                }
            }

            return true;
        }

        private static Animation? TibiaWikiToAnimation(string effect)
        {
            if ((effect == "spear") || (effect == "spears"))
            {
                return Animation.Spear;
            }
            else if ((effect == "throwing knives") || (effect == "throwing knife"))
            {
                return Animation.ThrowingKnife;
            }
            return null;
        }

        /// <summary>
        /// Converts the string representation of a number range to its interger
        ///     number equivalent. A return value indicates whether the conversion succeeded
        ///     or failed.
        /// Example numeric ranges which can be parsed are "500", "0-500", and "0-500?"
        ///     In all examples the numberic value which is parsed would be 500
        /// </summary>
        private static bool ParseNumericRange(string range, out int min, out int max)
        {
            min = 0;
            range = RemoveNonNumericChars(range);
            if (!int.TryParse(range, out max))
            {
                var ranges = range.Split("-");
                if (ranges.Length >= 2)
                {
                    int.TryParse(ranges[0], out min);
                    int.TryParse(ranges[1], out max);
                    return true;
                }
                return false;
            }
            return true;
        }

        private static string RemoveNonNumericChars(string input)
        {
            Regex rgx = new Regex("[^0-9-]");
            input = rgx.Replace(input, "");
            return input;
        }

        public bool ReadMonster(string filename, out Monster monster)
        {
            string monsterurl = $"https://tibia.fandom.com/wiki/{filename}?action=edit";
            string looturl = $"https://tibia.fandom.com/wiki/Loot_Statistics:{filename}?action=edit";

            monster = new Monster();
            ScrapingBrowser browser = new ScrapingBrowser();
            // Have to explicitly set the encoding, AutoDetectCharsetEncoding set to true doesn't do it
            browser.Encoding = Encoding.UTF8;

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

                            double count = 0;
                            if (!double.TryParse(amount, out count))
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

        public bool WriteMonster(string directory, ref Monster monster)
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
