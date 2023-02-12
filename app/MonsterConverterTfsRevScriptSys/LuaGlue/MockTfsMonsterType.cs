using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MonsterConverterTfsRevScriptSys
{
    [MoonSharpUserData]
    internal class MockTfsMonsterType
    {
        const int MAX_LOOTCHANCE = 100000;

        public string Name { get; set; }

        public MockTfsMonsterType(string name)
        {
            Name = name;
        }

        // TODO Events?
        public object onThink { get; set; }
        public object onAppear { get; set; }
        public object onDisappear { get; set; }
        public object onMove { get; set; }
        public object onSay { get; set; }

        public void register(Table t)
        {
            Monster mon = new Monster();
            DynValue dv;
            ConvertResultEventArgs result = new ConvertResultEventArgs("temp");

            dv = t.Get("name");
            if (dv.Type == DataType.String)
            {
                mon.Name = dv.String;
            }

            dv = t.Get("description");
            if (dv.Type == DataType.String)
            {
                mon.Description = dv.String;
            }

            dv = t.Get("experience");
            if (dv.Type == DataType.Number)
            {
                mon.Experience = (int)dv.Number;
            }

            dv = t.Get("maxHealth");
            if (dv.Type == DataType.Number)
            {
                mon.Health = (int)dv.Number;
            }

            dv = t.Get("health");
            if (dv.Type == DataType.Number)
            {
                result.AppendMessage("Health field not supported defaulting to max health");
                result.IncreaseError(ConvertError.Warning);
            }

            dv = t.Get("runHealth");
            if (dv.Type == DataType.Number)
            {
                mon.RunOnHealth = (int)dv.Number;
            }

            dv = t.Get("speed");
            if (dv.Type == DataType.Number)
            {
                mon.Speed = (int)dv.Number;
            }

            dv = t.Get("light");
            if (dv.Type == DataType.Table)
            {
                Table light = dv.Table;
                dv = light.Get("color");
                if (dv.Type == DataType.Number)
                {
                    mon.LightColor = (int)dv.Number;
                }

                dv = light.Get("level");
                if (dv.Type == DataType.Number)
                {
                    mon.LightLevel = (int)dv.Number;
                }
            }

            dv = t.Get("changeTarget");
            if (dv.Type == DataType.Table)
            {
                Table light = dv.Table;
                dv = light.Get("chance");
                if (dv.Type == DataType.Number)
                {
                    mon.RetargetChance = (int)dv.Number;
                }

                dv = light.Get("interval");
                if (dv.Type == DataType.Number)
                {
                    mon.RetargetInterval = (int)dv.Number;
                }
            }

            int summonCost = 0;
            dv = t.Get("manaCost");
            if (dv.Type == DataType.Number)
            {
                summonCost = (int)dv.Number;
            }

            dv = t.Get("flags");
            if (dv.Type == DataType.Table)
            {
                Table flags = dv.Table;
                dv = flags.Get("attackable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.Attackable = dv.Boolean;
                }

                dv = flags.Get("healthHidden");
                if (dv.Type == DataType.Boolean)
                {
                    mon.HideHealth = dv.Boolean;
                }

                dv = flags.Get("boss");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsBoss = dv.Boolean;
                }

                dv = flags.Get("challengeable");
                if (dv.Type == DataType.Boolean)
                {
                    // not supported
                }

                dv = flags.Get("convinceable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.ConvinceCost = summonCost;
                }

                dv = flags.Get("summonable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.SummonCost = summonCost;
                }

                dv = flags.Get("ignoreSpawnBlock");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IgnoreSpawnBlock = dv.Boolean;
                }

                dv = flags.Get("illusionable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsIllusionable = dv.Boolean;
                }

                dv = flags.Get("hostile");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsHostile = dv.Boolean;
                }

                dv = flags.Get("pushable");
                if (dv.Type == DataType.Boolean)
                {
                    mon.IsPushable = dv.Boolean;
                }

                dv = flags.Get("canPushItems");
                if (dv.Type == DataType.Boolean)
                {
                    mon.PushItems = dv.Boolean;
                }

                dv = flags.Get("canPushCreatures");
                if (dv.Type == DataType.Boolean)
                {
                    mon.PushCreatures = dv.Boolean;
                }

                dv = flags.Get("targetDistance");
                if (dv.Type == DataType.Number)
                {
                    mon.TargetDistance = (int)dv.Number;
                }

                dv = flags.Get("staticAttackChance");
                if (dv.Type == DataType.Number)
                {
                    mon.StaticAttackChance = (int)dv.Number;
                }

                dv = flags.Get("canWalkOnEnergy");
                if (dv.Type == DataType.Boolean)
                {
                    mon.AvoidEnergy = !dv.Boolean;
                }

                dv = flags.Get("canWalkOnFire");
                if (dv.Type == DataType.Boolean)
                {
                    mon.AvoidFire = !dv.Boolean;
                }

                dv = flags.Get("canWalkOnPoison");
                if (dv.Type == DataType.Boolean)
                {
                    mon.AvoidPoison = dv.Boolean;
                }
            }

            dv = t.Get("skull");
            if (dv.Type == DataType.Number)
            {
                result.AppendMessage("Skull field not supported");
                result.IncreaseError(ConvertError.Warning);
            }

            dv = t.Get("corpse");
            if (dv.Type == DataType.Number)
            {
                mon.Look.CorpseId = (ushort)dv.Number;
            }
            dv = t.Get("outfit");
            if (dv.Type == DataType.Table)
            {
                Table outfit = dv.Table;

                dv = outfit.Get("lookTypeEx");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.LookType = LookType.Item;
                    mon.Look.LookId = (int)dv.Number;
                }

                dv = outfit.Get("lookType");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.LookType = LookType.Outfit;
                    mon.Look.LookId = (int)dv.Number;
                }
                dv = outfit.Get("lookHead");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Head = (int)dv.Number;
                }
                dv = outfit.Get("lookBody");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Body = (int)dv.Number;
                }
                dv = outfit.Get("lookLegs");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Legs = (int)dv.Number;
                }
                dv = outfit.Get("lookFeet");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Feet = (int)dv.Number;
                }
                dv = outfit.Get("lookAddons");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Addons = (int)dv.Number;
                }
                dv = outfit.Get("lookMount");
                if (dv.Type == DataType.Number)
                {
                    mon.Look.Mount = (int)dv.Number;
                }
            }

            dv = t.Get("maxSummons");
            if (dv.Type == DataType.Number)
            {
                mon.MaxSummons = (ushort)dv.Number;
            }
            dv = t.Get("summons");
            if (dv.Type == DataType.Table)
            {
                Table summons = dv.Table;
                for (int i = 1; i <= summons.Length; i++)
                {
                    dv = summons.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        mon.Summons.Add(GetSummon(dv.Table));
                    }
                }
            }

            mon.Race = TfsRevScriptSysRaceToGenericBlood(t.Get("race"));

            dv = t.Get("voices");
            if (dv.Type == DataType.Table)
            {
                Table voices = dv.Table;

                dv = voices.Get("interval");
                if (dv.Type == DataType.Number)
                {
                    mon.VoiceInterval = (int)dv.Number;
                }

                dv = voices.Get("chance");
                if (dv.Type == DataType.Number)
                {
                    mon.VoiceChance = (int)dv.Number;
                }

                for (int i = 1; i <= voices.Length; i++)
                {
                    dv = voices.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        mon.Voices.Add(GetVoice(dv.Table));
                    }
                }
            }

            // Valid Elements are from enum CombatType_t
            dv = t.Get("elements");
            if (dv.Type == DataType.Table)
            {
                Table elements = dv.Table;

                for (int i = 1; i <= elements.Length; i++)
                {
                    dv = elements.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        Table element = dv.Table;
                        double percent = 0;

                        dv = element.Get("percent");
                        if (dv.Type == DataType.Number)
                        {
                            percent = dv.Number;
                        }

                        dv = element.Get("type");
                        if (dv.Type == DataType.Number)
                        {
                            CombatDamage damageType = (CombatDamage)dv.Number;
                            if (damageType == CombatDamage.Physical)
                            {
                                mon.PhysicalDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Energy)
                            {
                                mon.EnergyDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Earth)
                            {
                                mon.EarthDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Fire)
                            {
                                mon.FireDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.LifeDrain)
                            {
                                mon.LifeDrainDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.ManaDrain)
                            {
                                mon.ManaDrainDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Healing)
                            {
                                mon.HealingMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Drown)
                            {
                                mon.DrownDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Ice)
                            {
                                mon.IceDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Holy)
                            {
                                mon.HolyDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                            else if (damageType == CombatDamage.Death)
                            {
                                mon.DeathDmgMod = TfsRevScriptSysToGenericElementalPercent(dv.Number);
                            }
                        }
                    }
                }
            }

            // Valid immunities
            // For combat LuaScriptInterface::luaMonsterTypeCombatImmunities
            // For condition LuaScriptInterface::luaMonsterTypeConditionImmunities
            dv = t.Get("immunities");
            if (dv.Type == DataType.Table)
            {
                Table immunities = dv.Table;

                for (int i = 1; i <= immunities.Length; i++)
                {
                    dv = immunities.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        Table immunity = dv.Table;
                        Boolean combatImmune = false;
                        Boolean conditionImmune = false;

                        dv = immunity.Get("combat");
                        if (dv.Type == DataType.Boolean)
                        {
                            combatImmune = dv.Boolean;
                        }

                        dv = immunity.Get("condition");
                        if (dv.Type == DataType.Boolean)
                        {
                            conditionImmune = dv.Boolean;
                        }

                        dv = immunity.Get("type");
                        if (dv.Type == DataType.String)
                        {
                            if (dv.String == "physical")
                            {
                                if (combatImmune)
                                {
                                    mon.PhysicalDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "energy")
                            {
                                if (combatImmune)
                                {
                                    mon.EnergyDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "fire")
                            {
                                if (combatImmune)
                                {
                                    mon.FireDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "poison" || dv.String == "earth")
                            {
                                if (combatImmune)
                                {
                                    mon.EarthDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "drown")
                            {
                                if (combatImmune)
                                {
                                    mon.DrownDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "ice")
                            {
                                if (combatImmune)
                                {
                                    mon.IceDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "holy")
                            {
                                if (combatImmune)
                                {
                                    mon.HolyDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "death")
                            {
                                if (combatImmune)
                                {
                                    mon.DeathDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "lifedrain")
                            {
                                if (combatImmune)
                                {
                                    mon.LifeDrainDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "manadrain")
                            {
                                if (combatImmune)
                                {
                                    mon.ManaDrainDmgMod = 0;
                                }
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                            else if (dv.String == "outfit")
                            {
                                mon.IgnoreOutfit = conditionImmune;
                            }
                            else if (dv.String == "drunk")
                            {
                                mon.IgnoreDrunk = conditionImmune;
                            }
                            else if (dv.String == "invisible" || dv.String == "invisibility")
                            {
                                mon.IgnoreInvisible = conditionImmune;
                            }
                            else if (dv.String == "bleed")
                            {
                                if (conditionImmune)
                                {
                                    result.AppendMessage($"Condition immunity {dv.String} not supported");
                                    result.IncreaseError(ConvertError.Warning);
                                }
                            }
                        }
                    }
                }
            }

            dv = t.Get("loot");
            if (dv.Type == DataType.Table)
            {
                Table loot = dv.Table;

                for (int i = 1; i <= loot.Length; i++)
                {
                    dv = loot.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        Table lootItemTable = dv.Table;
                        LootItem lootItem = ParseLootItem(lootItemTable);

                        dv = lootItemTable.Get("child");
                        if (dv.Type == DataType.Table)
                        {
                            Table child = dv.Table;

                            for (int j = 1; j <= child.Length; j++)
                            {
                                dv = child.Get(j);
                                lootItem.NestedLoot.Add(ParseLootItem(dv.Table));
                            }
                        }

                        mon.Items.Add(lootItem);
                    }
                }
            }

            dv = t.Get("attacks");
            if (dv.Type == DataType.Table)
            {
                Table attacks = dv.Table;

                for (int i = 1; i <= attacks.Length; i++)
                {
                    dv = attacks.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        Table attack = dv.Table;
                        mon.Attacks.Add(ParseSpell(attack, SpellCategory.Offensive));
                    }
                }
            }

            dv = t.Get("defenses");
            if (dv.Type == DataType.Table)
            {
                Table defenses = dv.Table;

                dv = defenses.Get("defense");
                if (dv.Type == DataType.Number)
                {
                    mon.Shielding = (int)dv.Number;
                }

                dv = defenses.Get("armor");
                if (dv.Type == DataType.Number)
                {
                    mon.TotalArmor = (int)dv.Number;
                }

                for (int i = 1; i <= defenses.Length; i++)
                {
                    dv = defenses.Get(i);
                    if (dv.Type == DataType.Table)
                    {
                        Table defense = dv.Table;
                        mon.Attacks.Add(ParseSpell(defense, SpellCategory.Defensive));
                    }
                }
            }

            if (onAppear != null)
            {
                result.AppendMessage("OnAppear script can't be converted");
                result.IncreaseError(ConvertError.Warning);
            }
            if (onDisappear != null)
            {
                result.AppendMessage("onDisappear script can't be converted");
                result.IncreaseError(ConvertError.Warning);
            }
            if (onMove != null)
            {
                result.AppendMessage("onMove script can't be converted");
                result.IncreaseError(ConvertError.Warning);
            }
            if (onThink != null)
            {
                result.AppendMessage("onThink script can't be converted");
                result.IncreaseError(ConvertError.Warning);
            }
            if (onSay != null)
            {
                result.AppendMessage("onSay script can't be converted");
                result.IncreaseError(ConvertError.Warning);
            }

            MockTfsGame.ConvertedMonsters.Enqueue(new Tuple<Monster, ConvertResultEventArgs>(mon, result));

            return;
        }

        private Spell ParseSpell(Table t, SpellCategory category)
        {
            DynValue dv;
            Spell spell = new Spell();
            spell.SpellCategory = category;

            dv = t.Get("chance");
            if (dv.Type == DataType.Number)
            {
                spell.Chance = dv.Number / 100.0;
            }

            dv = t.Get("interval");
            if (dv.Type == DataType.Number)
            {
                spell.Interval = (int)dv.Number;
            }

            dv = t.Get("minDamage");
            if (dv.Type == DataType.Number)
            {
                spell.MinDamage = (int)dv.Number;
            }

            dv = t.Get("maxDamage");
            if (dv.Type == DataType.Number)
            {
                spell.MaxDamage = (int)dv.Number;
            }

            dv = t.Get("name");
            if (dv.Type == DataType.String)
            {
                spell.DefinitionStyle = SpellDefinition.Raw;
                spell.Name = dv.String;

                if (spell.Name == "melee")
                {
                    dv = t.Get("attack");
                    if (dv.Type == DataType.Number)
                    {
                        spell.AttackValue = (int)dv.Number;
                    }

                    dv = t.Get("skill");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Skill = (int)dv.Number;
                    }

                    dv = t.Get("effect");
                    if (dv.Type == DataType.Number)
                    {
                        spell.AreaEffect = (Effect)dv.Number;
                    }
                }
                else
                {
                    dv = t.Get("type");
                    if (dv.Type == DataType.Number)
                    {
                        spell.DamageElement = (CombatDamage)dv.Number;
                    }

                    dv = t.Get("range");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Range = (int)dv.Number;
                    }

                    dv = t.Get("duration");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Duration = (int)dv.Number;
                    }

                    dv = t.Get("speed");
                    if (dv.Type == DataType.Table)
                    {
                        Table speed = dv.Table;

                        dv = speed.Get("max");
                        if (dv.Type == DataType.Number)
                        {
                            spell.MaxSpeedChange = (int)dv.Number;
                        }
                        dv = speed.Get("min");
                        if (dv.Type == DataType.Number)
                        {
                            spell.MinSpeedChange = (int)dv.Number;
                        }
                    }
                    else if (dv.Type == DataType.Number)
                    {
                        spell.MinSpeedChange = (int)dv.Number;
                        spell.MaxSpeedChange = (int)dv.Number;
                    }

                    dv = t.Get("target");
                    if (dv.Type == DataType.Boolean)
                    {
                        spell.OnTarget = dv.Boolean;
                    }

                    dv = t.Get("length");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Length = (int)dv.Number;
                    }

                    dv = t.Get("spread");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Spread = (int)dv.Number;
                    }

                    dv = t.Get("radius");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Radius = (int)dv.Number;
                    }

                    dv = t.Get("ring");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Ring = (int)dv.Number;
                    }

                    dv = t.Get("effect");
                    if (dv.Type == DataType.Number)
                    {
                        spell.AreaEffect = (Effect)dv.Number;
                    }

                    dv = t.Get("shootEffect");
                    if (dv.Type == DataType.Number)
                    {
                        spell.ShootEffect = (Missile)dv.Number;
                    }

                    dv = t.Get("outfit");
                    if (dv.Type == DataType.Table)
                    {
                        // todo error not supported
                    }
                    else if (dv.Type == DataType.Number)
                    {
                        spell.ItemId = (ushort)dv.Number;
                    }
                    else if (dv.Type == DataType.String)
                    {
                        spell.MonsterName = dv.String;
                    }

                    dv = t.Get("drunk");
                    if (dv.Type != DataType.Nil)
                    {
                        spell.Condition = ConditionType.Drunk;
                    }

                    dv = t.Get("drunkenness");
                    if (dv.Type == DataType.Number)
                    {
                        spell.Drunkenness = dv.Number / 100.0;
                    }
                }
            }

            dv = t.Get("condition");
            if (dv.Type == DataType.Table)
            {
                Table condition = dv.Table;

                dv = condition.Get("type");
                if (dv.Type == DataType.Number)
                {
                    spell.Condition = (ConditionType)dv.Number;
                }

                dv = condition.Get("startDamage");
                if (dv.Type == DataType.Number)
                {
                    spell.StartDamage = (int)dv.Number;
                }

                dv = condition.Get("minDamage");
                if (dv.Type == DataType.Number)
                {
                    spell.MinDamage = (int)dv.Number;
                }

                dv = condition.Get("maxDamage");
                if (dv.Type == DataType.Number)
                {
                    spell.MaxDamage = (int)dv.Number;
                }

                dv = condition.Get("duration");
                if (dv.Type == DataType.Number)
                {
                    spell.Duration = (int)dv.Number;
                }

                dv = condition.Get("interval");
                if (dv.Type == DataType.Number)
                {
                    spell.Interval = (int)dv.Number;
                }
            }

            dv = t.Get("script");
            if (dv.Type == DataType.String)
            {
                spell.DefinitionStyle = SpellDefinition.TfsLuaScript;
                spell.Name = dv.String;

                dv = t.Get("target");
                if (dv.Type == DataType.Boolean)
                {
                    spell.OnTarget = dv.Boolean;
                }

                dv = t.Get("direction");
                if (dv.Type == DataType.Boolean)
                {
                    spell.IsDirectional = dv.Boolean;
                }
            }

            return spell;
        }

        private LootItem ParseLootItem(Table t)
        {
            DynValue dv;
            LootItem lootItem = new LootItem();

            dv = t.Get("id");
            if (dv.Type == DataType.Number)
            {
                lootItem.Id = (ushort)dv.Number;
            }
            else if (dv.Type == DataType.String)
            {
                lootItem.Name = dv.String;
            }

            dv = t.Get("chance");
            if (dv.Type == DataType.Number)
            {
                lootItem.Chance = (decimal)dv.Number / MAX_LOOTCHANCE;
            }

            dv = t.Get("maxCount");
            if (dv.Type == DataType.Number)
            {
                lootItem.Count = (int)dv.Number;
            }

            dv = t.Get("aid");
            if (dv.Type == DataType.Number)
            {
                lootItem.ActionId = (int)dv.Number;
            }

            dv = t.Get("actionId");
            if (dv.Type == DataType.Number)
            {
                lootItem.ActionId = (int)dv.Number;
            }

            dv = t.Get("subType");
            if (dv.Type == DataType.Number)
            {
                lootItem.SubType = (int)dv.Number;
            }

            dv = t.Get("charges");
            if (dv.Type == DataType.Number)
            {
                lootItem.SubType = (int)dv.Number;
            }

            dv = t.Get("text");
            if (dv.Type == DataType.String)
            {
                lootItem.Text = dv.String;
            }

            dv = t.Get("description");
            if (dv.Type == DataType.String)
            {
                lootItem.Text = dv.String;
            }

            return lootItem;
        }

        private Voice GetVoice(Table t)
        {
            DynValue dv;
            Voice voice = null;

            dv = t.Get("text");
            if (dv.Type == DataType.String)
            {
                voice = new Voice(dv.String);
            }
            dv = t.Get("yell");
            if (dv.Type == DataType.String)
            {
                voice.SoundLevel = dv.Boolean ? SoundLevel.Yell : SoundLevel.Say;
            }

            return voice;
        }

        private Summon GetSummon(Table t)
        {
            DynValue dv;
            Summon summon = new Summon();

            dv = t.Get("name");
            if (dv.Type == DataType.String)
            {
                summon.Name = dv.String;
            }
            dv = t.Get("interval");
            if (dv.Type == DataType.Number)
            {
                summon.Interval = (int)dv.Number;
            }
            dv = t.Get("chance");
            if (dv.Type == DataType.Number)
            {
                summon.Chance = dv.Number;
            }
            dv = t.Get("max");
            if (dv.Type == DataType.Number)
            {
                summon.Max = (int)dv.Number;
            }

            return summon;
        }

        private double TfsRevScriptSysToGenericElementalPercent(double percent)
        {
            return (1 - (percent / 100.0));
        }

        private Blood TfsRevScriptSysRaceToGenericBlood(DynValue blood)
        {
            Blood race = Blood.blood; //default

            if (blood.Type == DataType.String)
            {
                switch (blood.String)
                {
                    case "venom":
                        race = Blood.venom;
                        break;

                    case "blood":
                        race = Blood.blood;
                        break;

                    case "undead":
                        race = Blood.undead;
                        break;

                    case "fire":
                        race = Blood.fire;
                        break;

                    case "energy":
                        race = Blood.venom;
                        break;
                }
            }
            else if (blood.Type == DataType.Number)
            {
                switch (blood.Number)
                {
                    case 1:
                        race = Blood.venom;
                        break;

                    case 2:
                        race = Blood.blood;
                        break;

                    case 3:
                        race = Blood.undead;
                        break;

                    case 4:
                        race = Blood.fire;
                        break;

                    case 5:
                        race = Blood.venom;
                        break;
                }
            }

            return race;
        }

    }
}
