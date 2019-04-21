using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OTMonsterConverter
{
    class Program
    {
        public enum MonsterFormats
        {
            PyOT,
            TfsXml,
            TfsRevScriptSys
        }

        static void Main(string[] args)
        {
            bool show_help = false;
            string inputDirectory;
            int desiredFormat;
            string outputDirectory;
            bool mirrorFolderStructure = true;

            var p = new OptionSet()
            {
                "Usage: greet [OPTIONS]+ message",
                "Greet a list of individuals with an optional message.",
                "If no message is specified, a generic greeting is used.",
                "",
                "Options:",
                { "i|inputDirectory=", "The directory of monster files to parse.", v => inputDirectory = v },
                { "o|outputDirectory=", "The directory of monster files to parse.", v => outputDirectory = v },
                { "desiredFormat=", "The format to converter the monster files to.", (int v) => desiredFormat = v },
                { "m|MirrorFolders", "True to mirror the folder structure of the input directory", v => mirrorFolderStructure = v != null },
                { "h|help",  "show this message and exit", v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }

            if (show_help)
            {
                p.WriteOptionDescriptions(Console.Out);
                return;
            }

            // Call functions from converter class
            // Get list of files?
            // Get format from extensions?
            // Do for each file:
            ///
            // Read in file
            //  Print error for any unknown variables
            // Print out file
            //   Print error for any unknown variables
            // For each file output new file
            //  Print out erro
            ///
            // Done?

            Console.Write("Enter Monster File to Parse: ");
            string inputFile = Console.ReadLine();

            //string[] array2 = Directory.GetFiles(inputDirectory, "*.xml");
            //enumerate files?

            //Console.Write("Enter Output Directory: ");
            //string outputPath = Console.ReadLine();

            ICommonConverter tfsConverter = new TfsXmlConverter();
            ICustomMonster monster;
            tfsConverter.ReadMonster(inputFile, out monster);
        }
    }
}
