using BoilerplateGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityPropertyWrapper : BaseSymbolWrapper<IPropertySymbol>
    {
        public string Type { get; set; }

        public EntityPropertyWrapper(IPropertySymbol symbol) : base(symbol)
        {
            Type = symbol.Type.ToTypeAlias();
        }
    }
}
