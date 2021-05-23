using MonsterConverterInterface.MonsterTypes;
using System;

namespace MonsterConverterInterface
{
    public enum FileSource
    {
        LocalFiles,
        Web
    }

    // name, isreadsupported, and iswritesupported can all be metadataattributes should that be valuable localfile source too
    // none of those fields deal with function they all are for information only
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
