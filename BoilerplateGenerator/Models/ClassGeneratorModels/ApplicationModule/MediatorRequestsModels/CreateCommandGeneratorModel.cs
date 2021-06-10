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
    public class CreateCommandGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public CreateCommandGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override AssetKind GeneratedClassKind => AssetKind.CreateCommand;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.MediatR,
           _metadataGenerationService.AssetToNamespaceMapping[AssetKind.ResponseDomainEntity],
           _metadataGenerationService.AssetToNamespaceMapping[AssetKind.CreateRequestDomainEntity],
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity]}",
                Name = $"{CommonTokens.Model}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity]}",
                Name = $"{nameof(CommonTokens.Model).ToLowerCamelCase()}",
                MapToClassProperty = true
            }
        };
    }
}
