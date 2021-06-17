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
    public class UpdateCommandGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        public UpdateCommandGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool CanBeCreated => _viewModelBase.UpdateCommandIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.UpdateCommand;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UpdateRequestDomainEntity),
        }.Union(base.UsingsBuilder);

        protected override IEnumerable<string> BaseTypesBuilder => new string[]
        {
            $"{CommonTokens.IRequest}<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>"
        };

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity]}",
                Name = $"{CommonTokens.Model}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            },
            new PropertyDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> ConstructorParametersBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}",
                MapToClassProperty = true
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity]}",
                Name = $"{nameof(CommonTokens.Model).ToLowerCamelCase()}",
                MapToClassProperty = true
            }
        };
    }
}
