using MonsterConverterInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterProcessor
{
    public sealed class FileProcessorEventArgs : EventArgs
    {
        public FileProcessorEventArgs(ConvertResultEventArgs source, ConvertResultEventArgs destination, double completed, double total)
        {
            Source = source;
            Destination = destination;
            Completed = completed;
            Total = total;
        }

        public ConvertResultEventArgs Source { get; }
        public ConvertResultEventArgs Destination { get; }
        public double Completed { get; }
        public double Total { get; }
    }
}
