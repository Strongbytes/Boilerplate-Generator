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


        #region Model Builder
        public abstract AssetKind GeneratedClassKind { get; }

        public virtual bool CanBeCreated => true;

        public virtual bool MergeWithExistingClass => false;

        public virtual SyntaxKind RootClassModifier => SyntaxKind.PublicKeyword;

        protected virtual IEnumerable<string> UsingsBuilder => new string[] { UsingTokens.System };

        private IEnumerable<PropertyDefinitionModel> _availablePropertiesBuilder;
        protected virtual IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder
        {
            get
            {
                lock (_locker)
                {
                    if (_availablePropertiesBuilder != null)
                    {
                        return _availablePropertiesBuilder;
                    }

                    _availablePropertiesBuilder = _viewModelBase.EntityTree.First().FilterTreeProperties();
                    return _availablePropertiesBuilder;
                }
            }
        }

        protected virtual IEnumerable<string> BaseTypesBuilder { get; } = Enumerable.Empty<string>();

        protected virtual IEnumerable<AttributeDefinitionModel> AttributesBuilder { get; } = Enumerable.Empty<AttributeDefinitionModel>();

        protected virtual IEnumerable<ParameterDefinitionModel> ConstructorParametersBuilder { get; } = Enumerable.Empty<ParameterDefinitionModel>();

        protected virtual IEnumerable<MethodDefinitionModel> ConstructorsBuilder { get; } = Enumerable.Empty<MethodDefinitionModel>();

        protected virtual IEnumerable<MethodDefinitionModel> AvailableMethodsBuilder { get; } = Enumerable.Empty<MethodDefinitionModel>();
        #endregion

        #region Public Model Properties
        private string _classNamespace;
        public string ClassNamespace
        {
            get
            {
                if (_classNamespace != null)
                {
                    return _classNamespace;
                }

                _classNamespace = _metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind);
                return _classNamespace;
            }
        }

        private string _generatedClassName;
        public string GeneratedClassName
        {
            get
            {
                if (_generatedClassName != null)
                {
                    return _generatedClassName;
                }

                _generatedClassName = _metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind];
                return _generatedClassName;
            }
        }

        public bool FileExistsInProject => TargetModule.GeneratedFileAlreadyExists($"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}", $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}");


        public string TargetProjectName => TargetModule.Name;

        private IEnumerable<string> _usings;
        public IEnumerable<string> Usings
        {
            get
            {
                if (_usings != null)
                {
                    return _usings;
                }

                _usings = UsingsBuilder.Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToArray();
                return _usings;
            }
        }

        private IEnumerable<PropertyDefinitionModel> _availableProperties;
        public IEnumerable<PropertyDefinitionModel> AvailableProperties
        {
            get
            {
                if (_availableProperties != null)
                {
                    return _availableProperties;
                }

                _availableProperties = AvailablePropertiesBuilder.ToArray();
                return _availableProperties;
            }
        }

        private IEnumerable<string> _baseTypes;
        public IEnumerable<string> BaseTypes
        {
            get
            {
                if (_baseTypes != null)
                {
                    return _baseTypes;
                }

                _baseTypes = BaseTypesBuilder.ToArray();
                return _baseTypes;
            }
        }

        private IEnumerable<ParameterDefinitionModel> _constructorParameters;
        public IEnumerable<ParameterDefinitionModel> ConstructorParameters
        {
            get
            {
                if (_constructorParameters != null)
                {
                    return _constructorParameters;
                }

                _constructorParameters = ConstructorParametersBuilder.ToArray();
                return _constructorParameters;
            }
        }

        private IEnumerable<MethodDefinitionModel> _constructors;
        public IEnumerable<MethodDefinitionModel> Constructors
        {
            get
            {
                if (_constructors != null)
                {
                    return _constructors;
                }

                _constructors = ConstructorsBuilder.ToArray();
                return _constructors;
            }
        }

        private IEnumerable<MethodDefinitionModel> _availableMethods;
        public IEnumerable<MethodDefinitionModel> AvailableMethods
        {
            get
            {
                if (_availableMethods != null)
                {
                    return _availableMethods;
                }

                _availableMethods = AvailableMethodsBuilder.ToArray();
                return _availableMethods;
            }
        }


        private IEnumerable<AttributeDefinitionModel> _attributes;
        public IEnumerable<AttributeDefinitionModel> Attributes
        {
            get
            {
                if (_attributes != null)
                {
                    return _attributes;
                }

                _attributes = AttributesBuilder.ToArray();
                return _attributes;
            }
        }
        #endregion

        #region Internal Properties

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

        protected virtual IProjectWrapper TargetModule => _viewModelBase.SelectedTargetModuleProject;
        #endregion

        #region Methods
        public async Task ExportFile(string content)
        {
            await TargetModule.ExportFile
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}",
                $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}",
                content
            );
        }

        public async Task<CompilationUnitSyntax> LoadClassFromExistingFile()
        {
            return await TargetModule.GetExistingFileClass
            (
                $"{_metadataGenerationService.NamespaceByAssetKind(GeneratedClassKind)}",
                $"{_metadataGenerationService.AssetToClassNameMapping[GeneratedClassKind]}"
            );
        }
        #endregion
    }
}
