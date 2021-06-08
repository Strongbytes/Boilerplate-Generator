using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class GetByIdQueryGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.GetByIdQuery;

        public override IEnumerable<string> Usings => new List<string>
        {
           "MediatR",
           AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
        }.Union(base.Usings);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };

        public GetByIdQueryGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
