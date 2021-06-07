using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class ResponseEntityDomainModelGeneratorModel : BaseGenericGeneratorModel
    {
        public override string Namespace => $"{base.Namespace}.Domain.Models";

        public override IEnumerable<string> Usings => new List<string>
        {
           nameof(System.ComponentModel.DataAnnotations),
        }.Union(base.Usings);

        public override AssetKind AssetKind => AssetKind.ResponseEntityDomainModel;

        public ResponseEntityDomainModelGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
