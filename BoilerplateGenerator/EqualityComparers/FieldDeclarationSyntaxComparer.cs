using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class FieldDeclarationSyntaxComparer : IEqualityComparer<FieldDeclarationSyntax>
    {
        public bool Equals(FieldDeclarationSyntax x, FieldDeclarationSyntax y)
        {
            return x != null && y != null && x.Declaration.Type.ToFullString() == y.Declaration.Type.ToFullString() 
                                          && x.Declaration.Variables.ToFullString() == y.Declaration.Variables.ToFullString();
        }

        public int GetHashCode(FieldDeclarationSyntax obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.Declaration.Type.ToFullString().GetHashCode();
            hash = hash * 23 + obj.Declaration.Variables.ToFullString().GetHashCode();
            return hash;
        }
    }
}
