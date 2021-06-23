using BoilerplateGenerator.Models.Enums;
using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface IMetadataGenerationService
    {
        string PrimaryEntityPluralizedName { get; }

        IDictionary<AssetKind, string> AssetToCompilationUnitNameMapping { get; }

        string NamespaceByAssetKind(AssetKind referencedAsset);

        IEnumerable<string> AvailableNamespaces { get; }
    }
}
