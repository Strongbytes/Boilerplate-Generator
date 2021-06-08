using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.CommandsInputModels
{
    public class UpdateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.UpdateRequestDomainEntity;

        public override IEnumerable<string> Usings => new List<string>
        {
           nameof(System.ComponentModel.DataAnnotations),
        }.Union(base.Usings);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);

        public UpdateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
