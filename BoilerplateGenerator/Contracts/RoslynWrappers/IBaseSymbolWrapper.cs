using System.ComponentModel;

namespace BoilerplateGenerator.Contracts.RoslynWrappers
{
    public interface IBaseSymbolWrapper : INotifyPropertyChanged
    {
        string Name { get; set; }

        bool? IsChecked { get; set; }

        bool IsPropertyChanging { get; set; }

        bool IsEnabled { get; }
    }
}
