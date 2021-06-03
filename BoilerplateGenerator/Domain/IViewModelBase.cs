using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Models;
using System.Collections.Generic;
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

        ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; }

        string ReferencedEntityName { get; set; }

        void ResetInterface();

        Task PopulateSolutionProjects();

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "");
    }
}