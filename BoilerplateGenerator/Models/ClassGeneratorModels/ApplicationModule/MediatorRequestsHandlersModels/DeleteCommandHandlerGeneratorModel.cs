using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class DeleteCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public DeleteCommandHandlerGeneratorModel
        (
            IViewModelBase viewModelBase,
            IMetadataGenerationService metadataGenerationService,
            IUnitOfWorkRequirements unitOfWorkRequirements
        )
            : base(viewModelBase, metadataGenerationService, unitOfWorkRequirements)
        {
            _viewModelBase = viewModelBase;
        }

        public override bool CanBeCreated => _viewModelBase.DeleteCommandIsEnabled;

        public override AssetKind Kind => AssetKind.DeleteCommandHandler;

        protected override string HandlerResponseType => $"{CommonTokens.Unit}";
    }
}
