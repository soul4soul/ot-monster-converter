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
        static void Main(string[] args)
        {
            bool show_help = false;
            string inputDirectory = "";
            int desiredFormat = 0;
            string outputDirectory = "";
            bool mirrorFolderStructure = true;

            var p = new OptionSet()
            {
                "Usage: OTMonsterConverter [OPTIONS]+",
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
                Console.Write("OTMonsterConverter: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `OTMonsterConverter --help' for more information.");
                return;
            }

            if (show_help)
            {
                p.WriteOptionDescriptions(Console.Out);
                return;
            }

            MonsterConverter converter = new MonsterConverter(inputDirectory,
                                                              outputDirectory,
                                                              (MonsterFormat)desiredFormat,
                                                              mirrorFolderStructure);
            converter.ParseFiles();
        }
    }
}
