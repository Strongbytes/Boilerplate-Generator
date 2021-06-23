using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class UnitOfWorkImplementationGeneralModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        public UnitOfWorkImplementationGeneralModel
        (
            IViewModelBase viewModelBase,
            IMetadataGenerationService metadataGenerationService,
            IUnitOfWorkRequirements unitOfWorkRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _unitOfWorkRequirements = unitOfWorkRequirements;
        }

        public override bool CanBeCreated => _viewModelBase.UseUnitOfWork;

        public override bool MergeWithExistingAsset => true;

        public override AssetKind Kind => AssetKind.UnitOfWorkImplementation;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           _unitOfWorkRequirements.BaseUnitOfWorkClass.Namespace,
           _unitOfWorkRequirements.DbContextClass.Namespace,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UnitOfWorkInterface),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.EntityRepositoryInterface),
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            Type = SyntaxKind.ClassDeclaration,
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.BaseUnitOfWork}",
                $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UnitOfWorkInterface]}"
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                Accessors = new PropertyAccessorDefinitionModel[] 
                {
                    new PropertyAccessorDefinitionModel
                    {
                        AccessorType = SyntaxKind.GetAccessorDeclaration
                    },
                    new PropertyAccessorDefinitionModel
                    {
                        AccessorType = SyntaxKind.SetAccessorDeclaration,
                        AccessorModifier = SyntaxKind.InternalKeyword
                    }
                },
                Name = _metadataGenerationService.PrimaryEntityPluralizedName,
                ReturnType = _metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.EntityRepositoryInterface]
            }
        };

        protected override IEnumerable<ConstructorDefinitionModel> ConstructorsBuilder
        {
            get
            {
                return new ConstructorDefinitionModel[]
                {
                    new ConstructorDefinitionModel
                    {
                        CallBaseConstructor = true,
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = _unitOfWorkRequirements.DbContextClass.Name,
                                Name = $"{nameof(CommonTokens.Context).ToLowerCamelCase()}"
                            },
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{CommonTokens.IServiceProvider}",
                                Name = $"{nameof(CommonTokens.ServiceProvider).ToLowerCamelCase()}"
                            }
                        },
                    }
                };
            }
        }
    }
}
