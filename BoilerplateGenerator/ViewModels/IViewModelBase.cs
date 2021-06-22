using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Models.Pagination;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ViewModels
{
    public interface IViewModelBase : INotifyPropertyChanged
    {
        bool UseUnitOfWork { get; set; }

        bool GenerateAutoMapperProfile { get; set; }

        bool GetByIdQueryIsEnabled { get; set; }

        bool GetAllQueryIsEnabled { get; set; }

        bool GetPaginatedQueryIsEnabled { get; set; }

        bool CreateCommandIsEnabled { get; set; }

        bool UpdateCommandIsEnabled { get; set; }

        bool DeleteCommandIsEnabled { get; set; }

        IProjectWrapper SelectedTargetModuleProject { get; set; }

        IProjectWrapper SelectedControllersProject { get; set; }

        IPaginationRequirements PaginationRequirements { get; }

        ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; }

        Task PopulateUiData();
    }
}