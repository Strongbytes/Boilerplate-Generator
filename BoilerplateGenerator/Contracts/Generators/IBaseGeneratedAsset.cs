using BoilerplateGenerator.Models.Enums;
using System.ComponentModel;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IBaseGeneratedAsset : INotifyPropertyChanged
    {
        AssetKind AssetKind { get; }

        string AssetName { get; }
    }
}
