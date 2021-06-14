using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
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
            { AssetKind.GetByIdQueryHandler, AssetKind.GetByIdQuery },
            { AssetKind.CreateCommandHandler, AssetKind.CreateCommand },
            { AssetKind.DeleteCommandHandler, AssetKind.DeleteCommand },
            { AssetKind.UpdateCommandHandler, AssetKind.UpdateCommand },
        };

        protected virtual string HandlerResponseType => $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}";

        public override IEnumerable<string> Usings => new List<string>
        {
           UsingTokens.AutoMapper,
           UsingTokens.MediatR,
           UsingTokens.SystemThreading,
           UsingTokens.SystemThreadingTasks,
           UsingTokens.SystemCollectionsGeneric,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
        }.Union(base.Usings).OrderBy(x => x);

        public override IEnumerable<string> BaseTypes => new string[]
        {
            $"{CommonTokens.IRequestHandler}<{RequestHandlerClassName}, {HandlerResponseType}>"
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
                        ReturnType = $"{CommonTokens.Task}<{HandlerResponseType}>",
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
