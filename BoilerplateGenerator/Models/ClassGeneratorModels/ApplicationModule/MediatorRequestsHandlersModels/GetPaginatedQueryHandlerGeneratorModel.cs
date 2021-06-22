using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.ExtraFeatures.Pagination;
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
        private readonly IPaginationRequirements _paginationRequirements;

        public GetPaginatedQueryHandlerGeneratorModel
        (
            IViewModelBase viewModelBase, 
            IMetadataGenerationService metadataGenerationService, 
            IPaginationRequirements paginationRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _paginationRequirements = paginationRequirements;
        }

        public override bool CanBeCreated => _viewModelBase.GetPaginatedQueryIsEnabled;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
            _paginationRequirements.PaginatedDataResponseInterface.Namespace,
        }.Union(base.UsingsBuilder);

        public override AssetKind Kind => AssetKind.GetPaginatedQueryHandler;

        protected override string HandlerResponseType => $"{CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>";
    }
}
