using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsModels
{
    public class GetAllQueryGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetAllQueryGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool CanBeCreated => _viewModelBase.GetAllQueryIsEnabled;

        public override AssetKind Kind => AssetKind.GetAllQuery;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
           UsingTokens.SystemCollectionsGeneric,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.IRequest}<{CommonTokens.IEnumerable}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>>"
            }
        };
    }
}
