using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    //todo should I add proper display names?
    public enum Blood
    {
        blood,
        venom,
        undead,
        fire,
        energy
    }

    //todo should we add outfit ID to this class?
    public class DetailedLookType : IDetailedLookType
    {
        // Variables
        private const ushort MAX_COLOR = 132; //todo is this correct?

        private ushort _Head;
        private ushort _Body;
        private ushort _Legs;
        private ushort _Feet;

        // Properties
        public ushort Head
        {
            get { return _Head; }
            set { _Head = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Body
        {
            get { return _Body; }
            set { _Body = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Legs
        {
            get { return _Legs; }
            set { _Legs = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public ushort Feet
        {
            get { return _Feet; }
            set { _Feet = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
    }

    public class CustomSummon : ICustomSummon
    {
        public string Name { get; set; }
        public uint Rate { get; set; }
        public double Chance { get; set; }
    }

    //public class Loot
    //{
    //    //todo: name count be ID or string
    //    public uint max;
    //    public uint chance;
    //}

    public class CustomMonster : ICustomMonster
    {
        // Member Variables

        // Constructors
        public CustomMonster()
        {
            Voices = new List<string>();
            MaxSummons = 0;
            Summons = new List<CustomSummon>();


            SummonCost     = 0;
            Hostile        = true;
            Illusionable   = false;
            ConvinceCost   = 0;
            Pushable       = false;
            PushItems      = false;
            PushCreatures  = false;
            TargetDistance = 1;
            RunOnHealth    = 0;

            avoidFire   = false;
            avoidEnergy = false;
            avoidPoison = false;

            IgnoreParalyze  = false;
            IgnoreInvisible = false;
            LifeDrain = 0;
            IgnoreDrunk     = false;
            ManaDrain = 0;
            IgnoreOutfit    = false;

            TotalArmor = 10;
            Shielding  = 5;
            Fire     = 1;
            Earth    = 1;
            Energy   = 1;
            Ice      = 1;
            Holy     = 1;
            Death    = 1;
            Physical = 1;
            Drown    = 1;
        }

        // Events

        // Properties
            // Generic
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Health { get; set; }
        public uint Experience { get; set; }
        public uint Speed { get; set; }
        public List<string> Voices { get; set; }
        public Blood Race { get; set; }
        public uint ManaCost { get; set; }
        public uint RetargetChance { get; set; }
        public uint MaxSummons { get; set; }
        public List<CustomSummon> Summons { get; set; }

            // Look
        public uint CorpseId { get; set; }
        public uint OutfitIdLookType { get; set; }
        public uint ItemIdLookType { get; set; } // none 0 means creature looks like an item
        public IDetailedLookType LookTypeDetails { get; set; }

            // Behavior
        public uint SummonCost { get; set; }
        public bool Hostile { get; set; }
        public bool Illusionable { get; set; }
        public uint ConvinceCost { get; set; }
        public bool Pushable { get; set; }
        public bool PushItems { get; set; }
        public bool PushCreatures { get; set; }
        public uint TargetDistance { get; set; }
        public uint RunOnHealth { get; set; }

            // Walk Behavior
        public bool avoidFire { get; set; }
        public bool avoidEnergy { get; set; }
        public bool avoidPoison { get; set; }

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

        // Functions
    }
}
