using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace BoilerplateGenerator.Contracts.RoslynWrappers
{
    public interface IBaseSymbolWrapper : INotifyPropertyChanged
    {
        string Name { get; set; }

        string Namespace { get; set; }

        bool? IsChecked { get; set; }

        bool IsPropertyChanging { get; set; }

        bool SymbolWasFound { get; set; }

        bool IsEnabled { get; }
    }
}
