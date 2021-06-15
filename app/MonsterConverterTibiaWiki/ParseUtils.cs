using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    enum FancySplitStyle
    {
        CommaOutsideParathese,
        BarOutsideBrackets
    }

    class ParseUtils
    {
        // https://stackoverflow.com/a/61140830
        public static IEnumerable<string> FancySplitString(string input, FancySplitStyle splitStyle)
        {
            var pattern = "";
            if (splitStyle == FancySplitStyle.CommaOutsideParathese)
            {
                pattern = @"
                (?>
                    (?<S>\()
                    |
                    (?<-S>\))
                    |
                    [^,()]
                    |
                    (?(S),|(?!))
                )+
                (?(S)(?!))
                ";
                var s = Regex.Replace(input, @"\((?>[^()]+|(?<o>)\(|(?<-o>)\))*(?(o)(?!))\)|(,)", m => m.Groups[1].Success ? "___temp___" : m.Value);
                return s.Split("___temp___");
            }
            else /*if (splitStyle == FancySplitStyle.BarOutsideBrackets)*/
            {
                pattern = @"
                (?>
                    (?<S>{)
                    |
                    (?<-S>})
                    |
                    [^|{}]
                    |
                    (?(S)\||(?!))
                )+
                (?(S)(?!))
                ";
                return Regex.Matches(input, pattern, RegexOptions.IgnorePatternWhitespace).Select(m => m.Value);
            }
        }
    }
}
