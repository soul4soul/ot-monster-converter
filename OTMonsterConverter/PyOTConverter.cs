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
        public override void ReadMonster(string filename, out ICustomMonster monster)
        {
            monster = new CustomMonster();
        }

        public override void WriteMonster(string filename, ref ICustomMonster monster)
        {
            string lowerName = monster.Name.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleName = textInfo.ToTitleCase(lowerName);
            string[] lines =
            {
                string.Format("{0} = genMonster(\"{1}\", ({2}, {3}), \"{4}\")", lowerName, titleName, monster.CorpseId, monster.OutfitIdLookType, monster.Description),
                string.Format("{0}.setHealth({1})", lowerName, monster.Health),
                string.Format("{0}.bloodType({1})", lowerName, generictoPyOTBlood(monster.Race)),
                string.Format("{0}.setDefense(armor={1}, fire={2}, earth={3}, energy={4}, ice={5}, holy={6}, death={7}, physical={8}, drown={9})",
                    lowerName, monster.TotalArmor, monster.Fire, monster.Earth, monster.Energy, monster.Ice, monster.Holy, monster.Death, monster.Physical, monster.Drown),
                string.Format("{0}.setExperience({1})", lowerName, monster.Experience),
                string.Format("{0}.setSpeed({1})", lowerName, monster.Speed),
                //string.Format("{0}.setTargetChance({1})", lowerName, monster.retargetChance), //Currently unused in pyOT, consider changing code in pyOT to be chance of 0 to 1 for a percent instead of 0 to 100
                string.Format("{0}setBehavior(summonable={1}, hostile={2}, illusionable={3}, convinceable={4}, pushable={5}, pushItems={6}, pushCreatures={7}, targetDistance={8}, runOnHealth={9}, targetChange={10})",
                    lowerName, monster.SummonCost, boolToInt(monster.Hostile), boolToInt(monster.Illusionable), boolToInt(monster.Pushable), boolToInt(monster.PushItems), boolToInt(monster.PushCreatures), monster.TargetDistance, monster.RunOnHealth, 1),
                string.Format("{0}.walkAround(energy={1}, fire={2}, poison={3})", lowerName, monster.avoidEnergy, monster.avoidFire, monster.avoidPoison),
                string.Format("{0}.setImmunity(paralyze={1}, invisible={2}, lifedrain={3}, drunk={4})",
                    lowerName, boolToInt(monster.IgnoreParalyze), boolToInt(monster.IgnoreInvisible), 0, monster.IgnoreDrunk),
                //string.Format("{0}.maxSummons({1})", lowerName, monster.health),
                //string.Format("{0}.summon({1})", lowerName, monster.health),
                //string.Format("{0}.voices({1})", lowerName, monster.health),
                //string.Format("{0}.loot({1})", lowerName, monster.health),
            };
            File.WriteAllLines(@filename, lines);
        }

        private ushort boolToInt(bool value)
        {
            if (value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
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
