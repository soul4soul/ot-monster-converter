using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    enum ParameterRequired
    {
        No,
        Partical,
        Yes
    }

    [Flags]
    enum ParameterIndicator
    {
        Name,
        Position
    }

    [AttributeUsage(AttributeTargets.Property)]
    class TemplateParameterAttribute : System.Attribute
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public ParameterRequired Required { get; set; }
        public ParameterIndicator Indicator { get; set; }
    }
}
