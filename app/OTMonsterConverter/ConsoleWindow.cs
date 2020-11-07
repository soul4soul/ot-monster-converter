using MonsterConverterInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace OTMonsterConverter
{
    class ConsoleWindow
    {
        private MonsterFileProcessor fileProcessor;
        private string inputDirectory;
        private string outputDirectory;
        private string inputFormat;
        private string outputFormat;
        private IMonsterConverter input;
        private IMonsterConverter output;
        private bool mirrorFolderStructure;

        public ConsoleWindow(string inputDirectory, string outputDirectory, string inputFormat, string outputFormat, bool mirrorFolderStructure)
        {
            this.inputDirectory = inputDirectory;
            this.outputDirectory = outputDirectory;
            this.inputFormat = inputFormat;
            this.outputFormat = outputFormat;
            this.mirrorFolderStructure = mirrorFolderStructure;

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
            // Big below
            else if ((string.IsNullOrEmpty(inputFormat)) || (true))
            {
                input = null;
                Console.WriteLine("Input format was not specified or invalid");
                return false;
            }
            else if ((string.IsNullOrEmpty(outputFormat)) || (true))
            {
                output = null;
                Console.WriteLine("Input format was not specified or invalid");
                return false;
            }
            // add check/convert plugin string name to instance of plugin class, only send plugin class to consolewindow class?
            else
            {
                return true;
            }
        }

        public bool ScanFiles()
        {
            Console.WriteLine("Scanning...");
            // add convert plugin string name to instance of plugin class, only send plugin class to consolewindow class?
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
