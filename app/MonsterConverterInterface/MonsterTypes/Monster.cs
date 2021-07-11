using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Monster
    {
        // Constructors
        public Monster()
        {
            Voices = new List<Voice>();
            MaxSummons = 0;
            Summons = new List<Summon>();
            Items = new List<Loot>();
            Look = new LookData();
            Attacks = new List<Spell>();
            Scripts = new List<Script>();
            TargetStrategy = new TargetStrategy();

            SummonCost = 0;
            Attackable = true;
            IsHostile = true;
            IsIllusionable = false;
            ConvinceCost = 0;
            IsPushable = false;
            PushItems = false;
            PushCreatures = false;
            TargetDistance = 1;
            StaticAttackChance = 90;
            LightLevel = 0;
            LightColor = 0;
            RunOnHealth = 0;
            IsBoss = false;
            HideHealth = false;
            AvoidFire = true;
            AvoidEnergy = true;
            AvoidPoison = true;

            // Immunities
            IgnoreParalyze = false;
            IgnoreInvisible = false;
            IgnoreDrunk = false;
            IgnoreOutfit = false;

            // Defences
            TotalArmor = 10;
            Shielding = 5;

            // Damage Modifiers
            FireDmgMod = 1;
            EarthDmgMod = 1;
            EnergyDmgMod = 1;
            IceDmgMod = 1;
            HolyDmgMod = 1;
            DeathDmgMod = 1;
            PhysicalDmgMod = 1;
            DrownDmgMod = 1;
            LifeDrainDmgMod = 1;
            ManaDrainDmgMod = 1;
            HealingMod = 1;
        }

        // Properties
        // Generic
        /// <summary>
        /// Name of the monster as seen in game
        /// </summary>
        public string Name { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// The common name of the monster, which is used for some formats to register the monster with the game engine
        /// An example would be "Red Butterfly" as it's commonly called and used to register in a game engine
        /// but in game "Red Butterfly" will use the name "Butterfly" which should be set in name field
        /// Another example would be a demon goblin ingame seen as a demon
        /// </summary>
        public string RegisteredName { get; set; }
        public string Description { get; set; }
        public int Health { get; set; }
        public int Experience { get; set; }
        public int Speed { get; set; }
        public IList<Voice> Voices { get; }
        public Blood Race { get; set; }
        public int MaxSummons { get; set; }
        public IList<Summon> Summons { get; }
        public LookData Look { get; }

        // Other
        public int SummonCost { get; set; }
        public bool Attackable { get; set; }
        public bool IsIllusionable { get; set; }
        public int ConvinceCost { get; set; }
        public int LightLevel { get; set; }
        public int LightColor { get; set; }
        public bool HideHealth { get; set; }
        public bool IsBoss { get; set; }

        // Behavior
        public bool IsPushable { get; set; }
        public bool PushItems { get; set; }
        public bool PushCreatures { get; set; }
        public bool IsHostile { get; set; }
        public int RetargetInterval { get; set; }
        public double RetargetChance { get; set; }
        public int TargetDistance { get; set; }
        public int RunOnHealth { get; set; }
        public int StaticAttackChance { get; set; } // Static attack controls how much the monster dances/moves when attacking a target, 100 = completely static, 0 = always dance
        public TargetStrategy TargetStrategy { get; }

        // Walk Behavior
        public bool AvoidFire { get; set; }
        public bool AvoidEnergy { get; set; }
        public bool AvoidPoison { get; set; }

        // Attacks
        public IList<Spell> Attacks { get; }

        // Defeneses
        public int TotalArmor { get; set; }
        public int Shielding { get; set; }
        public double FireDmgMod { get; set; }
        public double EarthDmgMod { get; set; }
        public double EnergyDmgMod { get; set; }
        public double IceDmgMod { get; set; }
        public double HolyDmgMod { get; set; }
        public double DeathDmgMod { get; set; }
        public double PhysicalDmgMod { get; set; }
        public double DrownDmgMod { get; set; }
        public double LifeDrainDmgMod { get; set; }
        public double ManaDrainDmgMod { get; set; }
        public double HealingMod { get; set; }  // This is an element type, some mobs like Leiden take damage from healing

        // Immunities
        public bool IgnoreParalyze { get; set; }
        public bool IgnoreInvisible { get; set; }
        public bool IgnoreDrunk { get; set; }
        public bool IgnoreOutfit { get; set; }
        public bool IgnoreBleed { get; set; }

        // Loot
        public IList<Loot> Items { get; }

        // Attached Scripts
        public IList<Script> Scripts { get; }
    }
}
