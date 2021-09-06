using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MonsterConverterPyOt
{
    // https://bitbucket.org/vapus/pyot/src/0aa7c38f46814f502f375b84ac905e7f5ebef1a3/game/monster.py?at=default
    [Export(typeof(IMonsterConverter))]
    public class PyOtConverter : MonsterConverter
    {
        public override string ConverterName { get => "pyOT"; }

        public override string FileExt { get => "py"; }

        public override ItemIdType ItemIdType { get => ItemIdType.Server; }

        public override bool IsReadSupported { get => false; }

        public override bool IsWriteSupported { get => true; }

        // Functions
        public override ConvertResultEventArgs ReadMonster(string filename, out Monster monster)
        {
            throw new NotImplementedException();
        }

        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            string lowerName = monster.Name.ToLower(); // TODO Remove special chars spaces etc.. want a-z_ for characters... Can we just use a fixed variable name such as "monster"?

            string[] lines =
            {
                string.Format("{0} = genMonster(\"{1}\", ({2}, {3}), \"{4}\")", lowerName, monster.Name, monster.Look.CorpseId, monster.Look.LookId, monster.Description),
                string.Format("{0}.health({1})", lowerName, monster.Health),
                string.Format("{0}.bloodType({1})", lowerName, GenericToPyOTBlood(monster.Race)), // todo: might change (see issue #8)
                string.Format("{0}.defense(armor={1}, fire={2}, earth={3}, energy={4}, ice={5}, holy={6}, death={7}, physical={8}, drown={9}, lifedrain={10}, manadrain={11})",
                    lowerName, monster.TotalArmor, monster.FireDmgMod, monster.EarthDmgMod, monster.EnergyDmgMod, monster.IceDmgMod, monster.HolyDmgMod, monster.DeathDmgMod, monster.PhysicalDmgMod, monster.DrownDmgMod, monster.LifeDrainDmgMod, monster.ManaDrainDmgMod),
                string.Format("{0}.experience({1})", lowerName, monster.Experience),
                string.Format("{0}.speed({1})", lowerName, monster.Speed),
                string.Format("{0}.targetChance({1})", lowerName, monster.RetargetChance * 100),  // todo: consider changing code in pyOT to be chance of 0 to 1 for a percent instead of 0 to 100 (see issue #8)
                string.Format("{0}.behavior(summonable={1}, hostile={2}, illusionable={3}, convinceable={4}, pushable={5}, pushItems={6}, pushCreatures={7}, targetDistance={8}, runOnHealth={9}, targetChange={10})",
                    lowerName, monster.SummonCost, monster.IsHostile, monster.IsIllusionable, monster.ConvinceCost, monster.IsPushable, monster.PushItems, monster.PushCreatures, monster.TargetDistance, monster.RunOnHealth, 1),
                string.Format("{0}.walkAround(energy={1}, fire={2}, poison={3})", lowerName, monster.AvoidEnergy, monster.AvoidFire, monster.AvoidPoison),
                string.Format("{0}.immunity(paralyze={1}, invisible={2}, drunk={3})",
                    lowerName, monster.IgnoreParalyze, monster.IgnoreInvisible, monster.IgnoreDrunk),
                GenericToPyOTSummons(lowerName, ref monster),
                GenericToPyOTVoice(lowerName, ref monster),
                GenericToPyOTLoot(lowerName, ref monster)
            };
            string fileName = Path.Combine(directory, monster.FileName + "." + FileExt);
            File.WriteAllLines(fileName, lines);

            return new ConvertResultEventArgs(fileName, ConvertError.Warning, "Format incomplete. Abilities, Look type, detailed loot information, and more has not been converted");
        }

        private string GenericToPyOTBlood(Blood race)
        {
            string bloodName = "blood";

            switch (race)
            {
                case Blood.venom:
                    bloodName = "slime";
                    break;

                case Blood.blood:
                    bloodName = "blood";
                    break;

                case Blood.undead:
                    bloodName = "undead";
                    break;

                case Blood.fire:
                    bloodName = "fire";
                    break;

                case Blood.energy:
                    bloodName = "energy";
                    break;
            }

            return bloodName;
        }

        private string GenericToPyOTVoice(string lowerName, ref Monster monster)
        {
            string voice = "";
            if (monster.Voices.Count > 0)
            {
                foreach (var v in monster.Voices)
                {
                    if (string.IsNullOrWhiteSpace(voice))
                    {
                        voice = $"\"{v.Sound}\"";
                    }
                    else
                    {
                        voice = $"{voice}, \"{v.Sound}\"";
                    }
                }
                voice = string.Format("{0}.voices({1})", lowerName, voice);
            }
            return voice;
        }

        private string GenericToPyOTSummons(string lowerName, ref Monster monster)
        {
            string summons = "";
            if (monster.Summons.Count > 0)
            {
                foreach (var s in monster.Summons)
                {
                    summons += $"{lowerName}.summon(\"{s.Name}\", {s.Chance * 100}){Environment.NewLine}";
                }
                summons += string.Format("{0}.maxSummons({1})", lowerName, monster.MaxSummons);
            }
            return summons;
        }

        /*
         * Loot formats
         * (item [name or id], chance)
         * (item [name or id], chance, count)
         * (item [name or id], chance, minCount, maxCount) - this option looks buggy in the source code, source code references index 5 in some places too
         * 
         * No nesting is supported, There is a config option to auto gen bags when corpse isn't large enough, we need to flatten the loot
         */
        private string GenericToPyOTLoot(string lowerName, ref Monster monster)
        {
            IList<LootItem> flatListLoot = new List<LootItem>();
            FlattenNestedLoot(flatListLoot, monster.Items);
            string loot = "";
            foreach (var lootItem in flatListLoot.OrderByDescending(l => l.Chance))
            {
                string newloot = LootItemToPyOtString(lootItem);

                if (string.IsNullOrWhiteSpace(loot))
                {
                    loot = newloot;
                }
                else
                {
                    loot = $"{loot}, {newloot}";
                }
            }
            loot = $"{lowerName}.loot( {loot} )";
            return loot;
        }

        private static void FlattenNestedLoot(IList<LootItem> flatList, IList<LootItem> nestedList)
        {
            foreach (var l in nestedList)
            {
                flatList.Add(l);
                FlattenNestedLoot(flatList, l.NestedLoot);
            }
        }

        private static string LootItemToPyOtString(LootItem lootItem)
        {
            string item = $"\"{lootItem.Name}\"";
            if (lootItem.Id > 0)
            {
                item = lootItem.Id.ToString();
            }

            decimal chance = lootItem.Chance * 100;

            // should return warning SubType, ActionId, and Text aren't supported

            string newloot;
            if (lootItem.Count > 1)
            {
                newloot = $"({item}, {lootItem.Count}, {chance})";
            }
            else
            {
                newloot = $"({item}, {chance})";
            }

            return newloot;
        }
    }
}
