﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class LookData
    {
        // Variables
        private const int MAX_COLOR = 132;

        private int _Head;
        private int _Body;
        private int _Legs;
        private int _Feet;

        // Properties
        public LookType LookType { get; set; }

        public int LookId { get; set; }

        public ushort CorpseId { get; set; }

        public int Head
        {
            get { return _Head; }
            set { _Head = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public int Body
        {
            get { return _Body; }
            set { _Body = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public int Legs
        {
            get { return _Legs; }
            set { _Legs = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }
        public int Feet
        {
            get { return _Feet; }
            set { _Feet = (value > MAX_COLOR) ? MAX_COLOR : value; }
        }

        public int Addons { get; set; }

        public int Mount { get; set; }

        public void CopyFrom(LookData other)
        {
            LookType = other.LookType;
            LookId = other.LookId;
            Head = other.Head;
            Body = other.Body;
            Legs = other.Legs;
            Feet = other.Feet;
            Addons = other.Addons;
        }
    }
}
