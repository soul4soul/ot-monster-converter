using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    class TemplateNameAttribute : System.Attribute
    {
        public string Name { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Optional capture text in a string being parsed for a template that occurs before the matched template
        /// Set this to the name of the property to store the captured string
        /// </summary>
        public string BeforeCaptureProperty { get; set; }
        /// <summary>
        /// Optional capture text in a string being parsed for a template that occurs after the matched template
        /// Set this to the name of the property to store the captured string
        /// </summary>
        public string AfterCaptureProperty { get; set; }

        public TemplateNameAttribute(string name)
        {
            Name = name;
        }
    }
}