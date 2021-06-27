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
        private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> typePropInfoDic = new Dictionary<Type, IDictionary<string, PropertyInfo>>();

        public static T Deseralize<T>(string input) where T : new()
        {
            if (input == null)
            {
                throw new Exception("Input is null");
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
                // TODO when there is a param like |look_direction=| this will incorrectly set the field as that value instead of skipping the field or setting the value as an empty string
                // Seems to work on regexr but not in c#
                Regex templatePartRegex = new Regex(@"(\s*(?<name>\w+)\s*=)?\s*(?<value>.+)\s*", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

                if (!typePropInfoDic.ContainsKey(myType))
                    typePropInfoDic.Add(myType, GetIndexedPropertyNames(myType));
                var indexedPropertyNames = typePropInfoDic[myType];

                if ((attr != null) && (attr.BeforeCaptureProperty != null) && (m.Groups["before"].Success))
                {
                    if (!indexedPropertyNames.ContainsKey(attr.BeforeCaptureProperty.ToLower()))
                    {
                        System.Diagnostics.Debug.WriteLine($"template {templateName} no matching BeforeCaptureProperty found");
                    }
                    prop = indexedPropertyNames[attr.BeforeCaptureProperty.ToLower()];
                    prop.SetValue(output, m.Groups["before"].Value.Trim(), null);
                }

                if ((attr != null) && (attr.AfterCaptureProperty != null) && (m.Groups["after"].Success))
                {
                    if (!indexedPropertyNames.ContainsKey(attr.AfterCaptureProperty.ToLower()))
                    {
                        System.Diagnostics.Debug.WriteLine($"template {templateName} no matching AfterCaptureProperty found");
                    }
                    prop = indexedPropertyNames[attr.AfterCaptureProperty.ToLower()];
                    prop.SetValue(output, m.Groups["after"].Value.Trim(), null);
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
            if (attrs.Length >= 1)
                return attrs[0] as TemplateNameAttribute;
            else
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
