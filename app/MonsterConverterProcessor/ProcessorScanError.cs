using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterProcessor
{
    public enum ProcessorScanError
    {
        Success,
        InvalidInputDirectory,
        NoMonstersFound,
        CouldNotCreateDirectory,
        DirectoriesMatch,
        OtbReadFailed,
    }
}
