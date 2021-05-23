using MonsterConverterInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OTMonsterConverter
{
    class ConsoleWindow
    {
        private MonsterFileProcessor fileProcessor;
        private string inputDirectory;
        private string outputDirectory;
        private IMonsterConverter input;
        private IMonsterConverter output;
        private bool mirrorFolderStructure;

        public ConsoleWindow(string inputDirectory, string outputDirectory, string inputFormatName, string outputFormatName, bool mirrorFolderStructure)
        {
            this.inputDirectory = inputDirectory;
            this.outputDirectory = outputDirectory;
            this.mirrorFolderStructure = mirrorFolderStructure;

            PluginHelper plugins = PluginHelper.Instance.Task.Result;
            input = plugins.Converters.FirstOrDefault(mc => mc.ConverterName == inputFormatName);
            output = plugins.Converters.FirstOrDefault(mc => mc.ConverterName == outputFormatName);

            fileProcessor = new MonsterFileProcessor();
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
                    Console.WriteLine("Completed successfully.");
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

            return (result == ScanError.Success) ? true : false;
        }
    }
}
