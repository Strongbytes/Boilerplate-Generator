﻿using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Application.QueriesModels
{
    public abstract class BaseMediatorHandlerGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        protected BaseMediatorHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        private string RequestHandlerClassName => _metadataGenerationService.AssetToClassNameMapping[AssetToMediatorRequestKind[GeneratedClassKind]];

        protected IDictionary<AssetKind, AssetKind> AssetToMediatorRequestKind => new Dictionary<AssetKind, AssetKind>
        {
            { AssetKind.GetAllQueryHandler, AssetKind.GetAllQuery },
            { AssetKind.GetByIdQueryHandler, AssetKind.GetByIdQuery }
        };

        protected abstract string HandlerResponseType { get; }

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.AutoMapper,
           UsingTokens.MediatR,
           UsingTokens.SystemThreading,
           UsingTokens.SystemCollectionsGeneric,
           _metadataGenerationService.AssetToNamespaceMapping[AssetKind.ResponseEntityDomainModel],
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"IRequestHandler<{RequestHandlerClassName}, {HandlerResponseType}>"
        };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IUnitOfWork}",
                Name = $"{nameof(CommonTokens.UnitOfWork).ToLowerCamelCase()}"
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IMapper}",
                Name = $"{nameof(CommonTokens.Mapper).ToLowerCamelCase()}"
            }
        };

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };

        public override IEnumerable<MethodDefinitionModel> AvailableMethods
        {
            get
            {
                return new MethodDefinitionModel[]
                {
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Handle}",
                        ReturnType = $"Task<{HandlerResponseType}>",
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{RequestHandlerClassName}",
                                Name = $"{nameof(CommonTokens.Request).ToLowerCamelCase()}"
                            },
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{CommonTokens.CancellationToken}",
                                Name = $"{nameof(CommonTokens.CancellationToken).ToLowerCamelCase()}"
                            }
                        },
                    }
                };
            }
        }
    }
}
