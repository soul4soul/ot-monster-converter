using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.MonsterTypes
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
            LookTypeDetails = new DetailedLookType();
            Attacks = new List<Spell>();

            SummonCost = 0;
            Attackable = true;
            Hostile = true;
            Illusionable = false;
            ConvinceCost = 0;
            Pushable = false;
            PushItems = false;
            PushCreatures = false;
            TargetDistance = 1;
            StaticAttack = 95;
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

            // Elements
            Fire = 1;
            Earth = 1;
            Energy = 1;
            Ice = 1;
            Holy = 1;
            Death = 1;
            Physical = 1;
            Drown = 1;
            LifeDrain = 1;
            ManaDrain = 1;
        }

        // Properties
        // Generic
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public uint Health { get; set; }
        public uint Experience { get; set; }
        public uint Speed { get; set; }
        public IList<Voice> Voices { get; set; }
        public Blood Race { get; set; }
        public uint RetargetInterval { get; set; }
        public uint RetargetChance { get; set; }
        public uint MaxSummons { get; set; }
        public IList<Summon> Summons { get; set; }

        // Look
        public uint CorpseId { get; set; }
        public uint OutfitIdLookType { get; set; }
        public uint ItemIdLookType { get; set; } // none 0 means creature looks like an item
        public DetailedLookType LookTypeDetails { get; set; }

        // Behavior
        public uint SummonCost { get; set; }
        public bool Attackable { get; set; }
        public bool Hostile { get; set; }
        public bool Illusionable { get; set; }
        public uint ConvinceCost { get; set; }
        public bool Pushable { get; set; }
        public bool PushItems { get; set; }
        public bool PushCreatures { get; set; }
        public uint TargetDistance { get; set; }
        public uint RunOnHealth { get; set; }
        public uint StaticAttack { get; set; }
        public uint LightLevel { get; set; }
        public uint LightColor { get; set; }
        public bool HideHealth { get; set; }
        public bool IsBoss { get; set; }

        // Walk Behavior
        public bool AvoidFire { get; set; }
        public bool AvoidEnergy { get; set; }
        public bool AvoidPoison { get; set; }

        // Attacks
        public IList<Spell> Attacks { get; set; }

        // Defeneses
        public uint TotalArmor { get; set; }
        public uint Shielding { get; set; }
        public double Fire { get; set; }
        public double Earth { get; set; }
        public double Energy { get; set; }
        public double Ice { get; set; }
        public double Holy { get; set; }
        public double Death { get; set; }
        public double Physical { get; set; }
        public double Drown { get; set; }
        public double LifeDrain { get; set; }
        public double ManaDrain { get; set; }

        // Immunities
        public bool IgnoreParalyze { get; set; }
        public bool IgnoreInvisible { get; set; }
        public bool IgnoreDrunk { get; set; }
        public bool IgnoreOutfit { get; set; }
        public bool IgnoreBleed { get; set; }

        // Loot
        public IList<Loot> Items { get; set; }
    }
}
