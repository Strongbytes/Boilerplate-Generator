using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityClassWrapper : BaseSymbolWrapper<INamedTypeSymbol>
    {
        public bool IsBaseTypeInheritance { get; private set; }

        public EntityClassWrapper(INamedTypeSymbol symbol, bool isBaseTypeInheritance = false) : base(symbol)
        {
            IsBaseTypeInheritance = isBaseTypeInheritance;
        }

        public EntityClassWrapper(string name) : base(name)
        {
        }
    }
}
