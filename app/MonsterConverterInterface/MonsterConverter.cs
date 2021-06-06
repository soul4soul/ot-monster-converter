using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MonsterConverterInterface.MonsterTypes;

namespace MonsterConverterInterface
{
    public abstract class MonsterConverter : IMonsterConverter
    {
        public abstract string ConverterName { get; }

        public abstract string FileExt { get; }

        public virtual FileSource FileSource { get => FileSource.LocalFiles; }

        public abstract bool IsReadSupported { get; }

        public abstract bool IsWriteSupported { get; }

        public virtual string[] GetFilesForConversion(string directory)
        {
            string searchPattern = "*." + FileExt;
            return Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
        }

        public abstract ConvertResult ReadMonster(string filename, out Monster monster);

        public abstract ConvertResult WriteMonster(string directory, ref Monster monster);
    }
}
