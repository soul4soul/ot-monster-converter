using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Loot
    {
        public string Item { get; set; }
        public decimal Chance { get; set; }
        public decimal Count { get; set; }
        public decimal SubType { get; set; } // Fluids in containers or rune charges
        public int ActionId { get; set; }
        public string Text { get; set; } // Sets text on writables like a letter

        public override string ToString()
        {
            return $"{Item} {Count} {Chance:N4}";
        }
    }
}
