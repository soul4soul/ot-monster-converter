using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    interface IGenericMonster
    {
        void ReadMonster(string filename);
        void WriteMonster();
    }

    abstract class GenericMonster : IGenericMonster
    {
        public string name;
        public string description;
        public int health;
        public List<string> voices;

        public GenericMonster()
        {
            voices = new List<string>();
        }

        public abstract void ReadMonster(string filename);

        public abstract void WriteMonster();
    }
}
