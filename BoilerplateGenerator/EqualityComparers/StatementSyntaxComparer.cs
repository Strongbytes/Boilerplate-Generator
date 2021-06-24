using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class StatementSyntaxComparer : IEqualityComparer<StatementSyntax>
    {
        public bool Equals(StatementSyntax x, StatementSyntax y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            var normalizedX = x.NormalizeWhitespace().GetText().ToString().Replace(";", string.Empty);
            var normalizedY = y.NormalizeWhitespace().GetText().ToString().Replace(";", string.Empty);

            return normalizedX == normalizedY;
        }

        public int GetHashCode(StatementSyntax obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.NormalizeWhitespace().GetText().ToString().GetHashCode();
            return hash;
        }
    }
}
