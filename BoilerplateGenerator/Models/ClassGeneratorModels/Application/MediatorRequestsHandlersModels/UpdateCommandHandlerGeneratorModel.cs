using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.MediatorRequestsHandlersModels
{
    public class UpdateCommandHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        public UpdateCommandHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.UpdateCommandHandler;
    }
}
