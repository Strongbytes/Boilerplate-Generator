using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace BoilerplateGenerator.ViewModels
{
    public interface IViewModelBase : INotifyPropertyChanged
    {
        Visibility LoaderVisibility { get; set; }

        IProjectWrapper SelectedTargetModuleProject { get; set; }

        IProjectWrapper SelectedControllersProject { get; set; }

        ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; }

        string ReferencedEntityName { get; set; }

        void ResetInterface();

        Task PopulateSolutionProjects();

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "");
    }
}