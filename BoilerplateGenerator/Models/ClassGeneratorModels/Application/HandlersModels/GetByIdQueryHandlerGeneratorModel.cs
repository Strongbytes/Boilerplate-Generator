using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class GetByIdQueryHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetByIdQueryHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override AssetKind GeneratedClassKind => AssetKind.GetByIdQueryHandler;

        protected override string HandlerResponseType => $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}";
    }
}
