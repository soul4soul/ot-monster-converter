using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public enum ScanError
    {
        Success,
        InvalidMonsterDirectory,
        InvalidMonsterFormat,
        NoMonstersFound,
        CouldNotCreateDirectory,
        DirectoriesMatch
    }

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

    public class MonsterFileProcessor
    {
        // Events
        public event EventHandler<FileProcessorEventArgs> OnMonsterConverted;

        // Functions
        public ScanError ConvertMonsterFiles(string monsterDirectory, IMonsterConverter inputConverter, string outputDirectory, IMonsterConverter outputConverter, bool mirroredFolderStructure = false)
        {
            if ((inputConverter.FileSource == FileSource.LocalFiles) && (!Directory.Exists(monsterDirectory)))
            {
                return ScanError.InvalidMonsterDirectory;
            }

            if (!Directory.Exists(outputDirectory))
            {
                try
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                catch (Exception)
                {
                    return ScanError.CouldNotCreateDirectory;
                }
            }

            if ((inputConverter.FileSource == FileSource.LocalFiles) &&
                (Path.GetFullPath(monsterDirectory) == Path.GetFullPath(outputDirectory)))
            {
                return ScanError.DirectoriesMatch;
            }

            string[] files  = inputConverter.GetFilesForConversion(monsterDirectory);
            if (inputConverter.FileSource == FileSource.Web)
            {
                // TibiaWiki provides a flat list
                mirroredFolderStructure = false;
            }
            if ((files != null) && (files.Length == 0))
            {
                return ScanError.NoMonstersFound;
            }

            string destination;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                destination = FindExactFileDestination(monsterDirectory, outputDirectory, file, mirroredFolderStructure);
                var result  = ProcessFile(file, inputConverter, outputConverter, destination);
                RaiseEvent(OnMonsterConverted, new FileProcessorEventArgs(result.Item1, result.Item2, i, files.Length));
            }

            return ScanError.Success;
        }

        private string FindExactFileDestination(string inputDirectory, string outputDirectory, string file, bool mirroredFolderStructure)
        {
            if (mirroredFolderStructure)
            {
                string nameOnly = Path.GetFileName(file);
                string subPath = file.Replace(nameOnly, "");
                subPath = subPath.Replace(inputDirectory, "");
                subPath = subPath.Substring(1, subPath.Length - 1);
                return Path.Combine(outputDirectory, subPath);
            }
            else
            {
                return outputDirectory;
            }
        }

        private Tuple<ConvertResultEventArgs, ConvertResultEventArgs> ProcessFile(string file, IMonsterConverter input, IMonsterConverter output, string outputDir)
        {
            ConvertResultEventArgs readResult = new ConvertResultEventArgs("unknown", ConvertError.Error, "Unknown error occured");
            ConvertResultEventArgs writeResult = new ConvertResultEventArgs("unknown", ConvertError.Error, "Unknown error occured");

            // The ReadMonster and Write methods processors should really do their best to catch and return meaningful errors
            try
            {
                readResult = input.ReadMonster(file, out Monster monster);
                if (readResult.Code != ConvertError.Error)
                {
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    writeResult = output.WriteMonster(outputDir, ref monster);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error pasring {file}. Exception {ex.Message}");
            }
            return new(readResult, writeResult);
        }

        protected bool RaiseEvent<T>(EventHandler<T> eventHandler, T args) where T : EventArgs
        {
            bool result = false;

            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var handler = eventHandler;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, args);
                result = true;
            }
            return result;
        }
    }
}
