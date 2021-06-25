using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.UnitOfWork;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
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
        private readonly IUnitOfWorkRequirements _unitOfWorkRequirements;

        protected BaseMediatorHandlerGeneratorModel
        (
            IViewModelBase viewModelBase, 
            IMetadataGenerationService metadataGenerationService,
            IUnitOfWorkRequirements unitOfWorkRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _unitOfWorkRequirements = unitOfWorkRequirements;
        }

        public override bool MergeWithExistingAsset => true;

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
           _viewModelBase.UseUnitOfWork 
            ? _metadataGenerationService.NamespaceByAssetKind(AssetKind.UnitOfWorkInterface)
            : _unitOfWorkRequirements.DbContextClass.Namespace
        }.Union(base.UsingsBuilder);

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            AccessModifier = SyntaxKind.InternalKeyword,
            DefinedInheritanceTypes = new EntityClassWrapper[]
            {
                new EntityClassWrapper($"{CommonTokens.IRequestHandler}<{RequestHandlerClassName}, {HandlerResponseType}>")
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> InjectedDependenciesBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UnitOfWorkInterface]}",
                Name = $"{nameof(CommonTokens.UnitOfWork).ToLowerCamelCase()}",
                IsEnabled = _viewModelBase.UseUnitOfWork
            },
            new ParameterDefinitionModel
            {
                ReturnType = _unitOfWorkRequirements.DbContextClass.Name,
                Name = $"{nameof(CommonTokens.Context).ToLowerCamelCase()}",
                IsEnabled = !_viewModelBase.UseUnitOfWork
            },
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IMapper}",
                Name = $"{nameof(CommonTokens.Mapper).ToLowerCamelCase()}"
            }
        };

        protected override IEnumerable<MethodDefinitionModel> DefinedMethodsBuilder
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
