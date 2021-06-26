﻿using System;
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

    static class StringExtensions
    {
        // https://stackoverflow.com/a/61140830
        public static IEnumerable<string> Split(this string s, FancySplitStyle splitStyle)
        {
            if (splitStyle == FancySplitStyle.CommaOutsideParathese)
            {
                var sPrepared = Regex.Replace(s, @"\((?>[^()]+|(?<o>)\(|(?<-o>)\))*(?(o)(?!))\)|(,)", m => m.Groups[1].Success ? "___temp___" : m.Value);
                return sPrepared.Split("___temp___");
            }
            else /*if (splitStyle == FancySplitStyle.BarOutsideBrackets)*/
            {
                string pattern = @"
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
                return Regex.Matches(s, pattern, RegexOptions.IgnorePatternWhitespace).Select(m => m.Value);
            }
        }

        /// <summary>
        /// Split a string by a seperator only when that seperator is outside of containing edges
        /// </summary>
        /// <param name="s"></param>
        /// <param name="seperator"></param>
        /// <param name="leftEdges"></param>
        /// <param name="rightEdges"></param>
        /// <returns></returns>
        public static string[] SplitTopLevel(this string s, char seperator, string leftEdges = "{[(", string rightEdges = "}])")
        {
            int edgeBalance = 0;
            int lastSeperatorIndex = 0;
            IList<string> parts = new List<string>();

            for (int i = 0; i < s.Length; i++)
            {
                if (leftEdges.Contains(s[i]))
                {
                    edgeBalance++;
                    continue;
                }
                if (rightEdges.Contains(s[i]))
                {
                    edgeBalance--;
                    if (edgeBalance < 0)
                    {
                        throw new Exception("Edges are unbalanced, can't split string");
                    }
                    continue;
                }
                if ((edgeBalance == 0) && (s[i] == seperator))
                {
                    parts.Add(s[lastSeperatorIndex..i]);
                    lastSeperatorIndex = i + 1;
                }
            }

            if (edgeBalance == 0)
            {
                parts.Add(s[lastSeperatorIndex..^0]);
            }
            else if (edgeBalance > 0)
            {
                throw new Exception("Edges are unbalanced, can't split string");
            }

            return parts.ToArray();
        }
    }
}
