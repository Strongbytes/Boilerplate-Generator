using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.CommandsInputModels
{
    public class CreateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public CreateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }

        public override bool CanBeCreated => _viewModelBase.CreateCommandIsEnabled;

        public override AssetKind Kind => AssetKind.CreateRequestDomainEntity;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.UsingsBuilder);

        protected override IEnumerable<PropertyDefinitionModel> DefinedPropertiesBuilder => _viewModelBase.EntityTree.First()
                                                                                                                     .FilterTreeProperties()
                                                                                                                     .Where(x => !x.IsPrimaryKey);
    }
}
