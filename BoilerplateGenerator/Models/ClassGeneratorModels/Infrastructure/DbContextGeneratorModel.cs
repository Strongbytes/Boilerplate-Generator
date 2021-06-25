using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System;
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

        protected override IProjectWrapper TargetModule => _viewModelBase.AvailableModules.FirstOrDefault(x => _unitOfWorkRequirements.DbContextClass.Namespace.Contains(x.Namespace))
                                                        ?? throw new NullReferenceException("Unable to load DbContext's containing Module. Make sure the Solution has a DbContext and restart extension.");

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
