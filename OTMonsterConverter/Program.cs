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
            Console.Write("Enter Monster File to Parse: ");
            string inputFile = Console.ReadLine();

            //string[] array2 = Directory.GetFiles(inputDirectory, "*.xml");
            //enumerate files?

            //Console.Write("Enter Output Directory: ");
            //string outputPath = Console.ReadLine();

            ICommonConverter tfsConverter = new TFSConverter();
            GenericMonster monster;
            tfsConverter.ReadMonster(inputFile, out monster);
        }
    }
}
