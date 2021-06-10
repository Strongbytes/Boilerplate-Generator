using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

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

        public virtual IEnumerable<string> Usings => new List<string> { UsingTokens.System };

        public string Namespace => _metadataGenerationService.AssetToNamespaceMapping[GeneratedClassKind];

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

                    _availableProperties = FilterTreeProperties(_viewModelBase.EntityTree.First());
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

                    _baseEntityPrimaryKey = FilterTreeProperties(_viewModelBase.EntityTree.First()).FirstOrDefault(x => x.IsPrimaryKey) ?? new PropertyDefinitionModel
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

        public string TargetProjectName => _viewModelBase.SelectedProject.Name;

        public abstract AssetKind GeneratedClassKind { get; }

        public virtual IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; } = Enumerable.Empty<ParameterDefinitionModel>();

        public virtual IEnumerable<MethodDefinitionModel> AvailableMethods { get; } = Enumerable.Empty<MethodDefinitionModel>();

        public virtual IEnumerable<AttributeDefinitionModel> Attributes { get; } = Enumerable.Empty<AttributeDefinitionModel>();

        private IEnumerable<PropertyDefinitionModel> FilterTreeProperties(ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            List<PropertyDefinitionModel> symbols = new List<PropertyDefinitionModel>();

            foreach (ITreeNode<IBaseSymbolWrapper> treeNode in rootNode.Children)
            {
                switch (treeNode.Current.GetType().Name)
                {
                    case nameof(EntityClassWrapper):
                        symbols.AddRange(FilterTreeProperties(treeNode));
                        break;

                    default:
                        if (!(treeNode.Current is EntityPropertyWrapper entityPropertyWrapper))
                        {
                            break;
                        }

                        if (!entityPropertyWrapper.IsChecked.HasValue || !entityPropertyWrapper.IsChecked.Value)
                        {
                            break;
                        }

                        symbols.Add(new PropertyDefinitionModel(entityPropertyWrapper));
                        break;
                }
            }

            return symbols;
        }
    }
}
