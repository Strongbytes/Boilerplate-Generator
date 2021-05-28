using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Helpers
{
    public static class DynamicClassHelper
    {
        public static async Task<string> ExtractClassNameFromFile(string selectedEntityPath)
        {
            return await Task.Run(() =>
            {
                string entityContent = File.ReadAllText(selectedEntityPath);

                const string pattern = @"(internal|public|private|protected|sealed|abstract|static)?\s*class\s*(\w+)\s*?";

                var match = Regex.Match(entityContent, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);

                return match.Success && match.Groups.Count == 3 ? match.Groups[2].Value : string.Empty;
            }).ConfigureAwait(false);
        }

        public static void GetAllProperties(Type inputObjectType, IDictionary<Type, List<PropertyInfo>> properties)
        {
            try
            {
                PropertyInfo[] cProperties = inputObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo theProperty in cProperties)
                {
                    if (theProperty.PropertyType.IsClass)
                    {
                        GetAllProperties(theProperty.GetType(), properties);
                    }

                    if (!properties.ContainsKey(theProperty.GetType()))
                    {
                        properties.Add(theProperty.GetType(), new List<PropertyInfo>());
                    }

                    properties[theProperty.GetType()].Add(theProperty);
                }
            }
            catch (Exception theException)
            {
                // TODO: Do something with the exception
            }
        }
    }
}
