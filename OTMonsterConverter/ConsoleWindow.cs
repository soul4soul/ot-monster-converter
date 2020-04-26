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
        private MonsterFormat inputFormat;
        private MonsterFormat outputFormat;
        private bool mirrorFolderStructure;

        public ConsoleWindow(string inputDirectory, string outputDirectory, MonsterFormat inputFormat, MonsterFormat outputFormat, bool mirrorFolderStructure)
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
            else
            {
                return true;
            }
        }

        public bool ScanFiles()
        {
            Console.WriteLine("Scanning...");
            ScanError result = fileProcessor.ConvertMonsterFiles(inputDirectory, inputFormat, outputDirectory, outputFormat, mirrorFolderStructure);
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
                default:
                    break;
            }

            return (result == ScanError.Success) ? true : false;
        }
    }
}
