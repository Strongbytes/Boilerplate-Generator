using BoilerplateGenerator.Models.Contracts;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class GeneratedDirectory : IBaseGeneratedAsset
    {
        public GeneratedDirectory(string directoryName)
        {
            AssetName = directoryName;
        }

        public AssetKind AssetKind => AssetKind.Directory;

        public string AssetName { get; }
    }
}
