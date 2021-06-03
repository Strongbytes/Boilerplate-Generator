using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ClassGeneratorModels
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

        public virtual IEnumerable<EntityPropertyWrapper> AvailableProperties => GetNodeChildren(_viewModelBase.EntityTree.First());

        protected EntityClassWrapper RootClass => _viewModelBase.EntityTree.First().Current as EntityClassWrapper;

        public virtual SyntaxKind RootClassModifier => SyntaxKind.PublicKeyword;

        public string Namespace => "Test";

        protected GenericGeneratorModel(IViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        private IEnumerable<EntityPropertyWrapper> GetNodeChildren(ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            List<EntityPropertyWrapper> symbols = new List<EntityPropertyWrapper>();

            foreach (ITreeNode<IBaseSymbolWrapper> treeNode in rootNode.Children)
            {
                switch (treeNode.Current.GetType().Name)
                {
                    case nameof(EntityClassWrapper):
                        symbols.AddRange(GetNodeChildren(treeNode));
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
