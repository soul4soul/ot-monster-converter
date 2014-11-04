using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    interface ICommonConverter
    {
        void ReadMonster(string filename, out IGenericMonster monster);

        void WriteMonster(string filename, ref IGenericMonster monster);
    }

    public abstract class CommonConverter : ICommonConverter
    {
        public abstract void ReadMonster(string filename, out IGenericMonster monster);

        public abstract void WriteMonster(string filename, ref IGenericMonster monster);
    }
}
