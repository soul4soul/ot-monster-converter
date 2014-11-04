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
        public ushort Rate { get; set; }
        public ushort Chance { get; set; }
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
        //public uint maxSummons;
        public Blood Race { get; set; }
        //public List<Summon> summons;

            // Look
        public uint CorpseId { get; set; }
        public uint OutfitIdLookType { get; set; }
        public uint ItemIdLookType { get; set; } // none 0 means creature looks like an item
        public IDetailedLookType LookTypeDetails { get; set; }

            // Behavior
        public uint Summonable { get; set; }
        public bool Hostile { get; set; }
        public bool Illusionable { get; set; }
        public uint Convinceable { get; set; }
        public bool Pushable { get; set; }
        public bool PushItems { get; set; }
        public bool PushCreatures { get; set; }
        public bool TargetDistance { get; set; }
        public uint RunOnHealth { get; set; }

            // Defeneses
        public uint Armor { get; set; }
        public uint Fire { get; set; }
        public uint Earth { get; set; }
        public uint Energy { get; set; }
        public uint Ice { get; set; }
        public uint Holy { get; set; }
        public uint Death { get; set; }
        public uint Physical { get; set; }
        public uint Drown { get; set; }

            // Immunities
        public bool Paralyze { get; set; }
        public bool Invisible { get; set; }
        public bool LifeDrain { get; set; }
        public bool Drunk { get; set; }
        public bool ManaDrain { get; set; }

        // Functions
    }
}
