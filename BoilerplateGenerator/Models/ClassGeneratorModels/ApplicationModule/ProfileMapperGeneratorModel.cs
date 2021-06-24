using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule
{
    public class ProfileMapperGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        public ProfileMapperGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool MergeWithExistingAsset => FileExistsInProject;

        public override bool CanBeCreated => _viewModelBase.GenerateAutoMapperProfile;

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[] { nameof(CommonTokens.Profile) }
        };

        public override AssetKind Kind => AssetKind.ProfileMapper;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.AutoMapper,
           $"{_viewModelBase.EntityTree.PrimaryEntityNamespace()}",
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.CreateRequestDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UpdateRequestDomainEntity),
        };

        protected override IEnumerable<ConstructorDefinitionModel> DefinedConstructorsBuilder => new ConstructorDefinitionModel[]
        {
            new ConstructorDefinitionModel
            {
                Body = ConstructorBody,
                MemberConflictResolutionKind = MemberConflictResolutionKind.Merge
            }
        };

        private IEnumerable<string> ConstructorBody
        {
            get
            {
                string referencedEntityType = _viewModelBase.EntityTree.PrimaryEntityType();

                ICollection<string> statements = new List<string>
                {
                    $"{CommonTokens.CreateMap}<{referencedEntityType}, {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>();",
                };

                if (_viewModelBase.CreateCommandIsEnabled)
                {
                    statements.Add($"{CommonTokens.CreateMap}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.CreateRequestDomainEntity]}, {referencedEntityType}>();");
                }

                if (_viewModelBase.UpdateCommandIsEnabled)
                {
                    statements.Add($"{CommonTokens.CreateMap}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UpdateRequestDomainEntity]}, {referencedEntityType}>();");
                }

                return statements;
            }
        }
    }
}
