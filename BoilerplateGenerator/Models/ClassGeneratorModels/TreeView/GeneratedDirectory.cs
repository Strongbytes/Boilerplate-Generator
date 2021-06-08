using BoilerplateGenerator.Models.Contracts;
using BoilerplateGenerator.Models.Enums;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.TreeView
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
