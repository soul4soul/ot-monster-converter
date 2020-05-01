using OTMonsterConverter.MonsterTypes;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            })
            // blood from primarytype or creatureclasss?
            // No mon.ManaDrain in tibiawiki database
            // No healMod in OT monsters?
        };

        // Abilities should be parsed for summons, melee, attacks, and defenses. Each ability is seperated by a comma
        //   We should be able to get summons (count could be tough), melee (max hit could be tough), healing, haste, and maybe more
        // Behavior maybe needs different parsing, we can try to make it smart searching for the term distance...
        //   TibiaWiki has inconsistent format and usually guesses or doesn't include distance stance, default to 4?
        // Strategy likely provides no help on occasion will provide more details about Behavior and Abilities

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
            throw new NotImplementedException();
        }
    }
}
