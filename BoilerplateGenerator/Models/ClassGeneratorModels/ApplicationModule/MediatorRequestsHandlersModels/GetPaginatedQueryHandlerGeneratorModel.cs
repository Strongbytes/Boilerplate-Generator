using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class GetPaginatedQueryHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetPaginatedQueryHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool CanBeCreated => _viewModelBase.GetPaginatedQueryIsEnabled;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
            _viewModelBase.PaginationRequirements.PaginatedDataResponseInterface.Namespace,
        }.Union(base.UsingsBuilder);

        public override AssetKind GeneratedAssetKind => AssetKind.GetPaginatedQueryHandler;

        protected override string HandlerResponseType => $"{CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>";
    }
}
