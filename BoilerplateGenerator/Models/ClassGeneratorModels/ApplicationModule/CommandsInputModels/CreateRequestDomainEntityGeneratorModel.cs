using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.CommandsInputModels
{
    public class CreateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public CreateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService) 
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.CreateRequestDomainEntity;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);
    }
}
