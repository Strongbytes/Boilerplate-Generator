using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class CreateCommandGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.CreateCommand;

        public override IEnumerable<string> Usings => new List<string>
        {
           "MediatR",
           AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
           AssetToNamespaceMapping[AssetKind.CreateRequestDomainEntity],
        }.Union(base.Usings);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] 
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity]}",
                Name = $"{CommonTokens.Model}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity]}",
                Name = "model",
                MapToClassProperty = true
            }
        };

        public CreateCommandGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
