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
                spawntype = monster.IgnoreSpawnBlock ? "Unblockable" : "",
                isboss = monster.IsBoss ? "yes" : "no",
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
                abilities = GenericToTibiaWikiAbilities(monster, ref result),
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

            string name = loot.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = loot.Id.ToString();
            }

            return $"{{{{Loot Item|{countPart}|{name}|{chancePart}}}}}";
        }

        private static string GenericToTibiaWikiAbilities(Monster mon, ref ConvertResultEventArgs result)
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
                    melee.damage = damage;
                    if (WikiToElements.Reverse.ContainsKey(s.DamageElement))
                    {
                        melee.element = WikiToElements.Reverse[s.DamageElement];
                    }
                    melee.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(melee));
                }
                else if (s.Name == "speed")
                {
                    if (s.SpellCategory == SpellCategory.Defensive)
                    {
                        HasteTemplate haste = new HasteTemplate();
                        if (wikiName == s.Name)
                        {
                            wikiName = null; // Let TibiaWiki handle it
                        }
                        haste.name = wikiName;
                        abilities.Add(TemplateParser.Serialize(haste));
                    }
                    else
                    {
                        // Use ability or debuff template
                    }
                }
                else if (s.Name == "firefield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates [[Fire Field|Fire]]";
                    }
                    ability.name = wikiName;
                    ability.element = WikiToElements.Reverse[CombatDamage.Fire];
                    ability.scene = GenericSpellToScene(s, mon.Name, "Fire");
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "energyfield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates [[Energy Field|Energy_Field_(Field)]]";
                    }
                    ability.name = wikiName;
                    ability.element = WikiToElements.Reverse[CombatDamage.Energy];
                    ability.scene = GenericSpellToScene(s, mon.Name, "Energy_Field_(Field)");
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "poisonfield")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (wikiName == s.Name)
                    {
                        wikiName = "Creates [[Poison Field|Poison_Gas]]";
                    }
                    ability.name = wikiName;
                    ability.element = WikiToElements.Reverse[CombatDamage.Earth];
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
                        if (WikiToElements.Reverse.ContainsKey(s.DamageElement))
                        {
                            ability.element = WikiToElements.Reverse[s.DamageElement];
                        }
                        ability.scene = GenericSpellToScene(s, mon.Name);
                        abilities.Add(TemplateParser.Serialize(ability));
                    }
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
                    if (s.ItemId != null)
                    {
                        result.AppendMessage($"Can't convert ability {s}, outfit with item");
                        result.IncreaseError(ConvertError.Warning);
                        continue;
                    }

                    AbilityTemplate ability = new AbilityTemplate();
                    if (s.Name == wikiName)
                    {
                        wikiName = "Creature Illusion";
                    }
                    ability.name = wikiName;

                    if (s.SpellCategory == SpellCategory.Offensive)
                    {
                        ability.damage = $"Turns you into [[{s.MonsterName}]]";
                    }
                    else
                    {
                        ability.damage = $"Shapeshifts into [[{s.MonsterName}]]";
                    }
                    ability.element = "shapeshifting";
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
                }
                else if (s.Name == "drunk")
                {
                    AbilityTemplate ability = new AbilityTemplate();
                    if (s.Name == wikiName)
                    {
                        wikiName = "Causes [[Drunkenness]]";
                    }
                    ability.name = wikiName;
                    ability.element = "drunk";
                    ability.scene = GenericSpellToScene(s, mon.Name);
                    abilities.Add(TemplateParser.Serialize(ability));
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

            if (missileIds.Count == 0)
            {
                GetMissileIds();
            }

            if (effectIds.Count == 0)
            {
                GetEffectIds();
            }

            if (missileIds.Reverse.ContainsKey(spell.ShootEffect))
            {
                scene.missile = missileIds.Reverse[spell.ShootEffect];
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
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 3))
            {
                scene.spell = "1sqmballtarget";
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 4))
            {
                scene.spell = "2sqmballtarget";
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 5))
            {
                scene.spell = "3sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 6))
            {
                scene.spell = "4sqmballtarget";
                scene.MissileDistance = "4/4";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 7))
            {
                scene.spell = "5sqmballtarget";
                scene.MissileDistance = "5/5";
                scene.MissileDirection = "south-east";
                scene.EffectOnTarget = scene.effect;
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 1))
            {
                scene.spell = "singleeffect";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 4))
            {
                scene.spell = "2sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 5))
            {
                scene.spell = "3sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 6))
            {
                scene.spell = "4sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 7))
            {
                scene.spell = "5sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 8))
            {
                scene.spell = "6sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 1))
            {
                scene.spell = "1sqmstrike";
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 2))
            {
                scene.spell = "2sqmstrike";
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 3))
            {
                scene.spell = "3sqmstrike";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 1) && (spell.Range == 5))
            {
                scene.spell = "5sqmstrike";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 5))
            {
                scene.spell = "great_explosion";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 3))
            {
                scene.spell = "3x3spell";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 2))
            {
                scene.spell = "plusspell";
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

            if (hasSceneData)
            {
                return TemplateParser.Serialize(scene);
            }
            return null;
        }
    }
}
