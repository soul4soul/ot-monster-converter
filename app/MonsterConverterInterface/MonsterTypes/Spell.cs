using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Spell
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SpellCategory SpellCategory { get; set; }
        public int? MinDamage { get; set; }
        public int? MaxDamage { get; set; }
        public Effect AreaEffect { get; set; }
        public Animation ShootEffect { get; set; }
        public double Chance { get; set; }
        public int Interval { get; set; }
        public int? Range { get; set; }
        public int? Radius { get; set; }
        public int? Length { get; set; }
        public int? Spread { get; set; }
        public bool? OnTarget { get; set; }
        public bool? IsDirectional { get; set; }
        public SpellDefinition DefinitionStyle { get; set; }
        // Magic damage
        public CombatDamage? DamageElement { get; set; }
        // Speed
        public int? MinSpeedChange { get; set; }
        public int? MaxSpeedChange { get; set; }
        public int? Duration { get; set; }
        // Melee damage overrides
        public int? AttackValue { get; set; }
        public int? Skill { get; set; }
        // Spell Condition stuff (used for condition only + melee conditions)
        public int? Tick { get; set; }
        public int? StartDamage { get; set; }
        public ConditionType Condition { get; set; }
        // Outfit stuff
        public string MonsterName { get; set; }
        public int? ItemId { get; set; }
        // Drunk
        public double? Drunkenness { get; set; }

        public override string ToString()
        {
            string category = (SpellCategory == SpellCategory.Offensive) ? "Offenseive" : "Defensive";
            return $"{category}: {Name} ({Description})";
        }
    }
}
