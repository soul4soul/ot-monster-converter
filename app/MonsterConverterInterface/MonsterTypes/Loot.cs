using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Loot
    {
        public string Item { get; set; }
        public decimal Chance { get; set; }
        public int Count { get; set; }
        public int SubType { get; set; } // Fluids in containers or rune charges
        public int ActionId { get; set; }
        public string Text { get; set; } // Sets text on writables like a letter
        public List<Loot> NestedLoot { get; }

        public Loot()
        {
            NestedLoot = new List<Loot>();
        }

        public override string ToString()
        {
            if (NestedLoot.Count > 0)
            {
                return $"{Item} {Count} {Chance:N4} (Nested: {NestedLoot.Count})";
            }
            else
            {
                return $"{Item} {Count} {Chance:N4}";
            }
        }
    }
}
