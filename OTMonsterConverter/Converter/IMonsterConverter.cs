using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.Converter
{
    public interface IMonsterConverter
    {
        string FileExtRegEx { get; }

        bool ReadMonster(string filename, out Monster monster);

        bool WriteMonster(string directory, ref Monster monster);
    }
}
