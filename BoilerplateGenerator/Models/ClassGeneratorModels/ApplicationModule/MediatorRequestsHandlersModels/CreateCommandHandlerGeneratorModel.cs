using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class CreateCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        public CreateCommandHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.CreateCommandHandler;
    }
}
