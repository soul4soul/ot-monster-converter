using MonsterConverterInterface;
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
            InfoboxCreatureTemplate creature = new InfoboxCreatureTemplate()
            {
                Name = monster.RegisteredName,
                Hp = monster.Health.ToString(),
                ActualName = monster.Name,
                Exp = monster.Experience.ToString(),
                Armor = monster.TotalArmor.ToString(),
                Summon = monster.SummonCost > 0 ? monster.SummonCost.ToString() : "--",
                Convince = monster.ConvinceCost > 0 ? monster.ConvinceCost.ToString() : "--",
                Illusionable = monster.IsIllusionable ? "yes" : "no",
                PrimaryType = monster.HideHealth ? "trap" : "",
                BestiaryClass = monster.Bestiary.Class,
                BestiaryLevel = GenericToTibiaWikiBestiaryLevel(monster),
                Occurrence = GenericToTibiaWikiOccurennce(monster),
                SpawnType = monster.IgnoreSpawnBlock ? "Unblockable" : "",
                IsBoss = monster.IsBoss ? "yes" : "no",
                Pushable = monster.IsPushable ? "yes" : "no",
                PushObjects = monster.PushItems ? "yes" : "no",
                WalksAround = GenericToTibiaWikiWalkAround(monster),
                WalksThrough = GenericToTibiaWikiWalkThrough(monster),
                ParaImmune = monster.IgnoreParalyze ? "yes" : "no",
                SenseInvis = monster.IgnoreInvisible ? "yes" : "no",
                PhysicalDmgMod = $"{monster.PhysicalDmgMod * 100:0}%",
                EarthDmgMod = $"{monster.EarthDmgMod * 100:0}%",
                FireDmgMod = $"{monster.FireDmgMod * 100:0}%",
                DeathDmgMod = $"{monster.DeathDmgMod * 100:0}%",
                EnergyDmgMod = $"{monster.EnergyDmgMod * 100:0}%",
                HolyDmgMod = $"{monster.HolyDmgMod * 100:0}%",
                IceDmgMod = $"{monster.IceDmgMod * 100:0}%",
                HealMod = $"{monster.HealingMod * 100:0}%",
                LifeDrainDmgMod = $"{monster.LifeDrainDmgMod * 100:0}%",
                DrownDmgMod = $"{monster.DrownDmgMod * 100:0}%",
                Sounds = GenericToTibiaWikiVoice(monster),
                RunsAt = monster.RunOnHealth.ToString(),
                Speed = monster.Speed.ToString(),
                Abilities = GenericToTibiaWikiAbilities(monster),
                Location = monster.Bestiary.Location,
                Loot = GenericToTibiaWikiLootList(monster)
            };
            string output = TemplateParser.Serialize(creature, false);
            string fileName = Path.Combine(directory, monster.FileName);
            File.WriteAllText(fileName, output);

            return new ConvertResultEventArgs(fileName, ConvertError.Warning, "Summons, abilities, and description information is not written.");
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
            sound.Sounds = monster.Voices.Select(v => v.Sound).ToArray();
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
            lootTable.Loot = flatLoot.OrderByDescending(kv => kv.Value.Chance).Select(kv => GenericToTibiaWikiLoot(kv.Value)).ToArray();
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

        private static string GenericToTibiaWikiAbilities(Monster mon)
        {
            IList<string> abilities = new List<string>();
            foreach (var s in mon.Summons)
            {
                SummonTemplate summon = new SummonTemplate()
                {
                    Creature = s.Name,
                    Amount = Math.Max(1, s.Max).ToString()
                };
                abilities.Add(TemplateParser.Serialize(summon));
            }

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
                        melee.Name = wikiName;
                    }
                    melee.Damage = damage;
                    if (WikiToElements.Reverse.ContainsKey(s.DamageElement))
                    {
                        melee.Element = WikiToElements.Reverse[s.DamageElement];
                    }

                    if (GenericSpellToScene(s, mon.Name, out string scene))
                    {
                        melee.Scene = scene;
                    }

                    abilities.Add(TemplateParser.Serialize(melee));
                }
                else if (s.Name == "haste")
                {
                    HasteTemplate haste = new HasteTemplate();
                    haste.Name = wikiName;
                    abilities.Add(TemplateParser.Serialize(haste));
                }
                else if (s.Name == "combat")
                {
                    if ((s.DamageElement == CombatDamage.Healing) && (s.SpellCategory == SpellCategory.Defensive))
                    {
                        HealingTemplate healing = new HealingTemplate();
                        healing.Name = wikiName;
                        healing.Damage = damage;

                        if (GenericSpellToScene(s, mon.Name, out string scene))
                        {
                            healing.Scene = scene;
                        }

                        abilities.Add(TemplateParser.Serialize(healing));
                    }
                    else
                    {
                        AbilityTemplate ability = new AbilityTemplate();
                        ability.Name = wikiName;
                        ability.Damage = damage;
                        if (WikiToElements.Reverse.ContainsKey(s.DamageElement))
                        {
                            ability.Element = WikiToElements.Reverse[s.DamageElement];
                        }

                        if (GenericSpellToScene(s, mon.Name, out string scene))
                        {
                            ability.Scene = scene;
                        }

                        abilities.Add(TemplateParser.Serialize(ability));
                    }
                }
            }

            AbilityListTemplate abilityList = new AbilityListTemplate();
            abilityList.Ability = abilities.ToArray();
            return TemplateParser.Serialize(abilityList, false);
        }

        private static bool GenericSpellToScene(Spell spell, string caster, out string sceneSerailized)
        {
            bool hasSceneData = false;
            SceneTemplate scene = new SceneTemplate();
            scene.Caster = caster;

            if (missileIds.Reverse.ContainsKey(spell.ShootEffect))
            {
                scene.Missile = missileIds.Reverse[spell.ShootEffect];
            }
            if (effectIds.Reverse.ContainsKey(spell.AreaEffect))
            {
                scene.Effect = effectIds.Reverse[spell.AreaEffect];
            }

            // Sort from most strict requirements to least strict
            if ((spell.IsDirectional == true) && (spell.Length == 1) && (spell.Spread == 3))
            {
                scene.Spell = "front_sweep";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 8) && (spell.Spread == 1))
            {
                scene.Spell = "8sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 7) && (spell.Spread == 1))
            {
                scene.Spell = "7sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 6) && (spell.Spread == 1))
            {
                scene.Spell = "6sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 1))
            {
                scene.Spell = "5sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 4) && (spell.Spread == 1))
            {
                scene.Spell = "4sqmbeam";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 8) && (spell.Spread == 5))
            {
                scene.Spell = "8sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 10) && (spell.Spread == 5))
            {
                scene.Spell = "10sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 3) && (spell.Spread == 3))
            {
                scene.Spell = "3sqmwave";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 3))
            {
                scene.Spell = "5sqmwavenarrow";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.IsDirectional == true) && (spell.Length == 5) && (spell.Spread == 5))
            {
                scene.Spell = "5sqmwavewide";
                scene.LookDirection = "east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 2))
            {
                scene.Spell = "plusspell";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 3))
            {
                scene.Spell = "1sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 4))
            {
                scene.Spell = "2sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 5))
            {
                scene.Spell = "3sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 6))
            {
                scene.Spell = "4sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Radius == 7))
            {
                scene.Spell = "5sqmballtarget";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 1))
            {
                scene.Spell = "singleeffect";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 4))
            {
                scene.Spell = "2sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 5))
            {
                scene.Spell = "3sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 6))
            {
                scene.Spell = "4sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 7))
            {
                scene.Spell = "5sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == false) && (spell.Radius == 8))
            {
                scene.Spell = "6sqmballself";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Range == 1))
            {
                scene.Spell = "1sqmstrike";
                scene.MissileDistance = "1/1";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Range == 2))
            {
                scene.Spell = "2sqmstrike";
                scene.MissileDistance = "2/2";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Range == 3))
            {
                scene.Spell = "3sqmstrike";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if ((spell.OnTarget == true) && (spell.Range == 5))
            {
                scene.Spell = "5sqmstrike";
                scene.MissileDistance = "3/3";
                scene.MissileDirection = "south-east";
                hasSceneData = true;
            }
            else if (spell.Radius == 5)
            {
                scene.Spell = "great_explosion";
                hasSceneData = true;
            }
            else if (spell.Radius == 3)
            {
                scene.Spell = "3x3spell";
                hasSceneData = true;
            }
            else if (spell.Radius == 2)
            {
                scene.Spell = "plusspell";
                hasSceneData = true;
            }

            sceneSerailized = TemplateParser.Serialize(scene);
            return hasSceneData;
        }
    }
}
