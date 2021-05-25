using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Spell
    {
        public string Name { get; set; }
        public SpellCategory SpellCategory { get; set; }
        public int? MinDamage { get; set; }
        public int? MaxDamage { get; set; }
        public Effect AreaEffect { get; set; }
        public Animation ShootEffect { get; set; }
        public double Chance { get; set; }
        public uint Interval { get; set; }
        public uint? Range { get; set; }
        public uint? Radius { get; set; }
        public uint? Length { get; set; }
        public uint? Spread { get; set; }
        public bool? OnTarget { get; set; }
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
    }
}
