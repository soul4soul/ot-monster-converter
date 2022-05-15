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
        private record PropInfoWithTemplateAttr(PropertyInfo PropertyInfo, TemplateParameterAttribute TemplateNameAttribute);

        private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> typePropInfoDic = new Dictionary<Type, IDictionary<string, PropertyInfo>>();
        private static readonly IDictionary<Type, IList<PropInfoWithTemplateAttr>> typeOrderedPropInfo = new Dictionary<Type, IList<PropInfoWithTemplateAttr>>();

        public static bool IsTemplateMatch<T>(string input)
        {
            if (input == null)
                return false;

            Type myType = typeof(T);
            string templateName = myType.Name;
            TemplateNameAttribute attr = GetTemplateNameAttribute(myType);
            if (attr != null)
            {
                templateName = attr.Name;
            }

            Regex templateWholeRegex = new Regex($"(?<before>.*){{{{(?<template>{templateName}.*)}}}}(?<after>.*)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return templateWholeRegex.Match(input).Success;
        }

        public static string Serialize<T>(T input, bool isSingleLine = true) where T : new()
        {
            Type myType = typeof(T);
            string templateName = myType.Name;
            TemplateNameAttribute attr = GetTemplateNameAttribute(myType);
            string beforePropName = null;
            string afterPropName = null;
            if (attr != null)
            {
                templateName = attr.Name;
                beforePropName = attr.BeforeCaptureProperty;
                afterPropName = attr.AfterCaptureProperty;
            }

            if (!typeOrderedPropInfo.ContainsKey(myType))
                typeOrderedPropInfo.Add(myType, GetOrderedPropertyNames(myType, beforePropName, afterPropName));
            var indexedPropertyNames = typeOrderedPropInfo[myType];

            // Used to align equal signs for multiline output
            var longestParamName = 0;
            foreach (var propInfoTemplateAttr in indexedPropertyNames)
            {
                string name = propInfoTemplateAttr.PropertyInfo.Name;
                if (!string.IsNullOrWhiteSpace(propInfoTemplateAttr.TemplateNameAttribute.Name))
                {
                    name = propInfoTemplateAttr.TemplateNameAttribute.Name;
                }
                if (name.Length > longestParamName)
                {
                    longestParamName = name.Length;
                }
            }

            string paramPrefix = isSingleLine ? "|" : $"{Environment.NewLine}| ";
            string paramAfterEqualAlignment = isSingleLine ? "" : " ";
            string templateClosingAlignment = isSingleLine ? "" : $"{Environment.NewLine}";

            string output = $"{{{{{templateName}";
            bool hasSkippedProp = false;
            foreach (var propInfoTemplateAttr in indexedPropertyNames)
            {
                string name = propInfoTemplateAttr.PropertyInfo.Name;
                if (!string.IsNullOrWhiteSpace(propInfoTemplateAttr.TemplateNameAttribute.Name))
                {
                    name = propInfoTemplateAttr.TemplateNameAttribute.Name;
                }

                object value = null;
                if (propInfoTemplateAttr.PropertyInfo.PropertyType.IsArray)
                {
                    object objValue = propInfoTemplateAttr.PropertyInfo.GetValue(input, new object[0]);
                    if (objValue is object[])
                    {
                        object[] objArray = objValue as object[];
                        if (objArray.Length > 0)
                        {
                            value = string.Join($"{paramPrefix}", objArray);
                        }
                    }
                }
                else
                {
                    value = propInfoTemplateAttr.PropertyInfo.GetValue(input);
                }

                if (value == null && propInfoTemplateAttr.TemplateNameAttribute.Required != ParameterRequired.Yes)
                {
                    hasSkippedProp = true;
                    continue;
                }

                if (propInfoTemplateAttr.TemplateNameAttribute.Indicator == ParameterIndicator.Name || hasSkippedProp)
                {
                    string paramBeforeEqualAlignment = "";
                    if (!isSingleLine)
                    {
                        int spaceCount = longestParamName - name.Length + 1;
                        paramBeforeEqualAlignment = " ".Multiply(spaceCount);
                    }
                    output += $"{paramPrefix}{name}{paramBeforeEqualAlignment}={paramAfterEqualAlignment}{value}";
                }
                else
                {
                    output += $"{paramPrefix}{value}";
                }
            }
            output += $"{templateClosingAlignment}}}}}";

            return output;
        }

        private static IList<PropInfoWithTemplateAttr> GetOrderedPropertyNames(Type myType, string beforePropName, string afterPropName)
        {
            IList<PropInfoWithTemplateAttr> result = new List<PropInfoWithTemplateAttr>();

            foreach (PropertyInfo pi in myType.GetProperties())
            {
                if ((pi.Name == beforePropName) || (pi.Name == afterPropName)) { continue; }

                object[] attrObjs = pi.GetCustomAttributes(typeof(TemplateParameterAttribute), false);
                if (attrObjs.Length == 0) { continue; }

                foreach (var attrObj in attrObjs)
                {
                    TemplateParameterAttribute templateParmAttr = attrObj as TemplateParameterAttribute;
                    if (templateParmAttr != null)
                    {
                        result.Add(new PropInfoWithTemplateAttr(pi, templateParmAttr));
                    }
                }
            }

            return result.OrderBy(p => p.TemplateNameAttribute.Index).ToList();
        }

        public static T Deserialize<T>(string input) where T : new()
        {
            if (input == null)
            {
                throw new NullReferenceException("input is null");
            }

            Type myType = typeof(T);
            string templateName = myType.Name;
            TemplateNameAttribute attr = GetTemplateNameAttribute(myType);
            if (attr != null)
            {
                templateName = attr.Name;
            }

            Regex templateWholeRegex = new Regex($"(?<before>.*){{{{(?<template>{templateName}.*)}}}}(?<after>.*)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var m = templateWholeRegex.Match(input);

            if (m.Success)
            {
                T output = new T();
                PropertyInfo prop;
                // Fix when there is a param like |look_direction=| this will incorrectly set the field as that value instead of skipping the field or setting the value as an empty string
                // Seems to work on regexr but not in c#
                Regex templatePartRegex = new Regex(@"(\s*(?<name>\w+)\s*=)?\s*(?<value>.+)\s*", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                if (!typePropInfoDic.ContainsKey(myType))
                    typePropInfoDic.Add(myType, GetIndexedPropertyNames(myType));
                var indexedPropertyNames = typePropInfoDic[myType];

                if ((attr != null) && (attr.BeforeCaptureProperty != null) && (m.Groups["before"].Success))
                {
                    if (!indexedPropertyNames.ContainsKey(attr.BeforeCaptureProperty))
                    {
                        System.Diagnostics.Debug.WriteLine($"template {templateName} no matching BeforeCaptureProperty found");
                    }
                    else
                    {

                        prop = indexedPropertyNames[attr.BeforeCaptureProperty];
                        prop.SetValue(output, m.Groups["before"].Value.Trim(), null);
                    }
                }

                if ((attr != null) && (attr.AfterCaptureProperty != null) && (m.Groups["after"].Success))
                {
                    if (!indexedPropertyNames.ContainsKey(attr.AfterCaptureProperty))
                    {
                        System.Diagnostics.Debug.WriteLine($"template {templateName} no matching AfterCaptureProperty found");
                    }
                    else
                    {
                        prop = indexedPropertyNames[attr.AfterCaptureProperty];
                        prop.SetValue(output, m.Groups["after"].Value.Trim(), null);
                    }
                }

                PropertyInfo arrayProp = null;
                Array arrayval = null;
                int arrayIndex = 0;
                var parts = m.Groups["template"].Value.SplitTopLevel('|');
                for (int i = 1; i < parts.Length; i++)
                {
                    m = templatePartRegex.Match(parts[i]);
                    if ((arrayProp == null) && (arrayval == null))
                    {
                        if (m.Groups["name"].Success)
                        {
                            string parameterName = m.Groups["name"].Value.ToLower();
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
                            arrayval.SetValue(m.Groups["value"].Value.Trim(), arrayIndex);
                            arrayIndex++;
                        }
                        else
                        {
                            prop.SetValue(output, m.Groups["value"].Value.Trim(), null);
                        }
                    }
                    else
                    {
                        arrayval.SetValue(m.Groups["value"].Value.Trim(), arrayIndex);
                        arrayIndex++;
                    }
                }
                if (arrayProp != null)
                {
                    arrayProp.SetValue(output, arrayval);
                }

                return output;
            }
            else
            {
                throw new Exception($"Invalid string format or {templateName} mismatch");
            }
        }

        private static TemplateNameAttribute GetTemplateNameAttribute(MemberInfo element)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(element, typeof(TemplateNameAttribute));
            foreach (var attrObj in attrs)
            {
                if (attrObj is TemplateNameAttribute)
                    return attrObj as TemplateNameAttribute;
            }
            return null;
        }

        private static IDictionary<string, PropertyInfo> GetIndexedPropertyNames(Type myType)
        {
            // <Index as string, PropInfo> && <Name, PropInfo>
            IDictionary<string, PropertyInfo> propInfoDic = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo pi in myType.GetProperties())
            {
                object[] attrObjs = pi.GetCustomAttributes(typeof(TemplateParameterAttribute), false);
                if (attrObjs.Length == 0) { continue; }

                foreach (var attrObj in attrObjs)
                {
                    TemplateParameterAttribute templateParmAttr = attrObj as TemplateParameterAttribute;
                    if (templateParmAttr != null)
                    {
                        string loopUpName = pi.Name;
                        if (!string.IsNullOrWhiteSpace(templateParmAttr.Name))
                            loopUpName = templateParmAttr.Name;

                        if (templateParmAttr.Indicator.HasFlag(ParameterIndicator.Position))
                            propInfoDic.Add(templateParmAttr.Index.ToString(), pi);
                        if (templateParmAttr.Indicator.HasFlag(ParameterIndicator.Name))
                            propInfoDic.Add(loopUpName.ToLower(), pi);
                    }
                }
            }

            return propInfoDic;
        }
    }
}
