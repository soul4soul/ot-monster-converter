using System;
using System.Collections.Generic;
using System.IO;
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
            string inputDirectory = Console.ReadLine();

            //string[] array2 = Directory.GetFiles(inputDirectory, "*.xml");
            //enumerate files?

            //Console.Write("Enter Output Directory: ");
            //string outputPath = Console.ReadLine();

            Monster monster;
            ReadMonster(inputDirectory, out monster);
        }

        private static void ReadMonster(string filename, out Monster monster)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Monster));

            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filename, FileMode.Open);

            /* Use the Deserialize method to restore the object's state with
            data from the XML document. */
            monster = (Monster)serializer.Deserialize(fs);
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

        private static void WriteMonster(ref Monster monster)
        {

        }
    }
}
