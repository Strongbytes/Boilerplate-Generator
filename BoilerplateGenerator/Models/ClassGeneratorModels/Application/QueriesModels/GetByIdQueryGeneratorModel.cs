using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class GetByIdQueryGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetByIdQueryGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override AssetKind GeneratedClassKind => AssetKind.GetByIdQuery;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.MediatR,
           _metadataGenerationService.AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}",
                MapToClassProperty = true
            },
        };
    }
}
