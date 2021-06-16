using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace BoilerplateGenerator.Helpers
{
    public static class TypeExtensions
    {
        public static string ToLowerCamelCase(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsLower(text[0]))
                return text;

            return char.ToLower(text[0]) + text.Substring(1);
        }

        public static string ToUpperCamelCase(this string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsUpper(text[0]))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            IEnumerable<TSource> sortedList = source.OrderBy(keySelector).ToArray();
            source.Clear();

            foreach (TSource sortedItem in sortedList)
            {
                source.Add(sortedItem);
            }
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (!seenKeys.Add(keySelector(element)))
                {
                    continue;
                }

                yield return element;
            }
        }

        public static string ToTypeAlias(this ITypeSymbol dotNetTypeName)
        {
            switch (dotNetTypeName.Name)
            {
                case "Boolean":
                    return "bool";
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Char":
                    return "char";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "double";
                case "Single":
                    return "float";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Object":
                    return "object";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "String":
                    return "string";

                default: return dotNetTypeName.Name;
            }
        }
    }
}
