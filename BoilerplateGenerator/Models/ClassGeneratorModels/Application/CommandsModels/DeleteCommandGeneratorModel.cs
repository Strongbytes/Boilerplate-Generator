
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class DeleteCommandGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.DeleteCommand;

        public override IEnumerable<string> Usings => new List<string>
        {
           "MediatR",
        }.Union(base.Usings);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<Unit>"
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
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}",
                MapToClassProperty = true
            }
        };

        public DeleteCommandGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
