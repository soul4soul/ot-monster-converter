using MonsterConverterProcessor;
using System;
using System.Linq;
using MonsterConverterInterface;

namespace otmc
{
    class ConsoleWindow
    {
        private MonsterFileProcessor fileProcessor;
        private string inputDirectory;
        private string outputDirectory;
        private IMonsterConverter input;
        private IMonsterConverter output;
        private bool mirrorFolderStructure;

        private int ConvertSuccessCount = 0;
        private int ConvertWarningCount = 0;
        private int ConvertErrorCount = 0;

        public ConsoleWindow(string inputDirectory, string outputDirectory, string inputFormatName, string outputFormatName, bool mirrorFolderStructure)
        {
            this.inputDirectory = inputDirectory;
            this.outputDirectory = outputDirectory;
            this.mirrorFolderStructure = mirrorFolderStructure;

            PluginHelper plugins = PluginHelper.Instance.Task.Result;
            input = plugins.Converters.FirstOrDefault(mc => mc.ConverterName == inputFormatName);
            output = plugins.Converters.FirstOrDefault(mc => mc.ConverterName == outputFormatName);

            fileProcessor = new MonsterFileProcessor();
            fileProcessor.OnMonsterConverted += FileProcessor_OnMonsterConverted;
        }

        private void FileProcessor_OnMonsterConverted(object sender, FileProcessorEventArgs e)
        {
            if ((e.Source.Code == ConvertError.Error) || (e.Destination.Code == ConvertError.Error))
            {
                ConvertErrorCount++;
                PrintInfo(ConvertError.Error, e.Source, e.Destination);
            }
            else if ((e.Source.Code == ConvertError.Warning) || (e.Destination.Code == ConvertError.Warning))
            {
                ConvertWarningCount++;
                PrintInfo(ConvertError.Warning, e.Source, e.Destination);
            }
            else
            {
                ConvertSuccessCount++;
            }
        }

        private void PrintInfo(ConvertError overallError, ConvertResultEventArgs source, ConvertResultEventArgs destination)
        {
            string prefix = "ERROR";
            if (overallError == ConvertError.Warning)
                prefix = "WARNING";
            Console.WriteLine($"{prefix}: \"{source.File}\" to \"{destination.File}\"");

            if (!string.IsNullOrWhiteSpace(source.Message))
            {
                Console.WriteLine($"Source Msg: {source.Message}");
            }
            if (!string.IsNullOrWhiteSpace(destination.Message))
            {
                Console.WriteLine($"Destination Msg: {destination.Message}");
            }
        }

        public bool ValidateValues()
        {
            if (string.IsNullOrEmpty(inputDirectory))
            {
                Console.WriteLine("Solution Directory not specified");
                return false;
            }
            else if (string.IsNullOrEmpty(outputDirectory))
            {
                Console.WriteLine("DevExpress Directory not specified");
                return false;
            }
            else if ((input == null) || (!input.IsReadSupported))
            {
                Console.WriteLine("Input format was not specified or invalid");
                return false;
            }
            else if ((output == null) || (!output.IsWriteSupported))
            {
                Console.WriteLine("Output format was not specified or invalid");
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ScanFiles()
        {
            Console.WriteLine("Scanning...");
            ScanError result = fileProcessor.ConvertMonsterFiles(inputDirectory, input, outputDirectory, output, mirrorFolderStructure);
            switch (result)
            {
                case ScanError.Success:
                    Console.WriteLine("Completed.");
                    Console.WriteLine($"{ConvertSuccessCount} Monsters converted succesfully.");
                    Console.WriteLine($"{ConvertWarningCount} Monsters converted with warnings.");
                    Console.WriteLine($"{ConvertErrorCount} Monsters converted with errors.");
                    break;
                case ScanError.NoMonstersFound:
                    Console.WriteLine("Couldn't find any monster files.");
                    break;
                case ScanError.InvalidMonsterDirectory:
                    Console.WriteLine("The selected project directory is invald.");
                    break;
                case ScanError.InvalidMonsterFormat:
                    Console.WriteLine("The selected input or output monster format is invalid.");
                    break;
                case ScanError.CouldNotCreateDirectory:
                    Console.WriteLine("Couldn't create destination directory.");
                    break;
                case ScanError.DirectoriesMatch:
                    Console.WriteLine("Input and output directories can't be the same.");
                    break;
                default:
                    break;
            }

            return (result == ScanError.Success);
        }
    }
}
