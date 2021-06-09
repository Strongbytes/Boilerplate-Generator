using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoilerplateGenerator.Collections
{
    public class TreeNode<T> : ITreeNode<T> where T : INotifyPropertyChanged
    {
        public ObservableCollection<ITreeNode<T>> Children { get; set; } = new ObservableCollection<ITreeNode<T>>();

        private T _current;
        public virtual T Current
        {
            get { return _current; }
            set
            {
                _current = value;
                NotifyPropertyChanged();
            }
        }

        private ITreeNode<T> _parent;
        public ITreeNode<T> Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                NotifyPropertyChanged();
            }
        }

        public void Clear()
        {
            Children.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
