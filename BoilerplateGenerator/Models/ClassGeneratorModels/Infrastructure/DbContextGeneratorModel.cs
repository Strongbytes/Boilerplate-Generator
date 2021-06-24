using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class DbContextGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        public DbContextGeneratorModel
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

        public override bool MergeWithExistingAsset => true;

        public override AssetKind Kind => AssetKind.DbContext;

        protected override IProjectWrapper TargetModule => _viewModelBase.AvailableModules.First(x => x.Name == _unitOfWorkRequirements.DbContextClass.ContainingModuleName);

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           $"{_viewModelBase.EntityTree.PrimaryEntityNamespace()}",
        };

        protected override IEnumerable<PropertyDefinitionModel> DefinedPropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{CommonTokens.DbSet}<{_viewModelBase.EntityTree.PrimaryEntityType()}>",
                Name = $"{_metadataGenerationService.PrimaryEntityPluralizedName}"
            }
        };
    }
}
