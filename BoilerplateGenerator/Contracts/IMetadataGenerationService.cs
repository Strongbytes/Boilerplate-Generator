using BoilerplateGenerator.Models.Enums;
using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts
{
    public interface IMetadataGenerationService
    {
        IDictionary<AssetKind, string> AssetToClassNameMapping { get; }

        IDictionary<AssetKind, string> AssetToNamespaceMapping { get; }
    }
}
