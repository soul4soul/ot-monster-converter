using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.MonsterTypes
{
    public class Loot
    {
        public string Item { get; set; }
        public decimal Chance { get; set; }
        public decimal Count { get; set; }

        public override string ToString()
        {
            return $"{Item} {Count} {Chance:N4}";
        }
    }
}
