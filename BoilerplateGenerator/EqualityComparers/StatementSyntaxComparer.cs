using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class StatementSyntaxComparer : IEqualityComparer<StatementSyntax>
    {
        public bool Equals(StatementSyntax x, StatementSyntax y)
        {
            return x != null && y != null && x.GetText().ToString() == y.GetText().ToString();
        }

        public int GetHashCode(StatementSyntax obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.GetText().ToString().GetHashCode();
            return hash;
        }
    }
}
