using BoilerplateGenerator.Contracts.RoslynWrappers;
using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class BaseSymbolWrapper<T> : IBaseSymbolWrapper where T : ISymbol
    {
        public string Name { get; set; }

        public string Namespace { get; }

        private bool? _isChecked;
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (value == _isChecked)
                {
                    return;
                }

                if (!_isChecked.HasValue)
                {
                    value = true;
                }

                _isChecked = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsPropertyChanging { get; set; }

        public virtual bool IsEnabled { get; }

        public string ContainingModuleName { get; }

        public BaseSymbolWrapper(T symbol)
        {
            Name = symbol?.Name;
            Namespace = symbol?.ContainingNamespace.ToString();
            ContainingModuleName = symbol?.ContainingAssembly?.MetadataName;

            IsChecked = true;
            IsEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
