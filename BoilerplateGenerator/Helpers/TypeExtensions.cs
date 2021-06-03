using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Helpers
{
    public static class TypeExtensions
    {
        public static string ToTypeAlias(this ITypeSymbol dotNetTypeName)
        {
            switch (dotNetTypeName.Name)
            {
                case "Boolean":
                    return typeof(bool).Name;
                case "Byte":
                    return typeof(byte).Name;
                case "SByte":
                    return typeof(sbyte).Name;
                case "Char":
                    return typeof(char).Name;
                case "Decimal":
                    return typeof(decimal).Name;
                case "Double":
                    return typeof(double).Name;
                case "Single":
                    return typeof(float).Name;
                case "Int32":
                    return typeof(int).Name;
                case "UInt32":
                    return typeof(uint).Name;
                case "Int64":
                    return typeof(long).Name;
                case "UInt64":
                    return typeof(ulong).Name;
                case "Object":
                    return typeof(object).Name;
                case "Int16":
                    return typeof(short).Name;
                case "UInt16":
                    return typeof(ushort).Name;
                case "String":
                    return typeof(string).Name;

                default: return dotNetTypeName.Name;
            }
        }
    }
}
