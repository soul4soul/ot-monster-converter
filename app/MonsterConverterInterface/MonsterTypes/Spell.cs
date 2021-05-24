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
        public CombatDamage? DamageElement { get; set; }
        public Effect? AreaEffect { get; set; }
        public Animation? ShootEffect { get; set; }
        public uint? Chance { get; set; }
        public uint? Interval { get; set; }
        public uint? Range { get; set; }
        public uint? Radius { get; set; }
        public uint? Length { get; set; }
        public uint? Spread { get; set; }
        public bool? Target { get; set; }
        // Speed
        public int? SpeedChange { get; set; }
        public int? Duration { get; set; }
        // Melee Stuff
        public int? AttackValue { get; set; }
        public int? Skill { get; set; }
        // Spell Condition stuff
        public int? Tick { get; set; }
        public int? StartDamage { get; set; }
        public ConditionType? Condition { get; set; }
        // Outfit stuff
        public string MonsterName { get; set; }
        public int? ItemId { get; set; }
    }
}
