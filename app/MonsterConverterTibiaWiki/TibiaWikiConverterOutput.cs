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
    }
}
