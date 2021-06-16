using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityInterfaceWrapper : BaseSymbolWrapper<INamedTypeSymbol>
    {
        public EntityInterfaceWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }
    }
}
