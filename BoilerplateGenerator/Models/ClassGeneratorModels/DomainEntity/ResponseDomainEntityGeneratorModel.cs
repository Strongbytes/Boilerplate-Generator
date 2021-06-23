using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.DomainEntity
{
    public class ResponseDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public ResponseDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.UsingsBuilder);

        public override AssetKind Kind => AssetKind.ResponseDomainEntity;

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => _viewModelBase.EntityTree.First()
                                                                                                            .FilterTreeProperties();
    }
}
