using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Infrastructure
{
    public class DependencyContainerRegistrationGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IMetadataGenerationService _metadataGenerationService;

        public DependencyContainerRegistrationGeneratorModel
        (
            IViewModelBase viewModelBase,
            IMetadataGenerationService metadataGenerationService
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _metadataGenerationService = metadataGenerationService;
        }

        public override bool MergeWithExistingAsset => true;

        public override AssetKind Kind => AssetKind.DependencyContainerRegistration;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
            UsingTokens.Autofac,
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.EntityRepositoryImplementation),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.EntityRepositoryInterface),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UnitOfWorkImplementation),
           _metadataGenerationService.NamespaceByAssetKind(AssetKind.UnitOfWorkInterface),
        };

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[]
            {
                "Autofac.Module"
            }
        };

        protected override IEnumerable<MethodDefinitionModel> DefinedMethodsBuilder => new MethodDefinitionModel[]
        {
            new MethodDefinitionModel
            {
                Name = $"{CommonTokens.Load}",
                Modifiers = new SyntaxKind[] { SyntaxKind.ProtectedKeyword, SyntaxKind.OverrideKeyword },
                ReturnType = $"{nameof(CommonTokens.Void).ToLowerCamelCase()}",
                Parameters = new ParameterDefinitionModel[]
                {
                    new ParameterDefinitionModel
                    {
                        Name = $"{nameof(CommonTokens.Builder).ToLowerCamelCase()}",
                        ReturnType = $"{CommonTokens.ContainerBuilder}",
                    }
                },
                Body = GetLoadBody
            },
            new MethodDefinitionModel
            {
                Name = $"{CommonTokens.RegisterUnitOfWorkRepositories}",
                Modifiers = new SyntaxKind[] { SyntaxKind.PrivateKeyword },
                ReturnType = $"{nameof(CommonTokens.Void).ToLowerCamelCase()}",
                Parameters = new ParameterDefinitionModel[]
                {
                    new ParameterDefinitionModel
                    {
                        Name = $"{nameof(CommonTokens.Builder).ToLowerCamelCase()}",
                        ReturnType = $"{CommonTokens.ContainerBuilder}",
                    }
                },
                Body = GetRegisterUnitOfWorkRepositoriesBody
            },
        };

        public IEnumerable<string> GetRegisterUnitOfWorkRepositoriesBody => new string[]
        {
            $@"builder.RegisterType<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.EntityRepositoryImplementation]}>()
                        .As<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.EntityRepositoryInterface]}>()
                        .InstancePerLifetimeScope();",
            $@"builder.RegisterType<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UnitOfWorkImplementation]}>()
                        .As<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UnitOfWorkInterface]}>()
                        .InstancePerLifetimeScope();",
        };

        public IEnumerable<string> GetLoadBody => new string[]
        {
            $"base.Load({nameof(CommonTokens.Builder).ToLowerCamelCase()});",
            $"{CommonTokens.RegisterUnitOfWorkRepositories}({nameof(CommonTokens.Builder).ToLowerCamelCase()});",
        };
    }
}
