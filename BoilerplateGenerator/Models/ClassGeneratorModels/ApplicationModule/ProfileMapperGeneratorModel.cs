using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule
{
    public class ProfileMapperGeneratorModel : BaseGenericGeneratorModel
    {
        public ProfileMapperGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        public override AssetKind GeneratedClassKind => AssetKind.ProfileMapper;

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);
    }
}
