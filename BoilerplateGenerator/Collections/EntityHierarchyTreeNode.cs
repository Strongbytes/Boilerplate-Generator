using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Extensions;
using System.ComponentModel;

namespace BoilerplateGenerator.Collections
{
    public class EntityHierarchyTreeNode : TreeNode<IBaseSymbolWrapper>
    {
        private void OnCurrentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IBaseSymbolWrapper symbolWrapper) || symbolWrapper.IsPropertyChanging)
            {
                return;
            }

            this.SetChildrenSelection(symbolWrapper.IsChecked ?? false);
            this.SetParentsSelection();
        }

        ~EntityHierarchyTreeNode()
        {
            if (_current == null)
            {
                return;
            }

            _current.PropertyChanged -= OnCurrentItemPropertyChanged;
        }

        private IBaseSymbolWrapper _current;
        public override IBaseSymbolWrapper Current
        {
            get { return _current; }
            set
            {
                if (_current != null)
                {
                    _current.PropertyChanged -= OnCurrentItemPropertyChanged;
                }

                _current = value;
                _current.PropertyChanged += OnCurrentItemPropertyChanged;
                NotifyPropertyChanged();
            }
        }
    }
}
