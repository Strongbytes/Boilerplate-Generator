using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
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

        public GetPaginatedQueryGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool CanBeCreated => _viewModelBase.GetPaginatedQueryIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.GetPaginatedQuery;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
           _viewModelBase.PaginationRequirements.PaginatedDataResponseInterface.Namespace,
           _viewModelBase.PaginationRequirements.PaginatedDataQueryInterface.Namespace,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
        }.Union(base.UsingsBuilder);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"{CommonTokens.IRequest}<{CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = _viewModelBase.PaginationRequirements.PaginatedDataQueryInterface.Name,
                Name = _viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Name,
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = _viewModelBase.PaginationRequirements.PaginatedDataQueryInterface.Name,
                Name = _viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase(),
                MapToClassProperty = true
            },
        };
    }
}
