using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class PropertyDeclarationSyntaxComparer : IEqualityComparer<PropertyDeclarationSyntax>
    {
        public bool Equals(PropertyDeclarationSyntax x, PropertyDeclarationSyntax y)
        {
            return x != null && y != null && x.Type.ToString() == y.Type.ToString() && x.Identifier.Text == y.Identifier.Text;
        }

        public int GetHashCode(PropertyDeclarationSyntax obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.Identifier.Text.GetHashCode();
            hash = hash * 23 + obj.Type.ToString().GetHashCode();
            return hash;
        }
    }
}
