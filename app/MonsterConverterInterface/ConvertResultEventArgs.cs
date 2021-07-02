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

    public class ConvertResultEventArgs : EventArgs
    {
        public string File { get; set; }
        public ConvertError Code { get; set; }
        public string Message { get; set; }

        public ConvertResultEventArgs(string file)
            : this(file, ConvertError.Success)
        {
        }

        public ConvertResultEventArgs(string file, ConvertError code, string message = null)
        {
            File = file;
            Code = code;
            Message = message;
        }

        public void IncreaseError(ConvertError newCode)
        {
            if (newCode > Code)
            {
                Code = newCode;
            }
        }

        public void AppendMessage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                if (Message == null)
                {
                    Message = msg;
                }
                else
                {
                    Message += $"{Environment.NewLine}{msg}";
                }
            }
        }
    }
}
