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

        public TemplateNameAttribute(string name)
        {
            Name = name;
        }
    }
}
