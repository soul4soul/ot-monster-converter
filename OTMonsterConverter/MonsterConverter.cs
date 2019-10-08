using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTMonsterConverter
{
    public enum MonsterFormat
    {
        PyOT,
        TfsXml,
        TfsRevScriptSys
    }

    class MonsterConverter
    {
        public MonsterConverter(string inputDirectory, string outputDirectory, MonsterFormat desiredFormat, bool mirroredFolderStructure)
        {
            InputDirectory = inputDirectory;
            OutputDirectory = outputDirectory;
            OutputFormat = desiredFormat;
            MirrorFolderStructure = mirroredFolderStructure;
        }

        private string InputDirectory { get; set; }

        private string OutputDirectory { get; set; }

        private MonsterFormat InputFormat { get; set; }

        private MonsterFormat OutputFormat { get; set; }

        private bool MirrorFolderStructure { get; set; }

        public bool ParseFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(InputDirectory, "*.*", SearchOption.AllDirectories);
                string ext = Path.GetExtension(files[0]);
                if (ext == ".xml")
                {
                    InputFormat = MonsterFormat.TfsXml;
                }

                foreach(string file in files)
                {
                    ParseFile(file);
                }
            }
            catch (Exception ex)
            {

            }

            // event for progress?
            return false;
        }

        private bool ParseFile(string fileName)
        {
            try
            {
                string outputDirectory;

                ICommonConverter tfsConverter = new TfsXmlConverter();
                ICustomMonster monster;
                tfsConverter.ReadMonster(fileName, out monster);

                if (MirrorFolderStructure)
                {
                    string nameOnly = Path.GetFileName(fileName);
                    string subPath = fileName.Replace(nameOnly, "");
                    subPath = subPath.Replace(InputDirectory, "");
                    subPath = subPath.Substring(1, subPath.Length - 1);
                    outputDirectory = Path.Combine(OutputDirectory, subPath);
                }
                else
                {
                    outputDirectory = OutputDirectory;
                }

                ICommonConverter TfsRevScriptSysConverter = new TfsRevScriptSysConverter();
                TfsRevScriptSysConverter.WriteMonster(outputDirectory, ref monster);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pasring {fileName}. Exception {ex.Message}");
                return false;
            }
        }
    }
}
