﻿using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.CommandsInputModels
{
    public class UpdateRequestDomainEntityGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public UpdateRequestDomainEntityGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }

        public override bool CanBeCreated => _viewModelBase.UpdateCommandIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.UpdateRequestDomainEntity;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.SystemComponentModelDataAnnotations,
        }.Union(base.UsingsBuilder);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => base.AvailableProperties.Where(x => !x.IsPrimaryKey);
    }
}
