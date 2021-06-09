using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.MediatorRequestsHandlersModels
{
    public class DeleteCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        public DeleteCommandHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.DeleteCommandHandler;

        protected override string HandlerResponseType => $"{CommonTokens.Unit}";
    }
}
