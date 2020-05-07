using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OTMonsterConverter.Converter
{
    // https://bitbucket.org/vapus/pyot/src/0aa7c38f46814f502f375b84ac905e7f5ebef1a3/game/monster.py?at=default
    public class PyOtConverter : IMonsterConverter
    {
        public string FileExtRegEx { get => "*.py"; }

        // Functions
        public bool ReadMonster(string filename, out Monster monster)
        {
            monster = new Monster();
            return false; // Not Implemented
        }

        public bool WriteMonster(string directory, ref Monster monster)
        {
            string lowerName = monster.Name.ToLower(); // TODO Remove special chars spaces etc.. want a-z_ for characters... Can we just use a fixed variable name such as "monster"?

            string[] lines =
            {
                string.Format("{0} = genMonster(\"{1}\", ({2}, {3}), \"{4}\")", lowerName, monster.Name, monster.CorpseId, monster.OutfitIdLookType, monster.Description),
                string.Format("{0}.health({1})", lowerName, monster.Health),
                string.Format("{0}.bloodType({1})", lowerName, GenericToPyOTBlood(monster.Race)), //todo might change
                string.Format("{0}.defense(armor={1}, fire={2}, earth={3}, energy={4}, ice={5}, holy={6}, death={7}, physical={8}, drown={9}, lifedrain={10}, manadrain={11})",
                    lowerName, monster.TotalArmor, monster.Fire, monster.Earth, monster.Energy, monster.Ice, monster.Holy, monster.Death, monster.Physical, monster.Drown, monster.LifeDrain, monster.ManaDrain),
                string.Format("{0}.setExperience({1})", lowerName, monster.Experience),
                string.Format("{0}.setSpeed({1})", lowerName, monster.Speed),
                //string.Format("{0}.setTargetChance({1})", lowerName, monster.RetargetChance), //Currently unused in pyOT?, consider changing code in pyOT to be chance of 0 to 1 for a percent instead of 0 to 100
                string.Format("{0}.behavior(summonable={1}, hostile={2}, illusionable={3}, convinceable={4}, pushable={5}, pushItems={6}, pushCreatures={7}, targetDistance={8}, runOnHealth={9}, targetChange={10})",
                    lowerName, monster.SummonCost, monster.Hostile, monster.Illusionable, monster.ConvinceCost, monster.Pushable, monster.PushItems, monster.PushCreatures, monster.TargetDistance, monster.RunOnHealth, 1),
                string.Format("{0}.walkAround(energy={1}, fire={2}, poison={3})", lowerName, monster.AvoidEnergy, monster.AvoidFire, monster.AvoidPoison),
                string.Format("{0}.immunity(paralyze={1}, invisible={2}, drunk={3})",
                    lowerName, monster.IgnoreParalyze, monster.IgnoreInvisible, monster.IgnoreDrunk),
                GenericToPyOTSummons(lowerName, ref monster),
                string.Format("{0}.voices({1})", lowerName, GenericToPyOTVoice(ref monster)),
                //string.Format("{0}.loot({1})", lowerName, monster.Items),
            };
            string fileName = Path.Combine(directory, monster.FileName + ".py");
            File.WriteAllLines(fileName, lines);

            return true;
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

        private string GenericToPyOTVoice(ref Monster monster)
        {
            string voice = "";
            foreach(var v in monster.Voices)
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
            }
            summons += string.Format("{0}.maxSummons({1})", lowerName, monster.MaxSummons);
            return summons;
        }
    }
}
