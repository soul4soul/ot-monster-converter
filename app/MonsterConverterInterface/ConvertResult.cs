using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface
{
    public enum ConvertCode
    {
        Error,
        Success,
        Warning
    }

    public record ConvertResult(string File, ConvertCode Code, string Message = null);
}
