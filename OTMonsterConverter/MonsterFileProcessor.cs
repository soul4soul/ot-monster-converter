using OTMonsterConverter.Converter;
using OTMonsterConverter.MonsterTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public enum MonsterFormat
    {
        PyOT,
        TfsXml,
        TfsRevScriptSys,
        TibiaWiki
    }

    public sealed class FileProcessorEventArgs : EventArgs
    {
        public FileProcessorEventArgs(string sourceMonsterFile, string destinationFile, bool convertedSuccessfully = true)
        {
            SourceMonsterFile = sourceMonsterFile;
            DestinationFile = destinationFile;
            ConvertedSuccessfully = convertedSuccessfully;
        }

        public string SourceMonsterFile { get; }
        public string DestinationFile { get; }
        public bool ConvertedSuccessfully { get; }
    }

    public class MonsterFileProcessor : EventArgs
    {
        // Events
        public event EventHandler<FileProcessorEventArgs> OnMonsterConverted;

        // Properties
        private string MonsterDirectory { get; set; }

        private string BaseOutputDirectory { get; set; }

        // Functions
        public ScanError ConvertMonsterFiles(string monsterDirectory, MonsterFormat inputFormat, string outputDirectory, MonsterFormat outputFormat, bool mirroredFolderStructure = false)
        {
            MonsterDirectory = monsterDirectory;
            BaseOutputDirectory = outputDirectory;

            if (Path.GetFullPath(monsterDirectory) == Path.GetFullPath(outputDirectory))
            {
                return ScanError.DirectoriesMatch;
            }

            // Input and output formats can match. Formats matching is an interesting run configuration.
            // The generated files should match the input files

            if (Directory.Exists(monsterDirectory))
            {
                var result = FormatToConverter(inputFormat, out IMonsterConverter inputConvert);
                if (result != ScanError.Success)
                    return result;

                result = FormatToConverter(outputFormat, out IMonsterConverter outputConvert);
                if (result != ScanError.Success)
                    return result;

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

                string[] files = Directory.GetFiles(monsterDirectory, inputConvert.FileExtRegEx, SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    return ScanError.NoMonstersFound;
                }

                bool copyOk;
                string destination;
                foreach (string file in files)
                {
                    destination = FindDestination(file, mirroredFolderStructure);
                    copyOk = ProcessFile(file, inputConvert, outputConvert, destination);
                    RaiseEvent(OnMonsterConverted, new FileProcessorEventArgs(file, destination, copyOk));
                }
            }
            else
            {
                return ScanError.InvalidMonsterDirectory;
            }

            return ScanError.Success;
        }

        private string FindDestination(string file, bool mirroredFolderStructure)
        {
            if (mirroredFolderStructure)
            {
                string nameOnly = Path.GetFileName(file);
                string subPath = file.Replace(nameOnly, "");
                subPath = subPath.Replace(MonsterDirectory, "");
                subPath = subPath.Substring(1, subPath.Length - 1);
                return Path.Combine(BaseOutputDirectory, subPath);
            }
            else
            {
                return BaseOutputDirectory;
            }
        }

        // This is dumb
        private ScanError FormatToConverter(MonsterFormat format, out IMonsterConverter converter)
        {
            if (format == MonsterFormat.TfsXml)
            {
                converter = new TfsXmlConverter();
            }
            else if (format == MonsterFormat.PyOT)
            {
                converter = new PyOtConverter();
            }
            else if (format == MonsterFormat.TfsRevScriptSys)
            {
                converter = new TfsRevScriptSysConverter();
            }
            else
            {
                converter = null;
                return ScanError.InvalidMonsterFormat;
            }
            return ScanError.Success;
        }

        private bool ProcessFile(string file, IMonsterConverter input, IMonsterConverter output, string outputDir)
        {
            bool result = false;
            try
            {
                if (input.ReadMonster(file, out Monster monster))
                {
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    output.WriteMonster(outputDir, ref monster);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error pasring {file}. Exception {ex.Message}");
                result = false;
            }
            return result;
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
