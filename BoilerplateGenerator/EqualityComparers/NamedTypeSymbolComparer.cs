using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class NamedTypeSymbolComparer : IEqualityComparer<INamedTypeSymbol>
    {
        public bool Equals(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            return x != null && y != null && x.MetadataName == y.MetadataName;
        }

        public int GetHashCode(INamedTypeSymbol obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.MetadataName.GetHashCode();
            return hash;
        }
    }
}
