using MonsterConverterProcessor;
using System;
using System.Linq;
using MonsterConverterInterface;

namespace otmc
{
    class ConsoleWindow
    {
        private MonsterFileProcessor fileProcessor;
        private readonly string inputDirectory;
        private readonly string outputDirectory;
        private readonly string otbPath;
        private IMonsterConverter input;
        private IMonsterConverter output;
        private string itemConversionMethodValue;
        private bool mirrorFolderStructure;

        private int ConvertSuccessCount = 0;
        private int ConvertWarningCount = 0;
        private int ConvertErrorCount = 0;

        public ConsoleWindow(string inputDirectory, string outputDirectory, string inputFormatName, string outputFormatName, string otbPath, string itemConversionMethodValue, bool mirrorFolderStructure)
        {
            this.inputDirectory = inputDirectory;
            this.outputDirectory = outputDirectory;
            this.otbPath = otbPath;
            this.mirrorFolderStructure = mirrorFolderStructure;
            this.itemConversionMethodValue = itemConversionMethodValue;

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
            if (string.IsNullOrEmpty(outputDirectory))
            {
                Console.WriteLine("Output Directory not specified");
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
            else if (!Enum.TryParse<ItemConversionMethod>(itemConversionMethodValue, out _))
            {
                Console.WriteLine("Item Conversion method was not specified or invalid");
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
            ProcessorScanError result = fileProcessor.ConvertMonsterFiles(inputDirectory, input, outputDirectory, output, otbPath, Enum.Parse<ItemConversionMethod>(itemConversionMethodValue), mirroredFolderStructure: mirrorFolderStructure);
            switch (result)
            {
                case ProcessorScanError.Success:
                    Console.WriteLine("Completed.");
                    Console.WriteLine($"{ConvertSuccessCount} Monsters converted succesfully.");
                    Console.WriteLine($"{ConvertWarningCount} Monsters converted with warnings.");
                    Console.WriteLine($"{ConvertErrorCount} Monsters converted with errors.");
                    break;
                case ProcessorScanError.NoMonstersFound:
                    Console.WriteLine("Couldn't find any monster files.");
                    break;
                case ProcessorScanError.InvalidInputDirectory:
                    Console.WriteLine("The selected input directory is invald.");
                    break;
                case ProcessorScanError.CouldNotCreateDirectory:
                    Console.WriteLine("Couldn't create output directory.");
                    break;
                case ProcessorScanError.DirectoriesMatch:
                    Console.WriteLine("Input and output directories can't be the same.");
                    break;
                case ProcessorScanError.OtbReadFailed:
                    Console.WriteLine("Unable to read the specified OTB file.");
                    break;
                default:
                    break;
            }

            return (result == ProcessorScanError.Success);
        }
    }
}
