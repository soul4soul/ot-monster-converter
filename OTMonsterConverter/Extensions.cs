using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OTMonsterConverter
{
    public static class Extensions
    {
        public static string FindNamedGroupValue(this MatchCollection mc, string name)
        {
            foreach (Match m in mc)
            {
                var group = m.Groups[name];
                if (group != null)
                {
                    return group.Value;
                }
            }

            throw new Exception($"Named group parameter {name} not found in MatchCollection");
        }
    }
}
