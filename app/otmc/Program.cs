using Mono.Options;
using MonsterConverterProcessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            string otbPath = "";
            string itemConversionMethod = ItemConversionMethod.KeepSourceIds.ToString();
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
            if (!string.IsNullOrWhiteSpace(inputConverterNames))
            {
                inputConverterNames = inputConverterNames.Substring(0, inputConverterNames.Length - 2);
            }
            else
            {
                inputConverterNames = "No Formats Found";
            }
            if (!string.IsNullOrWhiteSpace(outputConverterNames))
            {
                outputConverterNames = outputConverterNames.Substring(0, outputConverterNames.Length - 2);
            }
            else
            {
                outputConverterNames = "No Formats Found";
            }

            var p = new OptionSet()
            {
                "OT Monster Converter:",
                "Developed By Soul4Soul",
                "Repository located at https://github.com/soul4soul/ot-monster-converter",
                "",
                "Usage: otmc [OPTIONS]+",
                "",
                "Options:",
                { "i|inputDirectory=", "The directory of monster files to parse.", v => inputDirectory = v },
                { "o|outputDirectory=", "The output directory of the new monster files.", v => outputDirectory = v },
                { "inputFormat=", "The starting input monster file format.", v => inputFormat = v },
                { "outputFormat=", "The desired monster file format.", v => outputFormat = v },
                { "otbPath=", "The path to an otb file.", v => otbPath = v },
                { "itemIdFormat=", "Desired item id types used for the converted monser loot.", v => itemConversionMethod = v },
                { "m|MirrorFolders", "Mirror the folder structure of the input directory, otherwise flat folder structure is output", v => mirrorFolderStructure = v != null },
                { "h|help",  "show this message and exit", v => showHelp = v != null },
                "",
                $"Input Formats: {inputConverterNames}",
                $"Output Formats: {outputConverterNames}",
                $"Item Id Formats: {string.Join(", ", Enum.GetNames(typeof(ItemConversionMethod)))}",
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("OT Monster Converter: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `otmc --help' for more information and if the issue persists report it.");
                return -1;
            }

            if ((showHelp) || (args.Length == 0))
            {
                p.WriteOptionDescriptions(Console.Out);
                return 0 ;
            }

            ConsoleWindow consoleWindow = new ConsoleWindow(inputDirectory, outputDirectory, inputFormat, outputFormat, otbPath, itemConversionMethod, mirrorFolderStructure);
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
