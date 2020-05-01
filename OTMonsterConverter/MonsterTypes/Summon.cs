using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.MonsterTypes
{
    public class Summon
    {
        public string Name { get; set; }
        public uint Rate { get; set; }
        public uint Chance { get; set; }
        public int Max { get; set; }
        public bool Force { get; set; }
    }
}
