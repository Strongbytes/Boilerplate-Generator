using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class GetByIdQueryHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public GetByIdQueryHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }

        public override bool CanBeCreated => _viewModelBase.GetByIdQueryIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.GetByIdQueryHandler;
    }
}
