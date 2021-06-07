using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class UpdateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public override string Namespace => $"{base.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Update.Models";

        public override IEnumerable<string> Usings => new List<string>
        {
           nameof(System.ComponentModel.DataAnnotations),
        }.Union(base.Usings);

        public override AssetKind AssetKind => AssetKind.UpdateRequestDomainEntity;

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);

        public UpdateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
