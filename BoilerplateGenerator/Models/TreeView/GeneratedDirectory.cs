using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Models.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoilerplateGenerator.Models.TreeView
{
    public class GeneratedDirectory : IBaseGeneratedAsset
    {
        public GeneratedDirectory(string directoryName)
        {
            AssetName = directoryName;
        }

        public AssetKind AssetKind => AssetKind.Directory;

        public string AssetName { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
