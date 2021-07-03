using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Summon
    {
        public string Name { get; set; }
        public int Interval { get; set; } = 2000;
        public double Chance { get; set; } = 0.15;
        public int Max { get; set; } = 1;
        public bool Force { get; set; } = false;
    }
}
