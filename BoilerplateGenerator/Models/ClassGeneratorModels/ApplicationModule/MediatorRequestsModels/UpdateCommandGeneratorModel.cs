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

        public override AssetKind Kind => AssetKind.UpdateCommand;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UpdateRequestDomainEntity),
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.IRequest}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>"
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> DefinedPropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UpdateRequestDomainEntity]}",
                Name = $"{CommonTokens.Model}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword },
                Accessors = new PropertyAccessorDefinitionModel[]
                {
                    new PropertyAccessorDefinitionModel
                    {
                        AccessorType = SyntaxKind.GetAccessorDeclaration
                    }
                },
            },
            new PropertyDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name}",
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
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}",
                MapToClassProperty = true
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UpdateRequestDomainEntity]}",
                Name = $"{nameof(CommonTokens.Model).ToLowerCamelCase()}",
                MapToClassProperty = true,
                ThrowExceptionWhenNull = true
            }
        };
    }
}
