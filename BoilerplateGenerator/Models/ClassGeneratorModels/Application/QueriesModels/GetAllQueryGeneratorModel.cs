using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public class GetAllQueryGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.GetAllQuery;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.MediatR,
           UsingTokens.SystemCollectionsGeneric,
           AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
        }.Union(base.Usings);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequest<IEnumerable<{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>>"
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };

        public GetAllQueryGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
