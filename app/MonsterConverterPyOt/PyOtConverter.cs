using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;

namespace MonsterConverterPyOt
{
    // https://bitbucket.org/vapus/pyot/src/0aa7c38f46814f502f375b84ac905e7f5ebef1a3/game/monster.py?at=default
    [Export(typeof(IMonsterConverter))]
    public class PyOtConverter : MonsterConverter
    {
        public override string ConverterName { get => "pyOT"; }

        public override string FileExt { get => "py"; }

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
                string.Format("{0}.bloodType({1})", lowerName, GenericToPyOTBlood(monster.Race)), //todo might change
                string.Format("{0}.defense(armor={1}, fire={2}, earth={3}, energy={4}, ice={5}, holy={6}, death={7}, physical={8}, drown={9}, lifedrain={10}, manadrain={11})",
                    lowerName, monster.TotalArmor, monster.FireDmgMod, monster.EarthDmgMod, monster.EnergyDmgMod, monster.IceDmgMod, monster.HolyDmgMod, monster.DeathDmgMod, monster.PhysicalDmgMod, monster.DrownDmgMod, monster.LifeDrainDmgMod, monster.ManaDrainDmgMod),
                string.Format("{0}.setExperience({1})", lowerName, monster.Experience),
                string.Format("{0}.setSpeed({1})", lowerName, monster.Speed),
                //string.Format("{0}.setTargetChance({1})", lowerName, monster.RetargetChance), //Currently unused in pyOT?, consider changing code in pyOT to be chance of 0 to 1 for a percent instead of 0 to 100
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

            return new ConvertResultEventArgs(fileName, ConvertError.Warning, "Format incomplete. abilities and other information has not been converted");
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
                    summons += $"{lowerName}.summon(\"{s.Name}\", {s.Chance * 100})\n";
                }
                summons += string.Format("{0}.maxSummons({1})", lowerName, monster.MaxSummons);
            }
            return summons;
        }

        private string GenericToPyOTLoot(string lowerName, ref Monster monster)
        {
            string loot = "";
            if (monster.Items.Count > 0)
            {
                foreach (var mi in monster.Items)
                {
                    string item;
                    if (int.TryParse(mi.Item, out int itemid))
                    {
                        item = itemid.ToString();
                    }
                    else
                    {
                        item = $"\"{mi.Item}\"";
                    }

                    decimal chance = mi.Chance * 100;

                    string newloot;
                    if (mi.Count > 1)
                    {
                        newloot = $"({item}, {mi.Count}, {chance})";
                    }
                    else
                    {
                        newloot = $"({item}, {chance})";
                    }

                    if (string.IsNullOrWhiteSpace(loot))
                    {
                        loot = newloot;
                    }
                    else
                    {
                        loot = $"{loot}, {newloot}";
                    }
                }
                loot = string.Format("{0}.loot( {1} )", lowerName, loot);
            }
            return loot;
        }
    }
}
