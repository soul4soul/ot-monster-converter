using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface
{
    public enum ConvertError
    {
        Error,
        Success,
        Warning
    }

    public record ConvertResult(string File, ConvertError Code, string Message = null);
}
