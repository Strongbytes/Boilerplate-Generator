using BoilerplateGenerator.Models.Enums;
using System.ComponentModel;

namespace BoilerplateGenerator.Contracts
{
    public interface IBaseGeneratedAsset : INotifyPropertyChanged
    {
        AssetKind AssetKind { get; }

        string AssetName { get; }
    }
}
