using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.RoslynWrappers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace BoilerplateGenerator.Domain
{
    public interface IViewModelBase : INotifyPropertyChanged
    {
        Visibility LoaderVisibility { get; set; }

        ProjectWrapper SelectedProject { get; set; }

        ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; }

        string ReferencedEntityName { get; set; }

        void ResetInterface();

        Task PopulateSolutionProjects();

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "");
    }
}