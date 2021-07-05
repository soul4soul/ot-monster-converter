using Mono.Options;
using MonsterConverterProcessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace otmc
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static async Task<int> Main(string[] args)
        {
            bool showHelp = false;

            string inputDirectory = "";
            string outputDirectory = "";
            string inputFormat = "";
            string outputFormat = "";
            bool mirrorFolderStructure = true;

            PluginHelper plugins = await PluginHelper.Instance;
            string inputConverterNames = "";
            string outputConverterNames = "";
            foreach (var c in plugins.Converters)
            {
                if (c.IsReadSupported)
                    inputConverterNames += $"{c.ConverterName}, ";
                if (c.IsWriteSupported)
                    outputConverterNames += $"{c.ConverterName}, ";
            }
            if (string.IsNullOrWhiteSpace(inputConverterNames))
            {
                inputConverterNames = "No Formats Found";
            }
            else
            {
                inputConverterNames = inputConverterNames.Substring(0, inputConverterNames.Length - 2);
            }
            if (string.IsNullOrWhiteSpace(outputConverterNames))
            {
                outputConverterNames = "No Formats Found";
            }
            else
            {
                outputConverterNames = outputConverterNames.Substring(0, outputConverterNames.Length - 2);
            }

            var p = new OptionSet()
            {
                "Usage: OTMonsterConverter [OPTIONS]+",
                "",
                "Options:",
                { "i|inputDirectory=", "The directory of monster files to parse.", v => inputDirectory = v },
                { "o|outputDirectory=", "The output directory of the new monster files.", v => outputDirectory = v },
                { "inputFormat=", "The starting input monster file format.", v => inputFormat = v },
                { "outputFormat=", "The desired monster file format.", v => outputFormat = v },
                { "m|MirrorFolders", "Mirror the folder structure of the input directory, otherwise flat folder structure is output", v => mirrorFolderStructure = v != null },
                { "h|help",  "show this message and exit", v => showHelp = v != null },
                "",
                $"Input Formats: {inputConverterNames}",
                $"Output Formats: {outputConverterNames}",
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("OTMonsterConverter: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `OTMonsterConverter --help' for more information.");
                return -1;
            }

            if ((showHelp) || (args.Length == 0))
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0 ;
            }

            ConsoleWindow consoleWindow = new ConsoleWindow(inputDirectory, outputDirectory, inputFormat, outputFormat, mirrorFolderStructure);
            if (!consoleWindow.ValidateValues())
            {
                return -2;
            }
            if (!consoleWindow.ScanFiles())
            {
                return -3;
            }
            else
            {
                return 0;
            }
        }
    }
}
