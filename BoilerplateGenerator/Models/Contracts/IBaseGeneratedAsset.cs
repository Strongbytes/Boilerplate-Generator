using BoilerplateGenerator.Models.ClassGeneratorModels;

namespace BoilerplateGenerator.Models.Contracts
{
    public interface IBaseGeneratedAsset
    {
        AssetKind AssetKind { get; }

        string AssetName { get; }
    }
}
