using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityClassWrapper : BaseSymbolWrapper<INamedTypeSymbol>
    {
        public EntityClassWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }
    }
}
