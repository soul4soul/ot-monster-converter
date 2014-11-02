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

    class PyOTMonster : GenericMonster
    {
        // Constructor
        public PyOTMonster()
            : base()
        {
        }

                // Functions
        public override void ReadMonster(string filename)
        {
        }

        public override void WriteMonster(string filename)
        {
            string lowerName = this.name.ToLower();

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string titleName = textInfo.ToTitleCase(lowerName); //War And Peace

            string[] lines =
            {
                string.Format("{0} = genMonster(\"{1}\", ({2}, {3}), \"{4}\")", lowerName, titleName, this.corpseId, this.lookId, this.description),
                string.Format("{0}.setHealth({1})", lowerName, this.health),
                //string.Format("{0}.bloodType({1})", lowerName, this. ),
                //string.Format("{0}.setDefense({1})", lowerName, this. ),
                string.Format("{0}.setExperience({1})", lowerName, this.experience),
                string.Format("{0}.setSpeed({1})", lowerName, this.speed),
                //string.Format("{0}.setBehavior({1})", lowerName, this. ),
                //string.Format("{0}.walkAround({1})", lowerName, this. ),
                //string.Format("{0}.summon({1})", lowerName, this.health),
                //string.Format("{0}.maxSummons({1})", lowerName, this.health),
                //string.Format("{0}.voices({1})", lowerName, this.health),
                //string.Format("{0}.loot({1})", lowerName, this.health),
            };
            File.WriteAllLines(@filename, lines);
        }
    }
}
