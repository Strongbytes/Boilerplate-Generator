using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class UpdateCommandGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.UpdateCommand;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.MediatR,
           AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
           AssetToNamespaceMapping[AssetKind.UpdateRequestDomainEntity],
        }.Union(base.Usings);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] 
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity]}",
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

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}",
                MapToClassProperty = true
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity]}",
                Name = $"{nameof(CommonTokens.Model).ToLowerInvariant()}",
                MapToClassProperty = true
            }
        };

        public UpdateCommandGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
