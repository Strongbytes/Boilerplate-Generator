using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.CommandsInputModels
{
    public class UpdateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public UpdateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService) 
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.UpdateRequestDomainEntity;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);
    }
}
