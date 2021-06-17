using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.DomainEntity
{
    public class ResponseDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        public ResponseDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
        }

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.UsingsBuilder);

        public override AssetKind GeneratedClassKind => AssetKind.ResponseDomainEntity;
    }
}
