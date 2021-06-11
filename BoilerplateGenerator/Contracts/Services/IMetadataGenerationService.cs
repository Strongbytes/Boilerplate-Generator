using BoilerplateGenerator.Models.Enums;
using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface IMetadataGenerationService
    {
        IDictionary<AssetKind, string> AssetToClassNameMapping { get; }

        string NamespaceByAssetKind(AssetKind referencedAsset);

        IEnumerable<string> AvailableNamespaces { get; }
    }
}
