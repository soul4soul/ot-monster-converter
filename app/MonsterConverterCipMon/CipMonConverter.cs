using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Text;

namespace MonsterConverterCipMon
{
    [Export(typeof(IMonsterConverter))]
    public class CipMonConverter : MonsterConverter
    {
        public override string ConverterName { get => "CipMon"; }

        public override string FileExt { get => "mon"; }

        public override bool IsReadSupported { get => true; }

        public override bool IsWriteSupported { get => false; }

        // Functions
        public override ConvertResultEventArgs ReadMonster(string filename, out Monster monster)
        {
            throw new NotImplementedException();
        }

        public override ConvertResultEventArgs WriteMonster(string directory, ref Monster monster)
        {
            throw new NotImplementedException();
        }
    }
}
