using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsHandlersModels
{
    public abstract class BaseMediatorHandlerGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        protected BaseMediatorHandlerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        private string RequestHandlerClassName => _metadataGenerationService.AssetToCompilationUnitNameMapping[AssetToMediatorRequestKind[Kind]];

        protected IDictionary<AssetKind, AssetKind> AssetToMediatorRequestKind => new Dictionary<AssetKind, AssetKind>
        {
            { AssetKind.GetAllQueryHandler, AssetKind.GetAllQuery },
            { AssetKind.GetByIdQueryHandler, AssetKind.GetByIdQuery },
            { AssetKind.GetPaginatedQueryHandler, AssetKind.GetPaginatedQuery },
            { AssetKind.CreateCommandHandler, AssetKind.CreateCommand },
            { AssetKind.DeleteCommandHandler, AssetKind.DeleteCommand },
            { AssetKind.UpdateCommandHandler, AssetKind.UpdateCommand },
        };

        protected virtual string HandlerResponseType => $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}";

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.AutoMapper,
           UsingTokens.MediatR,
           UsingTokens.SystemThreading,
           UsingTokens.SystemThreadingTasks,
           UsingTokens.SystemCollectionsGeneric,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.ResponseDomainEntity),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.IUnitOfWork),
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new string[]
            {
                $"{CommonTokens.IRequestHandler}<{RequestHandlerClassName}, {HandlerResponseType}>"
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> InjectedDependenciesBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IUnitOfWork}",
                Name = $"{nameof(CommonTokens.UnitOfWork).ToLowerCamelCase()}",
                IsEnabled = _viewModelBase.UseUnitOfWork
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IMapper}",
                Name = $"{nameof(CommonTokens.Mapper).ToLowerCamelCase()}"
            }
        };

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => new PropertyDefinitionModel[] { };

        protected override IEnumerable<MethodDefinitionModel> AvailableMethodsBuilder
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
