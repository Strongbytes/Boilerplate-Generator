using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class BaseTypeSyntaxComparer : IEqualityComparer<BaseTypeSyntax>
    {
        public bool Equals(BaseTypeSyntax x, BaseTypeSyntax y)
        {
            return x != null && y != null && x.GetText().ToString().Trim() == y.GetText().ToString().Trim();
        }

        public int GetHashCode(BaseTypeSyntax obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.GetText().ToString().Trim().GetHashCode();
            return hash;
        }
    }
}
