using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    //todo should we add outfit ID to this class?
    public class DetailedLookType
    {
        // Variables
        private const ushort MAX_COLOR = 132;

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
        public ushort Addons { get; set; }
        public ushort Mount { get; set; }
    }
}
