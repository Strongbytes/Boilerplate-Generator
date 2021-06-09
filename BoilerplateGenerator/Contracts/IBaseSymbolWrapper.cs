using System.ComponentModel;

namespace BoilerplateGenerator.Contracts
{
    public interface IBaseSymbolWrapper : INotifyPropertyChanged
    {
        string Name { get; set; }

        bool? IsChecked { get; set; }

        bool IsPropertyChanging { get; set; }

        bool IsEnabled { get; }
    }
}
