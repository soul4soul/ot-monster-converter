﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Summon
    {
        public string Name { get; set; }
        public int Interval { get; set; }
        public double Chance { get; set; }
        public int Max { get; set; }
        public bool Force { get; set; } = false;
    }
}
