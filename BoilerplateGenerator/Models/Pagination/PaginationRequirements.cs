using BoilerplateGenerator.Models.RoslynWrappers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoilerplateGenerator.Models.Pagination
{
    public class PaginationRequirements : IPaginationRequirements
    {
        private bool _loadIsComplete;

        public EntityInterfaceWrapper PaginatedDataQueryInterface { get; set; }

        public EntityClassWrapper PaginatedDataQueryClass { get; set; }

        public EntityInterfaceWrapper PaginatedDataResponseInterface { get; set; }

        public EntityClassWrapper PaginatedDataResponseClass { get; set; }

        public bool? PaginationIsAvailable
        {
            get
            {
                if (!_loadIsComplete)
                {
                    return null;
                }

                return !string.IsNullOrEmpty(PaginatedDataQueryInterface.Name) &&
                       !string.IsNullOrEmpty(PaginatedDataQueryClass.Name) &&
                       !string.IsNullOrEmpty(PaginatedDataResponseInterface.Name) &&
                       !string.IsNullOrEmpty(PaginatedDataResponseClass.Name);
            }
        }

        public void LoadComplete()
        {
            _loadIsComplete = true;
            NotifyPropertyChanged(nameof(PaginationIsAvailable));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
