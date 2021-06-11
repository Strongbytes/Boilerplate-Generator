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

        public override bool MergeWithExistingClass => FileExistsInProject;

        public override AssetKind GeneratedClassKind => AssetKind.ProfileMapper;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.AutoMapper,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.CreateRequestDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UpdateRequestDomainEntity),
        }.OrderBy(x => x);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => Enumerable.Empty<PropertyDefinitionModel>();

        public override IEnumerable<MethodDefinitionModel> Constructors
        {
            get
            {
                return new MethodDefinitionModel[]
                {
                    new MethodDefinitionModel
                    {
                        Body = ConstructorBody
                    }
                };
            }
        }

        private IEnumerable<string> ConstructorBody
        {
            get
            {
                string referencedEntityName = _viewModelBase.EntityTree.First().Current.Name;

                return new string[]
                {
                    $"CreateMap<{referencedEntityName}, {_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>();",
                    $"CreateMap<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity]}, {referencedEntityName}>();",
                    $"CreateMap<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity]}, {referencedEntityName}>();",
                };
            }
        }
    }
}
