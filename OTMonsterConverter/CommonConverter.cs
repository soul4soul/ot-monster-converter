using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    interface ICommonConverter
    {
        bool ReadMonster(string filename, out ICustomMonster monster);

        bool WriteMonster(string directory, ref ICustomMonster monster);
    }

    public abstract class CommonConverter : ICommonConverter
    {
        public abstract bool ReadMonster(string filename, out ICustomMonster monster);

        public abstract bool WriteMonster(string directory, ref ICustomMonster monster);
    }
}
