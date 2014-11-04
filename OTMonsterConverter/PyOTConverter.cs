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
                //string.Format("{0}.bloodType({1})", lowerName, monster. ),
                //string.Format("{0}.setDefense({1})", lowerName, monster. ),
                string.Format("{0}.setExperience({1})", lowerName, monster.Experience),
                string.Format("{0}.setSpeed({1})", lowerName, monster.Speed),
                //string.Format("{0}.setBehavior({1})", lowerName, monster. ),
                //string.Format("{0}.walkAround({1})", lowerName, monster. ),
                //string.Format("{0}.summon({1})", lowerName, monster.health),
                //string.Format("{0}.maxSummons({1})", lowerName, monster.health),
                //string.Format("{0}.voices({1})", lowerName, monster.health),
                //string.Format("{0}.loot({1})", lowerName, monster.health),
            };
            File.WriteAllLines(@filename, lines);
        }
    }
}
