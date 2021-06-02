using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BoilerplateGenerator.Collections
{
    public interface ITreeNode<T> : INotifyPropertyChanged
    {
        ObservableCollection<ITreeNode<T>> Children { get; set; }

        T Current { get; set; }

        ITreeNode<T> Parent { get; set; }
    }
}
