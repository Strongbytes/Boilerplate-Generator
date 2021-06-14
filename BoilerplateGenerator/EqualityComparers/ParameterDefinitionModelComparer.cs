using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;

namespace BoilerplateGenerator.EqualityComparers
{
    public class ParameterDefinitionModelComparer : IEqualityComparer<ParameterDefinitionModel>
    {
        public bool Equals(ParameterDefinitionModel x, ParameterDefinitionModel y)
        {
            return x != null && y != null && x.ReturnType == y.ReturnType && x.Name == y.Name;
        }

        public int GetHashCode(ParameterDefinitionModel obj)
        {
            if (obj == null)
                return default;

            int hash = 17;

            hash = hash * 23 + obj.Name.GetHashCode();
            hash = hash * 23 + obj.ReturnType.GetHashCode();
            return hash;
        }
    }
}
