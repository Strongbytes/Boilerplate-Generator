using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ViewModels
{
    public interface IViewModelBase : INotifyPropertyChanged
    {
        IProjectWrapper SelectedTargetModuleProject { get; set; }

        IProjectWrapper SelectedControllersProject { get; set; }

        ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; }

        Task PopulateSolutionProjects();
    }
}