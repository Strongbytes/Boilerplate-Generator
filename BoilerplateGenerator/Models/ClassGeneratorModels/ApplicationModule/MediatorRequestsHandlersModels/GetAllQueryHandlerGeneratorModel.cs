﻿using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.ViewModels;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public class GetAllQueryHandlerGeneratorModel : BaseMediatorHandlerGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public GetAllQueryHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override AssetKind GeneratedClassKind => AssetKind.GetAllQueryHandler;

        protected override string HandlerResponseType => $"IEnumerable<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>";
    }
}