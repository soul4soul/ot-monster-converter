using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MonsterConverterCipMon
{
    [Export(typeof(IMonsterConverter))]
    public class CipMonConverter : MonsterConverter
    {
        public override string ConverterName { get => "CipMon"; }

        public override string FileExt { get => "mon"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => false; }

        // <race id, registered name>
        private readonly IDictionary<int, string> raceIdNameMap = new Dictionary<int, string>();

        /// <summary>
        /// Not super efficient but we need a complete list of race ids ahead of time so we can map
        /// them to monster names later. Since it's extremely likely we won't encounter the race ids in the order we need
        /// them we need to get the fill list first.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public override string[] GetFilesForConversion(string directory)
        {
            string[] fileList = base.GetFilesForConversion(directory);
            foreach (string file in fileList)
            {
                string registeredName = Path.GetFileNameWithoutExtension(file);
                string fileContents = File.ReadAllText(file);

                Match m = Regex.Match(fileContents, @"RaceNumber\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (m.Success) {
                    int raceId = int.Parse(m.Value);
                    raceIdNameMap.Add(raceId, registeredName);
                }
            }

            return fileList;
        }

        // Functions
        public override ConvertResultEventArgs ReadMonster(string fileName, out Monster monster)
        {
            ConvertResultEventArgs result = new ConvertResultEventArgs(fileName);

            monster = new Monster();
            monster.FileName = monster.RegisteredName = Path.GetFileNameWithoutExtension(fileName);

            string fileContents = File.ReadAllText(fileName);

            Match m = Regex.Match(fileContents, @"RaceNumber\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RaceId = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"Name\s+= ""(.*?)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Name = m.Value; }

            m = Regex.Match(fileContents, @"/Article\s+= ""(\S+)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                {
                    monster.Description = monster.Name;
                }
                else
                {
                    monster.Description = string.Format("{0} {1}", m.Value.ToLower(), monster.Name).Trim();
                }
            }

            m = Regex.Match(fileContents, @"/Corpse\s+= ""(\d+)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Look.CorpseId = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"Outfit\s+= \((?<type>\d+), (?<head>\d+)-(?<body>\d+)-(?<legs>\d+)-(?<feet>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                monster.Look.LookType = LookType.Outfit;
                monster.Look.LookId = int.Parse(m.Groups["type"].Value);
                monster.Look.Head = int.Parse(m.Groups["head"].Value);
                monster.Look.Body = int.Parse(m.Groups["body"].Value);
                monster.Look.Legs = int.Parse(m.Groups["legs"].Value);
                monster.Look.Feet = int.Parse(m.Groups["feet"].Value);
            }

            m = Regex.Match(fileContents, @"Outfit\s+= \((?<type>\d+), (?<id>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                int type = int.Parse(m.Groups["type"].Value);
                int id = int.Parse(m.Groups["id"].Value);
                if ((type == 0) && (id == 0))
                {
                    monster.Look.LookType = LookType.Invisible;
                }
                else
                {
                    monster.Look.LookType = LookType.Item;
                    monster.Look.LookId = int.Parse(m.Groups["type"].Value);
                }
            }

            m = Regex.Match(fileContents, @"Blood\s+= (\S+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                if (string.Equals(m.Value, "blood", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.blood;
                }
                else if (string.Equals(m.Value, "slime", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.venom;
                }
                else if (string.Equals(m.Value, "fire", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.fire;
                }
                else if (string.Equals(m.Value, "bones", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.undead;
                }
            }

            m = Regex.Match(fileContents, @"Experience\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Experience = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"SummonCost\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int summonCost = 0;
            if (m.Success) { summonCost = int.Parse(m.Value); }
            if (!fileContents.Contains("NoSummon", StringComparison.OrdinalIgnoreCase)) { monster.SummonCost = summonCost; }
            if (!fileContents.Contains("NoConvince", StringComparison.OrdinalIgnoreCase)) { monster.ConvinceCost = summonCost; }

            m = Regex.Match(fileContents, @"Defend\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Shielding = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"Armor\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.TotalArmor = int.Parse(m.Value); }

            //m = Regex.Match(fileContents, @"Poison\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            m = Regex.Match(fileContents, @"LoseTarget\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RetargetChance = int.Parse(m.Value) / 100.0; }

            m = Regex.Match(fileContents, @"FleeThreshold\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RunOnHealth = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"HitPoints, (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Health = int.Parse(m.Value); }

            m = Regex.Match(fileContents, @"Strategy\s+= \((?<closest>\d+), (?<weakest>\d+), (?<strongest>\d+), (?<random>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                monster.TargetStrategy.Closest = int.Parse(m.Groups["closest"].Value);
                monster.TargetStrategy.Weakest = int.Parse(m.Groups["weakest"].Value);
                monster.TargetStrategy.Strongest = int.Parse(m.Groups["strongest"].Value);
                monster.TargetStrategy.Random = int.Parse(m.Groups["random"].Value);
            }

            monster.IsPushable = !fileContents.Contains("Unpushable", StringComparison.OrdinalIgnoreCase);
            monster.PushItems = fileContents.Contains("KickBoxes", StringComparison.OrdinalIgnoreCase);
            monster.PushCreatures = fileContents.Contains("KickCreatures", StringComparison.OrdinalIgnoreCase);
            monster.IgnoreInvisible = fileContents.Contains("SeeInvisible", StringComparison.OrdinalIgnoreCase);
            monster.IgnoreParalyze = fileContents.Contains("NoParalyze", StringComparison.OrdinalIgnoreCase);
            monster.IsIllusionable = !fileContents.Contains("NoIllusion", StringComparison.OrdinalIgnoreCase);
            monster.TargetDistance = fileContents.Contains("DistanceFighting", StringComparison.OrdinalIgnoreCase) ? 4 : 1;

            if (fileContents.Contains("NoHit", StringComparison.OrdinalIgnoreCase)) { monster.PhysicalDmgMod = 0; }
            if (fileContents.Contains("NoBurning", StringComparison.OrdinalIgnoreCase)) { monster.FireDmgMod = 0; }
            if (fileContents.Contains("NoPoison", StringComparison.OrdinalIgnoreCase)) { monster.EarthDmgMod = 0; }
            if (fileContents.Contains("NoEnergy", StringComparison.OrdinalIgnoreCase)) { monster.EnergyDmgMod = 0; }
            if (fileContents.Contains("NoLifeDrain", StringComparison.OrdinalIgnoreCase)) { monster.LifeDrainDmgMod = 0; }

            m = Regex.Match(fileContents, @"GoStrength, (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            monster.Speed = (m.Success) ? monster.Speed = int.Parse(m.Value) + 120 : 0;

            m = Regex.Match(fileContents, @"Attack\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int attack = 0;
            if (m.Success) { attack = int.Parse(m.Value); }
            monster.IsHostile = (attack != 0);

            m = Regex.Match(fileContents, @"FistFighting, (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int skill = 0;
            if (m.Success) { skill = int.Parse(m.Value); }

            if ((attack != 0) && (skill != 0))
            {
                Spell meleeAttack = new Spell()
                {
                    Name = "melee",
                    Interval = 2000,
                    Chance = 100,
                    Range = 1,
                    AttackValue = attack,
                    Skill = skill
                };
                monster.Attacks.Add(meleeAttack);
            }

            m = Regex.Match(fileContents, @"Spells.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Value, @"(\S+) \((.*?)\) -\> (\S+) \((.*?)\) \: (\d+)", RegexOptions.Singleline);
                // TODO parse abilities
            }

            m = Regex.Match(fileContents, @"Talk.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Value, @"""(.*?)""", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    SoundLevel volume = SoundLevel.Say;
                    string sound = match.Value;
                    if (sound.StartsWith("#Y"))
                    {
                        volume = SoundLevel.Yell;
                        sound = match.Value.Substring(2);
                    }
                    monster.Voices.Add(new Voice(sound, volume));
                }
            }

            m = Regex.Match(fileContents, @"Inventory.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Value, @"\((?<id>\d+), (?<count>\d+), (?<chance>\d+)\)", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    monster.Items.Add(new Loot()
                    {
                        Item = match.Groups["id"].Value,
                        Count = int.Parse(match.Groups["count"].Value),
                        Chance = decimal.Parse(match.Groups["id"].Value) / 1000
                    });
                }
            }

            return result;
        }

        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            throw new NotImplementedException();
        }
    }
}
