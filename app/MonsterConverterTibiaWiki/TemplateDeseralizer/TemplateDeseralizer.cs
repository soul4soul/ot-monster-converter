using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    static class TemplateParser
    {
        private static IDictionary<Type, IDictionary<string, PropertyInfo>> typePropInfoDic = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        public static T Deseralize<T>(string input) where T : new()
        {
            if (Regex.IsMatch(input, "{{.*}}"))
            {
                Type myType = typeof(T);
                string templateName = myType.Name;
                Attribute[] attrs = Attribute.GetCustomAttributes(myType, typeof(TemplateNameAttribute));
                if (attrs.Length >= 1)
                {
                    templateName = (attrs[0] as TemplateNameAttribute).Name;
                }
                // TODO when there is a param like |look_direction=| this will incorrectly set the field as that value instead of skipping the field or setting the value as an empty string
                // Seems to work on regexr but not in c#
                Regex partRegex = new Regex(@"(\s*(?<name>\w+)\s*=)?\s*(?<value>.+)\s*", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline);

                if (!typePropInfoDic.ContainsKey(myType))
                    typePropInfoDic.Add(myType, GetIndexedPropertyNames(myType));
                var indexedPropertyNames = typePropInfoDic[myType];

                T output = new T();
                var parts = input[2..^2].SplitTopLevel('|');
                if (parts.Length == 0)
                {
                    throw new Exception($"No template parts found");
                }
                if (parts[0].ToLower() != templateName.ToLower())
                {
                    throw new Exception($"Template name {templateName} doesn't match input");
                }

                PropertyInfo arrayProp = null;
                PropertyInfo prop;
                Array arrayval = null;
                int arrayIndex = 0;
                for (int i = 1; i < parts.Length; i++)
                {
                    var m = partRegex.Match(parts[i]);
                    if ((arrayProp == null) && (arrayval == null))
                    {
                        if (m.Groups["name"].Success)
                        {
                            string parameterName = m.Groups["name"].Value;
                            if (!indexedPropertyNames.ContainsKey(parameterName))
                            {
                                System.Diagnostics.Debug.WriteLine($"template {templateName} index {parameterName} not parsed");
                                continue;
                            }
                            prop = indexedPropertyNames[parameterName];
                        }
                        else
                        {
                            string adjustedIndex = (i - 1).ToString();
                            if (!indexedPropertyNames.ContainsKey(adjustedIndex))
                            {
                                System.Diagnostics.Debug.WriteLine($"template {templateName} index {adjustedIndex} not parsed");
                                continue;
                            }
                            prop = indexedPropertyNames[adjustedIndex];
                        }
                        if (prop.PropertyType.IsArray)
                        {
                            arrayProp = prop;
                            arrayval = Array.CreateInstance(prop.PropertyType.GetElementType(), parts.Length - i);
                            arrayval.SetValue(m.Groups["value"].Value, arrayIndex);
                            arrayIndex++;
                        }
                        else
                        {
                            prop.SetValue(output, m.Groups["value"].Value, null);
                        }
                    }
                    else
                    {
                        arrayval.SetValue(m.Groups["value"].Value, arrayIndex);
                        arrayIndex++;
                    }
                }
                if (arrayProp != null)
                {
                    arrayProp.SetValue(output, arrayval);
                }

                return output;
            }

            throw new Exception("Invalid string format");
        }

        private static IDictionary<string, PropertyInfo> GetIndexedPropertyNames(Type myType)
        {
            // <Index as string, PropInfo> && <Name, PropInfo>
            IDictionary<string, PropertyInfo> propInfoDic = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo pi in myType.GetProperties())
            {
                object[] attrObjs = pi.GetCustomAttributes(typeof(TemplateParameterAttribute), false);
                if (attrObjs.Length == 0) { continue; }

                TemplateParameterAttribute templateParmAttr = attrObjs[0] as TemplateParameterAttribute;

                string loopUpName = pi.Name.ToLower();
                if (!string.IsNullOrWhiteSpace(templateParmAttr.Name))
                    loopUpName = templateParmAttr.Name.ToLower();

                if (templateParmAttr.Indicator.HasFlag(ParameterIndicator.Position))
                    propInfoDic.Add(templateParmAttr.Index.ToString(), pi);
                if (templateParmAttr.Indicator.HasFlag(ParameterIndicator.Name))
                    propInfoDic.Add(loopUpName, pi);
            }

            return propInfoDic;
        }
    }
}
