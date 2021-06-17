using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class DeleteCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public DeleteCommandHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }
        public override bool CanBeCreated => _viewModelBase.DeleteCommandIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.DeleteCommandHandler;

        protected override string HandlerResponseType => $"{CommonTokens.Unit}";
    }
}
