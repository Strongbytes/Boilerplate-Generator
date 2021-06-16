using BoilerplateGenerator.Models.RoslynWrappers;
using System.ComponentModel;

namespace BoilerplateGenerator.Models.Pagination
{
    public interface IPaginationRequirements : INotifyPropertyChanged
    {
        bool? PaginationIsAvailable { get; }

        EntityInterfaceWrapper PaginatedDataQueryInterface { get; set; }

        EntityClassWrapper PaginatedDataQueryClass { get; set; }

        EntityInterfaceWrapper PaginatedDataResponseInterface { get; set; }

        EntityClassWrapper PaginatedDataResponseClass { get; set; }

        void LoadComplete();
    }
}
