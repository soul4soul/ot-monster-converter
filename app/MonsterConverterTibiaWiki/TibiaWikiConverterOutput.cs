﻿using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    public partial class TibiaWikiConverter
    {
        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            // Unlike input we have no better place to init Ids
            GetItemIds();
            GetMissileIds();
            GetEffectIds();

            string fileName = Path.Combine(directory, monster.FileName);
            ConvertResultEventArgs result = new ConvertResultEventArgs(fileName);

            string article = monster.Description.ToLower().Replace(monster.Name.ToLower(), "").Trim();
            if (article != "a" || article != "an")
            {
                article = null;
            }

            InfoboxCreatureTemplate creature = new InfoboxCreatureTemplate()
            {
                name = monster.RegisteredName,
                article = article,
                hp = monster.Health.ToString(),
                actualname = monster.Name,
                exp = monster.Experience.ToString(),
                armor = monster.TotalArmor.ToString(),
                summon = monster.SummonCost > 0 ? monster.SummonCost.ToString() : "--",
                convince = monster.ConvinceCost > 0 ? monster.ConvinceCost.ToString() : "--",
                illusionable = monster.IsIllusionable ? "yes" : "no",
                primarytype = monster.HideHealth ? "trap" : "",
                bestiaryclass = monster.Bestiary.Class,
                bestiarylevel = GenericToTibiaWikiBestiaryLevel(monster),
                occurrence = GenericToTibiaWikiOccurennce(monster),
                attacktype = monster.TargetDistance > 1 ? "Distance" : "Melee",
                spawntype = monster.IgnoreSpawnBlock ? "Unblockable" : "",
                isboss = monster.IsBoss ? "yes" : "no",
                lightcolor = monster.LightColor.ToString(),
                lightradius = monster.LightLevel.ToString(),
                pushable = monster.IsPushable ? "yes" : "no",
                pushobjects = monster.PushItems ? "yes" : "no",
                walksaround = GenericToTibiaWikiWalkAround(monster),
                walksthrough = GenericToTibiaWikiWalkThrough(monster),
                paraimmune = monster.IgnoreParalyze ? "yes" : "no",
                senseinvis = monster.IgnoreInvisible ? "yes" : "no",
                physicaldmgmod = $"{monster.PhysicalDmgMod * 100:0}%",
                earthdmgmod = $"{monster.EarthDmgMod * 100:0}%",
                firedmgmod = $"{monster.FireDmgMod * 100:0}%",
                deathdmgmod = $"{monster.DeathDmgMod * 100:0}%",
                energydmgmod = $"{monster.EnergyDmgMod * 100:0}%",
                holydmgmod = $"{monster.HolyDmgMod * 100:0}%",
                icedmgmod = $"{monster.IceDmgMod * 100:0}%",
                healmod = $"{monster.HealingMod * 100:0}%",
                lifedraindmgmod = $"{monster.LifeDrainDmgMod * 100:0}%",
                drowndmgmod = $"{monster.DrownDmgMod * 100:0}%",
                sounds = GenericToTibiaWikiVoice(monster),
                runsat = monster.RunOnHealth.ToString(),
                speed = monster.Speed.ToString(),
                abilities = GenericToTibiaWikiAbilities(monster, result),
                location = monster.Bestiary.Location,
                loot = GenericToTibiaWikiLootList(monster)
            };
            string output = TemplateParser.Serialize(creature, false);
            File.WriteAllText(fileName, output);

            return result;
        }

        private string GenericToTibiaWikiOccurennce(Monster monster)
        {
            if (monster.Bestiary.OccuranceDiamondCount < 0)
                return "Common";
            else if (monster.Bestiary.OccuranceDiamondCount < 0)
                return "Uncommon";
            else if (monster.Bestiary.OccuranceDiamondCount < 0)
                return "Rare";
            else
                return "Very Rare";
        }

        private string GenericToTibiaWikiBestiaryLevel(Monster monster)
        {
            if (monster.Bestiary.DifficultlyStarCount < 0)
                return "Harmless";
            else if (monster.Bestiary.DifficultlyStarCount == 1)
                return "Trivial";
            else if (monster.Bestiary.DifficultlyStarCount == 2)
                return "Easy";
            else if (monster.Bestiary.DifficultlyStarCount == 3)
                return "Medium";
            else if (monster.Bestiary.DifficultlyStarCount == 4)
                return "Hard";
            else
                return "Challenging";
        }

        private static string GenericToTibiaWikiWalkAround(Monster monster)
        {
            string walks = "";
            if (monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private static string GenericToTibiaWikiWalkThrough(Monster monster)
        {
            string walks = "";
            if (!monster.AvoidFire)
            {
                walks += "Fire, ";
            }
            if (!monster.AvoidEnergy)
            {
                walks += "Energy, ";
            }
            if (!monster.AvoidPoison)
            {
                walks += "Poison, ";
            }
            if (string.IsNullOrWhiteSpace(walks))
            {
                return "None";
            }
            else
            {
                return walks.Substring(0, walks.Length - 2); // Chop off trailing ", "
            }
        }

        private static string GenericToTibiaWikiVoice(Monster monster)
        {
            SoundListTemplate sound = new SoundListTemplate();
            sound.sounds = monster.Voices.Select(v => v.Sound).ToArray();
            return TemplateParser.Serialize(sound, true);
        }

        private static void FlattenNestedLoot(IDictionary<string, LootItem> flatLoot, IList<LootItem> items)
        {
            foreach (var l in items)
            {
                if (flatLoot.ContainsKey(l.ComboIdentifier))
                {
                    flatLoot[l.ComboIdentifier].Count += l.Count;
                    flatLoot[l.ComboIdentifier].Chance = flatLoot[l.ComboIdentifier].Chance + l.Chance;
                }
                else
                {
                    flatLoot.Add(l.ComboIdentifier, l);
                }

                FlattenNestedLoot(flatLoot, l.NestedLoot);
            }
        }

        private static string GenericToTibiaWikiLootList(Monster monster)
        {
            // Flatten loot list, TibiaWiki doesn't supported nested loot information
            // Merged duplicate items, TibiaWiki only lists 1 entry per item type
            // <item id OR name, Loot>
            IDictionary<string, LootItem> flatLoot = new Dictionary<string, LootItem>();
            FlattenNestedLoot(flatLoot, monster.Items);

            // Sort by drop chance to follow TibiaWiki practice
            LootTableTemplate lootTable = new LootTableTemplate();
            lootTable.loot = flatLoot.OrderByDescending(kv => kv.Value.Chance).Select(kv => GenericToTibiaWikiLoot(kv.Value)).ToArray();
            return TemplateParser.Serialize(lootTable, false);
        }

        private static string GenericToTibiaWikiLoot(LootItem loot)
        {
            string countPart = "1";
            if (loot.Count > 1)
                countPart = $"0-{loot.Count}";
            string chancePart = "always";
            if (loot.Chance < 0.005M)
            {
                chancePart = "very rare";
            }
            else if ((loot.Chance >= 0.005M) && (loot.Chance < 0.01M))
            {
                chancePart = "rare";
            }
            else if ((loot.Chance >= 0.01M) && (loot.Chance < 0.05M))
            {
                chancePart = "semi-rare";
            }
            else if ((loot.Chance >= 0.05M) && (loot.Chance < 0.25M))
            {
                chancePart = "uncommon";
            }
            else if ((loot.Chance >= 0.25M) && (loot.Chance < 1.0M))
            {
                chancePart = "common";
            }

            // Wiki Name > item name in file > id
            string name;
            if (itemsById.ContainsKey(loot.Id))
            {
                name = itemsById[loot.Id].Name;
            }
            else if (!string.IsNullOrWhiteSpace(loot.Name))
            {
                name = loot.Name;
            }
            else
            {
                name = loot.Id.ToString();
            }

            return $"{{{{Loot Item|{countPart}|{name}|{chancePart}}}}}";
        }

        private static string GenericToTibiaWikiAbilities(Monster mon, ConvertResultEventArgs result)
        {
            IList<string> abilities = new List<string>();
            foreach (var s in mon.Attacks)
            {
                string wikiName = s.Name;
                if (!string.IsNullOrWhiteSpace(s.Description))
                {
                    wikiName = s.Description;
                }

                string damage = "-";
                if ((s.MinDamage != null) && (s.MaxDamage != null))
                {
                    var min = Math.Abs((int)s.MinDamage);
                    var max = Math.Abs((int)s.MaxDamage);
                    damage = $"{min:0}-{max:0}";
                }
                else if (s.MaxDamage != null)
                {
                    var max = Math.Abs((int)s.MaxDamage);
                    damage = $"{max:0}";
                }

                if (s.Name == "melee")
                {
                    MeleeTemplate melee = new MeleeTemplate();
                    if (wikiName.ToLower() != "melee")
                    {
                        melee.name = wikiName;
                    }
                    if ((s.AttackValue != null) && (s.Skill != null) && (s.AttackValue > 0) && (s.Skill > 0))
                    {
                        double attack = (double)s.AttackValue;
                        double skill = (double)s.Skill;
                        // Note: Formula taken from TFS Engine
                        var max = Math.Ceiling((skill * (attack * 0.05)) + (attack * 0.5));
                        melee.damage = $"0-{max}";
                    }
                    else
                    {
                        melee.damage = damage;
                    }
                    if (DamageTypeToWikiElement.ContainsKey(s.DamageElement))
                    {
                        melee.element = DamageTypeToWikiElement[s.DamageElement];
                    }
                    if ((s.Condition == ConditionType.Poison) && (s.StartDamage != null) && (s.StartDamage > 0))
                    {
                        melee.poison = CalculateStartOfLogDamageOverTime((int)s.StartDamage, 0).ToString();
                    }
                    else if (s.Condition != ConditionType.None)
                    {
                        result.AppendMessage($"Can't convert ability {s} unsupported condition {s.Condition}");
                        result.IncreaseError(ConvertError.Warning);
                        continue;
                    }
                    abilities.Add(TemplateParser.Serialize(melee));
                }
                else if (s.Name == "speed")
                {
                    if (s.SpellCategory == SpellCategory.Defensive)
                    {
                        HasteTemplate haste = new HasteTemplate();
                        if ((s.MinSpeedChange > STRONG_HASTE_SPEED) || (s.MaxSpeedChange > STRONG_HASTE_SPEED))
                        {
                            wikiName = "[[Strong Haste]]";
                        }
                        else
                        {
                            wikiName = null; // Let TibiaWiki handle it
                        }
                        haste.name = wikiName;
                        haste.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(haste));
                    }
                    else
                    {
                        AbilityTemplate ability = new AbilityTemplate();
                        if (wikiName == s.Name)
                        {
                            wikiName = "Paralyze";
                        }

                        int timeSecVal = (s.Duration == null) ? 1 : (int)s.Duration / 1000;
                        string timeStr = (timeSecVal > 1) ? $"{timeSecVal} seconds" : "1 second";
                        if (s.MinSpeedChange == s.MaxSpeedChange)
                        {
                            ability.damage = $"reduces {Math.Abs((double)s.MaxSpeedChange)} speed points for {timeStr}";
                        }
                        else
                        {
                            ability.damage = $"{Math.Abs((double)s.MinSpeedChange)}-{Math.Abs((double)s.MaxSpeedChange)} points for {timeStr}";
                        }

                        ability.name = wikiName;
                        ability.element = "paralyze";
                        ability.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(ability));
                    }
                }
                else if (s.Name == "firefield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates Fire Fields";
                    }
                    ability.name = wikiName;
                    ability.element = "fire field";
                    ability.scene = GenericSpellToScene(s, mon.Name, "Fire");
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "energyfield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates Energy Fields";
                    }
                    ability.name = wikiName;
                    ability.element = "energy field";
                    ability.scene = GenericSpellToScene(s, mon.Name, "Energy_Field_(Field)");
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "poisonfield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates Poison Fields";
                    }
                    ability.name = wikiName;
                    ability.element = "poison field";
                    ability.scene = GenericSpellToScene(s, mon.Name, "Poison_Gas");
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "combat")
                {
                    if ((s.DamageElement == CombatDamage.Healing) && (s.SpellCategory == SpellCategory.Defensive))
                    {
                        HealingTemplate healing = new HealingTemplate();
                        if (wikiName == s.Name)
                        {
                            wikiName = null; // Let wiki handle it
                        }
                        healing.name = wikiName;
                        healing.damage = damage;
                        healing.scene = GenericSpellToScene(s, mon.Name);

                        abilities.Add(TemplateParser.Serialize(healing));
                    }
                    else
                    {
                        AbilityTemplate ability = new AbilityTemplate();
                        ability.name = wikiName;
                        ability.damage = damage;
                        if (DamageTypeToWikiElement.ContainsKey(s.DamageElement))
                        {
                            ability.element = DamageTypeToWikiElement[s.DamageElement];
                        }
                        else
                        {
                            result.AppendMessage($"Can't convert ability {s} unknown combat type");
                            result.IncreaseError(ConvertError.Warning);
                            continue;
                        }
                        ability.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(ability));
                    }
                }
                else if (s.Name == "condition")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    ability.name = wikiName;
                    if (ConditionTypeToWikiElement.ContainsKey(s.Condition))
                    {
                        ability.element = ConditionTypeToWikiElement[s.Condition];
                        if ((s.Condition == ConditionType.Bleeding) || (s.Condition == ConditionType.Poison))
                        {
                            int totalDamage = Math.Abs(Math.Min(s.MinDamage ?? 0, s.MaxDamage ?? 0));
                            ability.damage = $"starting up to {CalculateStartOfLogDamageOverTime(totalDamage, s.StartDamage ?? 0)} hp/turn";
                        }
                        else
                        {
                            ability.damage = damage;
                        }
                    }
                    else
                    {
                        result.AppendMessage($"Can't convert ability {s} unknown condition type");
                        result.IncreaseError(ConvertError.Warning);
                        continue;
                    }
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "invisible")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    ability.name = wikiName;
                    ability.element = "invisible";

                    int timeSecVal = (s.Duration == null) ? 1 : (int)s.Duration / 1000;
                    string timeStr = (timeSecVal > 1) ? $"{timeSecVal} seconds" : "1 second";

                    string rate = "";
                    if (s.Chance >= 0.25)
                    {
                        rate = ", often";
                    }
                    else if (s.Chance < 0.05)
                    {
                        rate = ", rarely";
                    }

                    ability.damage = $"{timeStr}{rate}";
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "outfit")
                {
                    OutfitTemplate ability = new OutfitTemplate();
                    ability.victim = (s.SpellCategory == SpellCategory.Offensive) ? "yes" : "no";

                    if (s.ItemId != null)
                    {
                        int id = (int)s.ItemId;
                        if (!itemsById.ContainsKey(id))
                        {
                            result.AppendMessage($"Can't convert ability {s}, outfit with item id {id}");
                            result.IncreaseError(ConvertError.Warning);
                            continue;
                        }
                        ability.thing = itemsById[id].Name;
                    }
                    else
                    {
                        ability.thing = s.MonsterName;
                    }

                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "drunk")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (s.Name == wikiName)
                    {
                        wikiName = "Drunkenness";
                    }

                    int timeSecVal = (s.Duration == null) ? 1 : (int)s.Duration / 1000;
                    string timeStr = (timeSecVal > 1) ? $"{timeSecVal} seconds" : "1 second";

                    if (s.Drunkenness >= 5)
                    {
                        ability.damage = $"Heavy, {timeStr}";
                    }
                    else
                    {
                        ability.damage = timeStr;
                    }

                    ability.name = wikiName;
                    ability.element = "drunk";
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "effect")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (s.Name == wikiName)
                    {
                        wikiName = "Unknown Effect";
                    }
                    ability.name = wikiName;
                    ability.element = "?";
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "strength")
                {
                    int timeSecVal = (s.Duration == null) ? 1 : (int)s.Duration / 1000;
                    string timeStr = (timeSecVal > 1) ? $"{timeSecVal} seconds" : "1 second";
                    string value;
                    if (s.MinSkillChange == s.MaxSkillChange)
                    {
                        value = $"{Math.Abs((double)s.MinSkillChange)}%";
                    }
                    else
                    {
                        value = $"{Math.Abs((double)s.MinSkillChange)}-{Math.Abs((double)s.MaxSkillChange)}%";
                    }

                    if (s.SpellCategory == SpellCategory.Offensive)
                    {
                        Debuff2Template ability = new Debuff2Template();
                        if (s.Name == wikiName)
                        {
                            wikiName = "Debuff";
                        }
                        ability.name = wikiName;
                        ability.value = value;
                        ability.duration = timeSecVal.ToString();

                        if (s.Strengths.HasFlag(StrengthSkills.Melee))
                        {
                            ability.melee = "yes";
                        }
                        else if (s.Strengths.HasFlag(StrengthSkills.Fist) | s.Strengths.HasFlag(StrengthSkills.Axe) | s.Strengths.HasFlag(StrengthSkills.Club) | s.Strengths.HasFlag(StrengthSkills.Sword))
                        {
                            result.AppendMessage($"Can't convert abilitiy name {s} with range {value} because individual melee skill flags set");
                            result.IncreaseError(ConvertError.Warning);
                            continue;
                        }

                        if (s.Strengths.HasFlag(StrengthSkills.Distance))
                        {
                            ability.distance = "yes";
                        }
                        if (s.Strengths.HasFlag(StrengthSkills.Shielding))
                        {
                            ability.shielding = "yes";
                        }
                        if (s.Strengths.HasFlag(StrengthSkills.Magic))
                        {
                            ability.magic = "yes";
                        }

                        ability.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(ability));
                    }
                    else
                    {
                        AbilityTemplate ability = new AbilityTemplate();
                        if (s.Name == wikiName)
                        {
                            wikiName = "Buff";
                        }
                        ability.name = wikiName;

                        string effectedSkills = "";
                        if (s.Strengths.HasFlag(StrengthSkills.Melee))
                        {
                            effectedSkills += "melee";
                        }
                        else
                        {
                            if (s.Strengths.HasFlag(StrengthSkills.Fist))
                            {
                                effectedSkills += "fist";
                            }
                            if (s.Strengths.HasFlag(StrengthSkills.Axe))
                            {
                                effectedSkills += "axe";
                            }
                            if (s.Strengths.HasFlag(StrengthSkills.Club))
                            {
                                effectedSkills += "club";
                            }
                            if (s.Strengths.HasFlag(StrengthSkills.Sword))
                            {
                                effectedSkills += "sword";
                            }
                        }
                        if (s.Strengths.HasFlag(StrengthSkills.Shielding))
                        {
                            effectedSkills += "shielding";
                        }
                        if (s.Strengths.HasFlag(StrengthSkills.Magic))
                        {
                            effectedSkills += "magic";
                        }
                        if (s.Strengths.HasFlag(StrengthSkills.Distance))
                        {
                            effectedSkills += "distance";
                        }

                        ability.damage = $"Increases {effectedSkills} by {value} for {timeStr}";
                        ability.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(ability));
                    }
                }
                else
                {
                    result.AppendMessage($"Can't convert ability {s}");
                    result.IncreaseError(ConvertError.Warning);
                }
            }

            foreach (var s in mon.Summons)
            {
                SummonTemplate summon = new SummonTemplate()
                {
                    creature = s.Name,
                    amount = Math.Max(1, s.Max).ToString()
                };
                abilities.Add(TemplateParser.Serialize(summon));
            }

            AbilityListTemplate abilityList = new AbilityListTemplate();
            abilityList.ability = abilities.ToArray();
            return TemplateParser.Serialize(abilityList, false);
        }

        private static string GenericSpellToScene(Spell spell, string caster, string field = null)
        {
            bool hasSceneData = false;
            SceneTemplate scene = new SceneTemplate();
            scene.caster = caster;
            bool hasMissile = false;
            int sceneMissileDistance = 0;

            if (spell.OnTarget == true && missileIds.Reverse.ContainsKey(spell.ShootEffect))
            {
                scene.missile = missileIds.Reverse[spell.ShootEffect];
                scene.MissileDirection = "south-east";
                hasMissile = true;
            }
            if (effectIds.Reverse.ContainsKey(spell.AreaEffect))
            {
                scene.effect = effectIds.Reverse[spell.AreaEffect];
            }
            if (field != null)
            {
                scene.effect = field;
            }

            // Sort from most strict requirements to least strict
            if ((spell.IsDirectional == true) && (spell.Length == 1) && (spell.Spread == 1))
            {
                scene.spell = "front_sweep";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 10) && (spell.Spread == 0))
            {
                scene.spell = "10sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 9) && (spell.Spread == 0))
            {
                scene.spell = "9sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 8) && (spell.Spread == 0))
            {
                scene.spell = "8sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 7) && (spell.Spread == 0))
            {
                scene.spell = "7sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 6) && (spell.Spread == 0))
            {
                scene.spell = "6sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 0))
            {
                scene.spell = "5sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 4) && (spell.Spread == 0))
            {
                scene.spell = "4sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 3) && (spell.Spread == 0))
            {
                scene.spell = "3sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 2) && (spell.Spread == 0))
            {
                scene.spell = "2sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 1) && (spell.Spread == 0))
            {
                scene.spell = "1sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 8) && (spell.Spread == 3))
            {
                scene.spell = "8sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 10) && (spell.Spread == 4))
            {
                scene.spell = "10sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 3) && (spell.Spread == 2))
            {
                scene.spell = "3sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 3) && (spell.Spread == 1))
            {
                scene.spell = "3sqmwavewide";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 3))
            {
                scene.spell = "5sqmwavenarrow";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 2))
            {
                scene.spell = "5sqmwavewide";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 2))
            {
                scene.spell = "plusspelltarget";
                sceneMissileDistance = 2;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 3))
            {
                scene.spell = "1sqmballtarget";
                sceneMissileDistance = 2;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 4))
            {
                scene.spell = "2sqmballtarget";
                sceneMissileDistance = 2;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 5))
            {
                scene.spell = "3sqmballtarget";
                sceneMissileDistance = 3;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 6))
            {
                scene.spell = "4sqmballtarget";
                sceneMissileDistance = 4;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 7))
            {
                scene.spell = "5sqmballtarget";
                sceneMissileDistance = 5;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 1))
            {
                scene.spell = "buffspell";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 4))
            {
                scene.spell = "2sqmballself";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 5))
            {
                scene.spell = "3sqmball2self";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 6))
            {
                scene.spell = "4sqmballself";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 7))
            {
                scene.spell = "5sqmballself";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 8))
            {
                scene.spell = "6sqmballself";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 1))
            {
                scene.spell = "1sqmstrike";
                sceneMissileDistance = 1;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 2))
            {
                scene.spell = "2sqmstrike";
                sceneMissileDistance = 2;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 3))
            {
                scene.spell = "3sqmstrike";
                sceneMissileDistance = 3;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 4))
            {
                scene.spell = "4sqmstrike";
                sceneMissileDistance = 4;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 5))
            {
                scene.spell = "5sqmstrike";
                sceneMissileDistance = 5;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 6))
            {
                scene.spell = "6sqmstrike";
                sceneMissileDistance = 6;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 7))
            {
                scene.spell = "7sqmstrike";
                sceneMissileDistance = 7;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 5))
            {
                scene.spell = "great_explosion";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 3))
            {
                scene.spell = "3x3spell";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 2))
            {
                scene.spell = "plusspell";
                scene.EffectOnCaster = "yes";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Ring == 2))
            {
                scene.spell = "xspell";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Ring == 3))
            {
                scene.spell = "2sqmring";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Ring == 4))
            {
                scene.spell = "3sqmring";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Ring == 5))
            {
                scene.spell = "4sqmring";
                hasSceneData = true;
            }

            if (hasMissile)
            {
                scene.MissileDistance = $"{sceneMissileDistance}/{sceneMissileDistance}";
            }

            if (hasSceneData)
            {
                return TemplateParser.Serialize(scene);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Can't generate scene for spell {spell}");
                return null;
            }
        }

        // Note: Formula taken from TFS Engine
        private static int CalculateStartOfLogDamageOverTime(int amount, int start)
        {
            if (start == 0)
            {
                start = (int)Math.Max(1, Math.Ceiling(amount / 20.0));
            }
            return start;
        }
    }
}
