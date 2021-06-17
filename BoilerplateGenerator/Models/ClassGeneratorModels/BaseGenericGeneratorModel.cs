using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public abstract class BaseGenericGeneratorModel : IGenericGeneratorModel
    {
        private readonly object _locker = new object();
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        protected BaseGenericGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        public virtual bool CanBeCreated => true;

        protected virtual IEnumerable<string> UsingsBuilder => new string[] { UsingTokens.System };

        public IEnumerable<string> Usings => UsingsBuilder.Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x);

        public string ClassNamespace => _metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind);

        public string GeneratedClassName => _metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind];

        public virtual IEnumerable<string> BaseTypes { get; } = new string[] { };

        private IEnumerable<PropertyDefinitionModel> _availableProperties;
        public virtual IEnumerable<PropertyDefinitionModel> AvailableProperties
        {
            get
            {
                lock (_locker)
                {
                    if (_availableProperties != null)
                    {
                        return _availableProperties;
                    }

                    _availableProperties = _viewModelBase.EntityTree.First().FilterTreeProperties();
                    return _availableProperties;
                }
            }
        }

        private PropertyDefinitionModel _baseEntityPrimaryKey;
        protected PropertyDefinitionModel BaseEntityPrimaryKey
        {
            get
            {
                lock (_locker)
                {
                    if (_baseEntityPrimaryKey != null)
                    {
                        return _baseEntityPrimaryKey;
                    }

                    _baseEntityPrimaryKey = _viewModelBase.EntityTree.First().FilterTreeProperties().FirstOrDefault(x => x.IsPrimaryKey) ?? new PropertyDefinitionModel
                    {
                        Name = $"{CommonTokens.Id}",
                        IsPrimaryKey = true,
                        ReturnType = "int"
                    };

                    return _baseEntityPrimaryKey;
                }
            }
        }

        public virtual SyntaxKind RootClassModifier => SyntaxKind.PublicKeyword;

        protected virtual IProjectWrapper TargetModule => _viewModelBase.SelectedTargetModuleProject;

        public string TargetProjectName => TargetModule.Name;

        public abstract AssetKind GeneratedClassKind { get; }

        public virtual bool MergeWithExistingClass { get; }

        public bool FileExistsInProject => TargetModule.GeneratedFileAlreadyExists($"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}", $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}");

        public async Task ExportFile(string content)
        {
            await TargetModule.ExportFile
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}",
                $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}",
                content
            );
        }

        public virtual IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; } = Enumerable.Empty<ParameterDefinitionModel>();

        public virtual IEnumerable<MethodDefinitionModel> Constructors { get; } = Enumerable.Empty<MethodDefinitionModel>();

        public virtual IEnumerable<MethodDefinitionModel> AvailableMethods { get; } = Enumerable.Empty<MethodDefinitionModel>();

        public virtual IEnumerable<AttributeDefinitionModel> Attributes { get; } = Enumerable.Empty<AttributeDefinitionModel>();

        public async Task<CompilationUnitSyntax> LoadClassFromExistingFile()
        {
            return await TargetModule.GetExistingFileClass
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}",
                $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}"
            );
        }
    }
}
