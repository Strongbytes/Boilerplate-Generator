using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.CommandsInputModels
{
    public class CreateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public override AssetKind GeneratedClassKind => AssetKind.CreateRequestDomainEntity;

        public override IEnumerable<string> Usings => new List<string>
        {
           nameof(System.ComponentModel.DataAnnotations),
        }.Union(base.Usings);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);

        public CreateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
