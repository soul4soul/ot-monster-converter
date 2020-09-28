using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter.Converter
{
    public enum FileSource
    {
        LocalFiles,
        Web
    }

    public interface IMonsterConverter
    {
        string ConverterName { get; }

        string FileExt { get; }

        FileSource FileSource { get; }

        bool IsReadSupported { get; }

        bool IsWriteSupported { get; }

        string[] GetFilesForConversion(string directory);

        bool ReadMonster(string filename, out Monster monster);

        bool WriteMonster(string directory, ref Monster monster);
    }
}
