using BoilerplateGenerator.Models.Enums;

namespace BoilerplateGenerator.Contracts
{
    public interface IBaseGeneratedAsset
    {
        AssetKind AssetKind { get; }

        string AssetName { get; }
    }
}
