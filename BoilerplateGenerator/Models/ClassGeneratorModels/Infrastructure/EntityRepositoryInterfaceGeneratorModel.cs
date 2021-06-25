using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class EntityRepositoryInterfaceGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        public EntityRepositoryInterfaceGeneratorModel
        (
            IViewModelBase viewModelBase, 
            IMetadataGenerationService metadataGenerationService,
            IUnitOfWorkRequirements unitOfWorkRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _unitOfWorkRequirements = unitOfWorkRequirements;
        }

        public override bool CanBeCreated => _viewModelBase.UseUnitOfWork;

        public override bool MergeWithExistingAsset => true;

        public override AssetKind Kind => AssetKind.EntityRepositoryInterface;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           _unitOfWorkRequirements.BaseRepositoryInterface.Namespace,
           $"{_viewModelBase.EntityTree.PrimaryEntityNamespace()}"
        };

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            Type = SyntaxKind.InterfaceDeclaration,
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new EntityClassWrapper[]
            {
                new EntityClassWrapper($"{CommonTokens.IBaseRepository}<{_viewModelBase.EntityTree.PrimaryEntityType()}>")
            }
        };
    }
}
