using BoilerplateGenerator.ClassGeneratorModels;
using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Contracts;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public abstract class GenericGeneratorModel : IGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public virtual IEnumerable<string> Usings => new List<string>
        {
            nameof(System)
        };

        public abstract string RootClassName { get; }

        public virtual IEnumerable<string> BaseTypes { get; } = new string[] { };

        public virtual IEnumerable<EntityPropertyWrapper> AvailableProperties => GetPropertiesFromTree(_viewModelBase.EntityTree.First());

        protected EntityClassWrapper RootClass => _viewModelBase.EntityTree.First().Current as EntityClassWrapper;

        public virtual SyntaxKind RootClassModifier => SyntaxKind.PublicKeyword;

        public string TargetProjectName => _viewModelBase.SelectedProject.Name;

        public virtual string Namespace => _viewModelBase.SelectedProject.Namespace;

        public abstract AssetKind AssetKind { get; }

        public virtual KeyValuePair<string, string>[] ConstructorParameters { get; } = new KeyValuePair<string, string>[] { };

        public virtual IEnumerable<MethodDefinitionModel> AvailableMethods { get; } = new MethodDefinitionModel[] { };

        protected GenericGeneratorModel(IViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        private IEnumerable<EntityPropertyWrapper> GetPropertiesFromTree(ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            List<EntityPropertyWrapper> symbols = new List<EntityPropertyWrapper>();

            foreach (ITreeNode<IBaseSymbolWrapper> treeNode in rootNode.Children)
            {
                switch (treeNode.Current.GetType().Name)
                {
                    case nameof(EntityClassWrapper):
                        symbols.AddRange(GetPropertiesFromTree(treeNode));
                        break;

                    default:
                        symbols.Add(treeNode.Current as EntityPropertyWrapper);
                        break;
                }
            }

            return symbols.Where(x => x.IsChecked);
        }
    }
}
