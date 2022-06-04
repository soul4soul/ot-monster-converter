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
        // <race id, registered name>
        private readonly IDictionary<int, string> raceIdNameMap = new Dictionary<int, string>();

        private readonly IDictionary<ConditionType, int> conditionDefaultTick = new Dictionary<ConditionType, int>
        {
            {ConditionType.Bleeding,    4000},
            {ConditionType.Energy,      10000},
            {ConditionType.Fire,        9000},
            {ConditionType.Poison,      4000},
            {ConditionType.Drown,       5000},
            {ConditionType.Freezing,    8000},
            {ConditionType.Dazzled,     10000},
            {ConditionType.Cursed,      4000},
        };

        [Flags]
        private enum DamageKinds
        {
            Physical = 1,
            Poison = 2,
            Fire = 4,
            Energy = 8,
            PoisonCondition = 32,
            FireCondition = 64,
            EnergyCondition = 128,
            LifeDrain = 256,
            ManaDrain = 512
        }

        /// <summary>
        /// Monster speed in OT server if offset from cipbia by 120
        /// </summary>
        private readonly int SPEED_OFFSET = 120;

        /// <summary>
        /// Interval at which abilities will happen
        /// </summary>
        private readonly int INTERVAL = 2000;

        public override string ConverterName { get => "Cip Mon"; }

        public override string FileExt { get => "mon"; }

        public override ItemIdType ItemIdType { get => ItemIdType.Client; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => false; }

        private Missile animationFromString(string input)
        {
            int animationValue = int.Parse(input);
            return (Missile)animationValue;
        }

        private Effect effectFromString(string input)
        {
            int value = int.Parse(input);
            return (Effect)value;
        }

        private DamageKinds damageKindsFromString(string input)
        {
            int value = int.Parse(input);
            return (DamageKinds)value;
        }

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
                    int raceId = int.Parse(m.Groups[1].Value);
                    if (raceIdNameMap.ContainsKey(raceId))
                    {
                        raceIdNameMap[raceId] = registeredName;
                    }
                    else
                    {
                        raceIdNameMap.Add(raceId, registeredName);
                    }
                }
            }

            return fileList;
        }

        // Functions
        public override ConvertResultEventArgs ReadMonster(string fileName, out Monster monster)
        {
            ConvertResultEventArgs result = new ConvertResultEventArgs(fileName);
            //ConvertResultEventArgs result = new ConvertResultEventArgs(fileName, ConvertError.Warning, "item ids are in client format");

            monster = new Monster();
            monster.FileName = monster.RegisteredName = Path.GetFileNameWithoutExtension(fileName);

            string fileContents = File.ReadAllText(fileName);

            Match m = Regex.Match(fileContents, @"RaceNumber\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RaceId = int.Parse(m.Groups[1].Value); }

            m = Regex.Match(fileContents, @"Name\s+= ""(.*?)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Name = m.Groups[1].Value; }

            m = Regex.Match(fileContents, @"Article\s+= ""(\S*)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                if (string.IsNullOrWhiteSpace(m.Groups[1].Value))
                {
                    monster.Description = monster.Name;
                }
                else
                {
                    monster.Description = string.Format("{0} {1}", m.Groups[1].Value.ToLower(), monster.Name).Trim();
                }
            }

            m = Regex.Match(fileContents, @"Corpse\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Look.CorpseId = ushort.Parse(m.Groups[1].Value); }

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
                    monster.Look.LookId = id;
                }
            }

            m = Regex.Match(fileContents, @"Blood\s+= (\S+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                if (string.Equals(m.Groups[1].Value, "blood", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.blood;
                }
                else if (string.Equals(m.Groups[1].Value, "slime", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.venom;
                }
                else if (string.Equals(m.Groups[1].Value, "fire", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.fire;
                }
                else if (string.Equals(m.Groups[1].Value, "bones", StringComparison.OrdinalIgnoreCase))
                {
                    monster.Race = Blood.undead;
                }
            }

            m = Regex.Match(fileContents, @"Experience\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Experience = int.Parse(m.Groups[1].Value); }

            m = Regex.Match(fileContents, @"SummonCost\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int summonCost = 0;
            if (m.Success) { summonCost = int.Parse(m.Groups[1].Value); }
            if (!fileContents.Contains("NoSummon", StringComparison.OrdinalIgnoreCase)) { monster.SummonCost = summonCost; }
            if (!fileContents.Contains("NoConvince", StringComparison.OrdinalIgnoreCase)) { monster.ConvinceCost = summonCost; }

            m = Regex.Match(fileContents, @"Defend\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Shielding = int.Parse(m.Groups[1].Value); }

            m = Regex.Match(fileContents, @"Armor\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.TotalArmor = int.Parse(m.Groups[1].Value); }

            int meleePoison = 0;
            m = Regex.Match(fileContents, @"Poison\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { meleePoison = int.Parse(m.Groups[1].Value); }

            m = Regex.Match(fileContents, @"LoseTarget\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RetargetChance = int.Parse(m.Groups[1].Value) / 100.0; }

            m = Regex.Match(fileContents, @"FleeThreshold\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.RunOnHealth = int.Parse(m.Groups[1].Value); }

            m = Regex.Match(fileContents, @"HitPoints, (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success) { monster.Health = int.Parse(m.Groups[1].Value); }

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
            monster.Speed = (m.Success) ? monster.Speed = int.Parse(m.Groups[1].Value) + SPEED_OFFSET : 0;

            m = Regex.Match(fileContents, @"Attack\s+= (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int attack = 0;
            if (m.Success) { attack = int.Parse(m.Groups[1].Value); }
            monster.IsHostile = (attack != 0);

            m = Regex.Match(fileContents, @"FistFighting, (\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int skill = 0;
            if (m.Success) { skill = int.Parse(m.Groups[1].Value); }

            if ((attack != 0) && (skill != 0))
            {
                Spell meleeAttack = new Spell()
                {
                    Name = "melee",
                    Interval = INTERVAL,
                    Chance = 1,
                    Range = 1,
                    AttackValue = attack,
                    Skill = skill
                };
                if (meleePoison > 0)
                {
                    meleeAttack.Condition = ConditionType.Poison;
                    meleeAttack.StartDamage = meleePoison;
                    meleeAttack.Tick = 4000; // Default cipbia poison tick
                }
                monster.Attacks.Add(meleeAttack);
            }

            m = Regex.Match(fileContents, @"Spells.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Groups[1].Value, @"(\S+) \((.*?)\) -\> (\S+) \((.*?)\) \: (\d+)", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    double chance = 1 / double.Parse(match.Groups[5].Value);
                    string castType = match.Groups[1].Value.ToLower();
                    string[] castTypeParams = match.Groups[2].Value.Split(',');
                    string action = match.Groups[3].Value.ToLower();
                    string[] actionParams = match.Groups[4].Value.Split(',');

                    Spell spell = new Spell() {
                        Interval = INTERVAL,
                        Chance = chance
                    };

                    if (castType == "actor")
                    {
                        spell.AreaEffect = effectFromString(castTypeParams[0]);
                    }
                    else if (castType == "victim")
                    {
                        spell.Range = int.Parse(castTypeParams[0]);
                        spell.ShootEffect = animationFromString(castTypeParams[1]);
                        spell.AreaEffect = effectFromString(castTypeParams[2]);
                        spell.OnTarget = true;
                    }
                    else if (castType == "origin")
                    {
                        spell.Radius = int.Parse(castTypeParams[0]) + 1;
                        spell.AreaEffect = effectFromString(castTypeParams[1]);
                        spell.OnTarget = false;
                    }
                    else if (castType == "destination")
                    {
                        spell.Range = int.Parse(castTypeParams[0]);
                        spell.ShootEffect = animationFromString(castTypeParams[1]);
                        spell.Radius = int.Parse(castTypeParams[2]) + 1;
                        spell.AreaEffect = effectFromString(castTypeParams[3]);
                        spell.OnTarget = true;
                    }
                    else if (castType == "angle")
                    {
                        spell.Length = int.Parse(castTypeParams[1]);
                        spell.Spread = int.Parse(castTypeParams[0]) / 10;
                        spell.AreaEffect = effectFromString(castTypeParams[2]);
                    }
                    // else There are no other castTypes in the 7.7 monster pack

                    if (action == "healing")
                    {
                        spell.Name = "combat";
                        spell.Description = "self-healing";
                        spell.SpellCategory = SpellCategory.Defensive;
                        spell.DamageElement = CombatDamage.Healing;

                        int baseVal = int.Parse(actionParams[0]);
                        int variation = int.Parse(actionParams[1]);
                        spell.MinDamage = baseVal - variation;
                        spell.MaxDamage = baseVal + variation;
                    }
                    else if (action == "speed")
                    {
                        spell.Name = "speed";
                        int baseVal = int.Parse(actionParams[0]);
                        int variation = int.Parse(actionParams[1]);
                        spell.Duration = int.Parse(actionParams[2]) * 1000;
                        if (baseVal < 0)
                        {
                            spell.MinSpeedChange = baseVal + variation - SPEED_OFFSET;
                            spell.MaxSpeedChange = baseVal - variation - SPEED_OFFSET;
                            spell.SpellCategory = SpellCategory.Offensive;
                            spell.Description = "paralyze";
                        }
                        else
                        {
                            spell.MinSpeedChange = baseVal - variation + SPEED_OFFSET;
                            spell.MaxSpeedChange = baseVal + variation + SPEED_OFFSET;
                            spell.SpellCategory = SpellCategory.Defensive;
                            spell.Description = "haste";
                        }
                    }
                    else if (action == "damage")
                    {
                        spell.Name = "combat";
                        spell.SpellCategory = SpellCategory.Offensive;

                        int baseVal = int.Parse(actionParams[1]);
                        int variation = int.Parse(actionParams[2]);
                        spell.MinDamage = - baseVal + variation;
                        spell.MaxDamage = - baseVal - variation;

                        DamageKinds damageKinds = damageKindsFromString(actionParams[0]);
                        if (damageKinds == DamageKinds.FireCondition)
                        {
                            spell.Name = "condition";
                            spell.Condition = ConditionType.Fire;
                            spell.Tick = conditionDefaultTick[spell.Condition];
                            spell.StartDamage = 0;
                        }
                        else if (damageKinds == DamageKinds.PoisonCondition)
                        {
                            spell.Name = "condition";
                            spell.Condition = ConditionType.Poison;
                            spell.Tick = conditionDefaultTick[spell.Condition];
                            spell.StartDamage = 0;
                        }
                        else if (damageKinds == DamageKinds.EnergyCondition)
                        {
                            spell.Name = "condition";
                            spell.Condition = ConditionType.Energy;
                            spell.Tick = conditionDefaultTick[spell.Condition];
                            spell.StartDamage = 0;
                        }
                        else if (damageKinds == DamageKinds.Physical)
                        {
                            spell.DamageElement = CombatDamage.Physical;
                        }
                        else if (damageKinds == DamageKinds.Poison)
                        {
                            spell.DamageElement = CombatDamage.Earth;
                        }
                        else if (damageKinds == DamageKinds.Fire)
                        {
                            spell.DamageElement = CombatDamage.Fire;
                        }
                        else if (damageKinds == DamageKinds.Energy)
                        {
                            spell.DamageElement = CombatDamage.Energy;
                        }
                        else if (damageKinds == DamageKinds.LifeDrain)
                        {
                            spell.DamageElement = CombatDamage.LifeDrain;
                        }
                        else if (damageKinds == DamageKinds.ManaDrain)
                        {
                            spell.DamageElement = CombatDamage.ManaDrain;
                        }
                        // else There are no other damageKinds used and none are used as flags
                    }
                    else if (action == "drunken")
                    {
                        spell.Name = "drunk";
                        spell.SpellCategory = SpellCategory.Offensive;
                        double strength = double.Parse(actionParams[0]);
                        // It's a mistake that some monsters have drunkenness strength more then 6
                        strength = (strength > 6) ? 6 : strength;
                        spell.Drunkenness = 1 / (7 - strength);
                        spell.Duration = int.Parse(actionParams[2]) * 1000;
                    }
                    else if (action == "outfit")
                    {
                        spell.Name = "outfit";
                        spell.SpellCategory = (castType == "victim") ? SpellCategory.Offensive : SpellCategory.Defensive;

                        Match outfitMatch = Regex.Match(match.Value, @"(?<type>\d+), (?<head>\d+)-(?<body>\d+)-(?<legs>\d+)-(?<feet>\d+)\), (?<duration>\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        if (outfitMatch.Success)
                        {
                            spell.MonsterName = raceIdNameMap[int.Parse(outfitMatch.Groups["type"].Value)];
                            spell.Duration = int.Parse(outfitMatch.Groups["duration"].Value) * 1000;
                        }

                        outfitMatch = Regex.Match(match.Value, @"(?<type>\d+), (?<id>\d+)\), (?<duration>\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        if (outfitMatch.Success)
                        {
                            int type = int.Parse(outfitMatch.Groups["type"].Value);
                            ushort id = ushort.Parse(outfitMatch.Groups["id"].Value);
                            if ((type == 0) && (id == 0))
                            {
                                spell.Name = "invisible";
                            }
                            else
                            {
                                spell.ItemId = id;
                            }
                            spell.Duration = int.Parse(outfitMatch.Groups["duration"].Value) * 1000;
                        }
                    }
                    else if (action == "field")
                    {
                        if (actionParams[0] == "1") { spell.Name = "firefield"; }
                        else if (actionParams[0] == "2") { spell.Name = "poisonfield"; }
                        else if (actionParams[0] == "3") { spell.Name = "energyfield"; }
                        spell.SpellCategory = SpellCategory.Offensive;
                    }
                    else if (action == "summon")
                    {
                        int raceId = int.Parse(actionParams[0]);
                        int count = int.Parse(actionParams[1]);
                        monster.Summons.Add(new Summon()
                        {
                            Name = raceIdNameMap[raceId],
                            Max = count,
                            Chance = chance,
                            Interval = INTERVAL
                        });
                        monster.MaxSummons += count;
                        continue;
                    }
                    else if (action == "strength")
                    {
                        string effectedSkills = "";
                        if (actionParams[0] == "1") { effectedSkills = "fist, club, sword, axe"; }
                        else if (actionParams[0] == "2") { effectedSkills = "distance"; }
                        else if (actionParams[0] == "3") { effectedSkills = "fist, club, sword, axe, distance"; }
                        else if (actionParams[0] == "5") { effectedSkills = "fist, club, sword, axe, shielding"; }
                        int baseVal = int.Parse(actionParams[1]);
                        int variation = int.Parse(actionParams[2]);
                        spell.Duration = int.Parse(actionParams[3]) * 1000;
                        spell.SpellCategory = (baseVal < 0) ? SpellCategory.Offensive : SpellCategory.Defensive;
                        result.AppendMessage($"Unable to convert strength ability, {spell.SpellCategory} change {effectedSkills} by {baseVal} +/- {variation} for {spell.Duration}ms");
                        result.IncreaseError(ConvertError.Warning);
                        continue;
                    }
                    // else There are no other actions in the 7.7 monster pack

                    monster.Attacks.Add(spell);
                }
            }

            m = Regex.Match(fileContents, @"Talk.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Groups[1].Value, "\"(.*?)\"", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    SoundLevel volume = SoundLevel.Say;
                    string sound = match.Groups[1].Value;
                    if (sound.StartsWith("#Y") || sound.StartsWith("#y"))
                    {
                        volume = SoundLevel.Yell;
                        sound = sound.Substring(3);
                    }
                    else if (sound.StartsWith("#W") || sound.StartsWith("#w"))
                    {
                        volume = SoundLevel.Whisper;
                        sound = sound.Substring(3);
                    }
                    monster.Voices.Add(new Voice(sound, volume));
                }
            }

            m = Regex.Match(fileContents, @"Inventory.*?= \{(.*?)\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (m.Success)
            {
                var matches = Regex.Matches(m.Groups[1].Value, @"\((?<id>\d+), (?<count>\d+), (?<chance>\d+)\)", RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    monster.Items.Add(new LootItem()
                    {
                        Id = ushort.Parse(match.Groups["id"].Value),
                        Count = int.Parse(match.Groups["count"].Value),
                        Chance = decimal.Parse(match.Groups["chance"].Value) / 1000
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
