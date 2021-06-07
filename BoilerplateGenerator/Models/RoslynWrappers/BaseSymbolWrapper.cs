using BoilerplateGenerator.Models.Contracts;
using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class BaseSymbolWrapper<T> : IBaseSymbolWrapper where T : ISymbol
    {
        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public BaseSymbolWrapper(T symbol)
        {
            Name = symbol.Name;
            IsChecked = true;
        }
    }
}
