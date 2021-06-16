using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/creating-custom-attributes
    class TemplateParser
    {
        class ParameterData
        {
            public string Name { get; set; }
            public int Index { get; set; }
            public ParameterRequired Required { get; set; }
        }


        // Broken func
        public static string Parse(string input)
        {
            if (Regex.IsMatch(input, "{{(melee|ability|summon|healing|debuff)"))
            {
                var parts = ParseUtils.FancySplitString(input, FancySplitStyle.BarOutsideBrackets);

            }

            return null;
        }


        // Working here!
        public static T Deseralize<T>(string input)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(typeof(T));

            var parts = ParseUtils.FancySplitString(input, FancySplitStyle.BarOutsideBrackets);

            if (Regex.IsMatch(input, "{{(melee|ability|summon|healing|debuff)"))
            {
                

            }

            return T;
        }
    }
}
