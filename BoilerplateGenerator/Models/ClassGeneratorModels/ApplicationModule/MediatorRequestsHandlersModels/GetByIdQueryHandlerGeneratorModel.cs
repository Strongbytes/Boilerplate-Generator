using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class GetByIdQueryHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        public GetByIdQueryHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.GetByIdQueryHandler;
    }
}
