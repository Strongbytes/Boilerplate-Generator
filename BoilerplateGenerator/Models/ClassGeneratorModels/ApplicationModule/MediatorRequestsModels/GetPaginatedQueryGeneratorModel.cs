using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.Pagination;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsModels
{
    public class GetPaginatedQueryGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;
        private readonly IPaginationRequirements _paginationRequirements;

        public GetPaginatedQueryGeneratorModel
        (
            IViewModelBase viewModelBase, 
            IMetadataGenerationService metadataGenerationService, 
            IPaginationRequirements paginationRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _paginationRequirements = paginationRequirements;
        }

        public override bool CanBeCreated => _viewModelBase.GetPaginatedQueryIsEnabled;

        public override AssetKind Kind => AssetKind.GetPaginatedQuery;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
           _paginationRequirements.PaginatedDataResponseInterface.Namespace,
           _paginationRequirements.PaginatedDataQueryInterface.Namespace,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.IRequest}<{CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>>"
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> DefinedPropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = _paginationRequirements.PaginatedDataQueryInterface.Name,
                Name = _paginationRequirements.PaginatedDataQueryClass.Name,
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword },
                Accessors = new PropertyAccessorDefinitionModel[]
                {
                    new PropertyAccessorDefinitionModel
                    {
                        AccessorType = SyntaxKind.GetAccessorDeclaration
                    }
                },
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> InjectedDependenciesBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = _paginationRequirements.PaginatedDataQueryInterface.Name,
                Name = _paginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase(),
                MapToClassProperty = true,
                ThrowExceptionWhenNull = true
            },
        };
    }
}
