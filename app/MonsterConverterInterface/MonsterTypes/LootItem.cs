using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class LootItem
    {
        private decimal chance = 0;

        public LootItem()
        {
            NestedLoot = new List<LootItem>();
        }

        public string ComboIdentifier
        {
            get { return $"{Id}:{Name}"; }
        }

        public ushort Id { get; set; }

        public string Name { get; set; }

        public decimal Chance
        {
            get { return chance; }
            set { chance = Math.Min(value, 1); }
        }

        public int Count { get; set; }

        /// <summary>
        /// Fluids in containers or rune charges
        /// </summary>
        public int SubType { get; set; }

        public int ActionId { get; set; }

        /// <summary>
        /// Sets text on writables like a letter
        /// </summary>
        public string Text { get; set; }

        public List<LootItem> NestedLoot { get; }

        /// <summary>
        /// Free form string field not used by any engine, information stored here is useful for maintaining a server
        /// For example, this field can contain the item's name
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Basic reward chest implementation for now, cipbia has a more advanced system with partcipation tiers
        /// https://tibia.fandom.com/wiki/Cooperative_Loot_System but no open source server support such concepts
        /// The existing open source engines barely support loot chests and usually at bests support unique rares
        /// </summary>
        public bool RewardChest { get; set; }

        public override string ToString()
        {
            if (NestedLoot.Count > 0)
            {
                return $"{ComboIdentifier} {Count} {Chance:N4} (Nested: {NestedLoot.Count})";
            }
            else
            {
                return $"{ComboIdentifier} {Count} {Chance:N4}";
            }
        }
    }
}
