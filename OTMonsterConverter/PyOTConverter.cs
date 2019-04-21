using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    //https://bitbucket.org/vapus/pyot/src/0aa7c38f46814f502f375b84ac905e7f5ebef1a3/game/monster.py?at=default

    class PyOTConverter : CommonConverter
    {
        // Constructor
        public PyOTConverter()
            : base()
        {
        }

        // Functions
        public override bool ReadMonster(string filename, out ICustomMonster monster)
        {
            monster = new CustomMonster();
            return false; // Not Implemented
        }

        public override bool WriteMonster(string directory, ref ICustomMonster monster)
        {
            string lowerName = monster.Name.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleName = textInfo.ToTitleCase(lowerName);
            string[] lines =
            {
                string.Format("{0} = genMonster(\"{1}\", ({2}, {3}), \"{4}\")", lowerName, titleName, monster.CorpseId, monster.OutfitIdLookType, monster.Description),
                string.Format("{0}.health({1})", lowerName, monster.Health),
                string.Format("{0}.bloodType({1})", lowerName, generictoPyOTBlood(monster.Race)), //todo might change
                string.Format("{0}.defense(armor={1}, fire={2}, earth={3}, energy={4}, ice={5}, holy={6}, death={7}, physical={8}, drown={9}, lifedrain={10}, manadrain={11})",
                    lowerName, monster.TotalArmor, monster.Fire, monster.Earth, monster.Energy, monster.Ice, monster.Holy, monster.Death, monster.Physical, monster.Drown, monster.LifeDrain, monster.ManaDrain),
                string.Format("{0}.setExperience({1})", lowerName, monster.Experience),
                string.Format("{0}.setSpeed({1})", lowerName, monster.Speed),
                //string.Format("{0}.setTargetChance({1})", lowerName, monster.RetargetChance), //Currently unused in pyOT?, consider changing code in pyOT to be chance of 0 to 1 for a percent instead of 0 to 100
                string.Format("{0}.behavior(summonable={1}, hostile={2}, illusionable={3}, convinceable={4}, pushable={5}, pushItems={6}, pushCreatures={7}, targetDistance={8}, runOnHealth={9}, targetChange={10})",
                    lowerName, monster.SummonCost, monster.Hostile, monster.Illusionable, monster.Pushable, monster.PushItems, monster.PushCreatures, monster.TargetDistance, monster.RunOnHealth, 1),
                string.Format("{0}.walkAround(energy={1}, fire={2}, poison={3})", lowerName, monster.avoidEnergy, monster.avoidFire, monster.avoidPoison),
                string.Format("{0}.immunity(paralyze={1}, invisible={2}, drunk={4})",
                    lowerName, monster.IgnoreParalyze, monster.IgnoreInvisible, monster.IgnoreDrunk),
                string.Format("{0}.maxSummons({1})", lowerName, monster.MaxSummons),
                //string.Format("{0}.summon({1})", lowerName, monster.health),
                //string.Format("{0}.voices({1})", lowerName, monster.health),
                //string.Format("{0}.loot({1})", lowerName, monster.health),
            };
            string fileName = Path.Combine(directory, lowerName);
            File.WriteAllLines(fileName, lines);

            return true;
        }

        private string generictoPyOTBlood(Blood race)
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
    }
}
