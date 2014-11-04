using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public interface ICommonConverter
    {
        void ReadMonster(string filename, out GenericMonster monster);

        void WriteMonster(string filename, ref GenericMonster monster);
    }

    public abstract class CommonConverter : ICommonConverter
    {
        public abstract void ReadMonster(string filename, out GenericMonster monster);

        public abstract void WriteMonster(string filename, ref GenericMonster monster);
    }
}
