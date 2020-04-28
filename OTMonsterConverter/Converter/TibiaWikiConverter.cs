using OTMonsterConverter.MonsterTypes;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
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
            new RegexPatternKeys("name", "(?<name>[a-z ]*)", (mon, mc) => mon.FileName = mc.FindNamedGroupValue("name")),
            new RegexPatternKeys("actualname", "(?<actualname>[a-z ]*)", (mon, mc) => mon.Name = mc.FindNamedGroupValue("actualname")),
            new RegexPatternKeys("hp", @"(?<hp>\d+)", (mon, mc) => mon.Health = uint.Parse(mc.FindNamedGroupValue("hp"))),
            new RegexPatternKeys("exp", @"(?<exp>\d+)", (mon, mc) => mon.Experience = uint.Parse(mc.FindNamedGroupValue("exp"))),
            new RegexPatternKeys("armor", @"(?<armor>\d+)", (mon, mc) => mon.TotalArmor = mon.Shielding = uint.Parse(mc.FindNamedGroupValue("armor"))),
            new RegexPatternKeys("speed", @"(?<speed>\d+)", (mon, mc) => mon.Speed = uint.Parse(mc.FindNamedGroupValue("speed")) * 2),
            new RegexPatternKeys("runsat", @"(?<runsat>\d+)", (mon, mc) => mon.RunOnHealth = uint.Parse(mc.FindNamedGroupValue("runsat"))),
            new RegexPatternKeys("summon", @"(?<summon>\d+)", (mon, mc) => mon.SummonCost = uint.Parse(mc.FindNamedGroupValue("summon"))),
            new RegexPatternKeys("convince", @"(?<convince>\d+)", (mon, mc) => mon.ConvinceCost = uint.Parse(mc.FindNamedGroupValue("convince"))),
            new RegexPatternKeys("illusionable", @"(?<illusionable>yes)", (mon, mc) => mon.Illusionable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("illusionable"))),
            new RegexPatternKeys("priamrtype", @"(?<hidehealth>trap)", (mon, mc) => mon.HideHealth = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("hidehealth"))),
            new RegexPatternKeys("pushable", @"(?<pushable>yes)", (mon, mc) => mon.Pushable = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushable"))),
            // In cipbia ability to push objects means ability to push creatures too
            new RegexPatternKeys("pushobjects", @"(?<pushobjects>yes)", (mon, mc) => mon.PushItems = mon.PushCreatures = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("pushobjects"))),
            new RegexPatternKeys("senseinvis", @"(?<senseinvis>yes)", (mon, mc) => mon.IgnoreInvisible = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("senseinvis"))),
            new RegexPatternKeys("paraimmune", @"(?<paraimmune>yes)", (mon, mc) => mon.IgnoreParalyze = !string.IsNullOrWhiteSpace(mc.FindNamedGroupValue("paraimmune"))),
            new RegexPatternKeys("walksaround", @"(?<walksaround>\w+(, \w+)*)", (mon, mc) =>
            {
                string walksaround = mc.FindNamedGroupValue("walksaround");
                foreach (string field in walksaround.Split(","))
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
                return true; // to satisfy func
            }),
            new RegexPatternKeys("physicaldmgmod", @"(?<physicaldmgmod>\d+)%", (mon, mc) => mon.Physical = double.Parse(mc.FindNamedGroupValue("physicaldmgmod")) / 100.0),
            new RegexPatternKeys("earthdmgmod", @"(?<earthdmgmod>\d+)%", (mon, mc) => mon.Earth = double.Parse(mc.FindNamedGroupValue("earthdmgmod")) / 100.0),
            new RegexPatternKeys("firedmgmod", @"(?<firedmgmod>\d*)%", (mon, mc) => mon.Fire = double.Parse(mc.FindNamedGroupValue("firedmgmod")) / 100.0),
            new RegexPatternKeys("deathdmgmod", @"(?<deathdmgmod>\d*)%", (mon, mc) => mon.Death = double.Parse(mc.FindNamedGroupValue("deathdmgmod")) / 100.0),
            new RegexPatternKeys("energydmgmod", @"(?<energydmgmod>\d+)%", (mon, mc) => mon.Energy = double.Parse(mc.FindNamedGroupValue("energydmgmod")) / 100.0),
            new RegexPatternKeys("holydmgmod", @"(?<holydmgmod>\d+)%", (mon, mc) => mon.Holy = double.Parse(mc.FindNamedGroupValue("holydmgmod")) / 100.0),
            new RegexPatternKeys("icedmgmod", @"(?<icedmgmod>\d+)%", (mon, mc) => mon.Ice = double.Parse(mc.FindNamedGroupValue("icedmgmod")) / 100.0),
            new RegexPatternKeys("hpdraindmgmod", @"(?<hpdraindmgmod>\d+)%", (mon, mc) => mon.LifeDrain = double.Parse(mc.FindNamedGroupValue("hpdraindmgmod")) / 100.0),
            new RegexPatternKeys("drowndmgmod", @"(?<drowndmgmod>\d+)%", (mon, mc) => mon.Drown = double.Parse(mc.FindNamedGroupValue("drowndmgmod")) / 100.0),
            // TODO this needs to be on a none lowercase search
            new RegexPatternKeys("sounds", @"{{sound list\|(?<sounds>[a-z !?.]+(\|[a-z !?.]+)*)", (mon, mc) =>
            {
                string sounds = mc.FindNamedGroupValue("sounds");
                foreach (string sound in sounds.Split("|"))
                {
                    // TODO config default soundlevel of cipbia monsters... I thought it was yell
                    mon.Voices.Add(new Voice(){ Sound = sound, SoundLevel = SoundLevel.Say });
                }
                return true; // to satisfy func
            }),
            // blood from primarytype or creatureclasss?
            // No mon.ManaDrain in tibiawiki database
            // No healMod in OT monsters?
        };

        // Behavior maybe needs different parsing, we can try to make it smart searching for the term distance...
        // Abilities maybe needs different pasring, we should be able to get melee, summons, healing, and maybe even a few other attacks
        // Strategy likely provides no help on occasion will provide more details about beavhior and anilityies

        public bool ReadMonster(string filename, out Monster monster)
        {
            filename = "demon";
            string monsterurl = $"https://tibia.fandom.com/wiki/{filename}?action=edit";
            string looturl = $"https://tibia.fandom.com/wiki/Loot_Statistics:{filename}?action=edit";

            monster = new Monster();
            ScrapingBrowser browser = new ScrapingBrowser();

            WebPage monsterpage = browser.NavigateToPage(new Uri(monsterurl));
            var monsterElement = monsterpage.Html.CssSelect("#wpTextbox1").FirstOrDefault();
            if (monsterElement != null)
            {
                string element = monsterElement.InnerHtml;
                element = element.ToLower();

                foreach (var x in monparams)
                {
                    var matches = new Regex(x.Pattern).Matches(element);
                    try
                    {
                        x.Action.Invoke(monster, matches);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Pattern {x.Pattern} failed with \"{ex.Message}\"");
                    }
                }
            }

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

                    // Note this fails on loot lines that appear as "|Golden Legs, times:138". This lines should only appear when there is not enough loot statistics for a creature
                    var killsmatches = new Regex(@"\|kills=(?<kills>\d+)").Matches(loots);
                    double.TryParse(killsmatches.FindNamedGroupValue("kills"), out double kills);
                    var lootregex = new Regex(@"\|\s*(?<itemname>[a-z ()]*),\s*times:\s*(?<times>\d+), amount:\s*(?<amount>[0-9-]+), total:\s*(?<total>\d+)");
                    var matches = lootregex.Matches(loots);
                    foreach (Match loot in matches)
                    {
                        string item = loot.Groups["itemname"].Value;
                        double.TryParse(loot.Groups["times"].Value, out double times);
                        string amount = loot.Groups["amount"].Value;

                        double percent = times / kills;

                        double count;
                        if (!double.TryParse(amount, out count))
                        {
                            var amounts = amount.Split("-");
                            if (amounts.Length >= 2)
                            {
                                double.TryParse(amounts[1], out count);
                            }
                        }

                        monster.Items.Add(new Loot()
                        {
                            Item = item,
                            Chance = (decimal)percent,
                            Count = (decimal)count
                        });
                    }
                }
            }

            return true;
        }

        public bool WriteMonster(string directory, ref Monster monster)
        {
            throw new NotImplementedException();
        }

        public void ListMonsters()
        {
            string monsterlisturl = $"https://tibia.fandom.com/wiki/List_of_Creatures";
            IList<string> names = new List<string>();

            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage monsterspage = browser.NavigateToPage(new Uri(monsterlisturl));
            var tables = monsterspage.Html.CssSelect("table");


            //< a href = "/wiki/A_Shielded_Astral_Glyph" title = "A Shielded Astral Glyph" > A Shielded Astral Glyph </ a >
            var nameregex = new Regex("/wiki/(?<name>[a-zA-Z_]+)");
            foreach (var table in tables)
            {
                foreach (var namecell in table.SelectNodes("//td[1]"))
                {
                    if (nameregex.IsMatch(namecell.InnerHtml))
                    {
                        var namematches = nameregex.Matches(namecell.InnerHtml);
                        names.Add(namematches.FindNamedGroupValue("name"));
                    }
                }
            }
        }
    }
}
