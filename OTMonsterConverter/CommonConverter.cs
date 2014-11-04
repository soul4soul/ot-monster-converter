using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    interface ICommonConverter
    {
        void ReadMonster(string filename, out ICustomMonster monster);

        void WriteMonster(string filename, ref ICustomMonster monster);
    }

    public abstract class CommonConverter : ICommonConverter
    {
        public abstract void ReadMonster(string filename, out ICustomMonster monster);

        public abstract void WriteMonster(string filename, ref ICustomMonster monster);
    }
}
