using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class UnitOfWorkInterfaceGeneralModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        public UnitOfWorkInterfaceGeneralModel
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

        public override AssetKind Kind => AssetKind.UnitOfWorkInterface;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           _unitOfWorkRequirements.BaseUnitOfWorkInterface.Namespace,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.EntityRepositoryInterface)
        };

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            Type = SyntaxKind.InterfaceDeclaration,
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.IBaseUnitOfWork}"
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> DefinedPropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                Modifiers = Enumerable.Empty<SyntaxKind>(),
                Accessors = new PropertyAccessorDefinitionModel[] 
                {
                    new PropertyAccessorDefinitionModel
                    {
                        AccessorType = SyntaxKind.GetAccessorDeclaration
                    }
                },
                Name = _metadataGenerationService.PrimaryEntityPluralizedName,
                ReturnType = _metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.EntityRepositoryInterface]
            }
        };
    }
}
