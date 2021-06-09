using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.MediatorRequestsModels
{
    public class GetAllQueryGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetAllQueryGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override AssetKind GeneratedClassKind => AssetKind.GetAllQuery;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.MediatR,
           UsingTokens.SystemCollectionsGeneric,
           _metadataGenerationService.AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<IEnumerable<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };
    }
}
