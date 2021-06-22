using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.ExtraFeatures.Pagination
{
    public class PaginationRequirements : IPaginationRequirements
    {
        private bool _loadIsComplete;
        private readonly IEntityManagerService _entityManagerService;

        public PaginationRequirements(IEntityManagerService entityManagerService)
        {
            _entityManagerService = entityManagerService;
        }

        public EntityInterfaceWrapper PaginatedDataQueryInterface { get; private set; }

        public EntityClassWrapper PaginatedDataQueryClass { get; private set; }

        public EntityInterfaceWrapper PaginatedDataResponseInterface { get; private set; }

        public EntityClassWrapper PaginatedDataResponseClass { get; private set; }

        public bool? FeatureIsAvailable
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

        public async Task RetrieveFeatureRequirements()
        {
            INamedTypeSymbol[] availableTypes = await _entityManagerService.RetrieveAllAvailableProjectTypes();

            var iPaginatedDataQuery = availableTypes.Where(x => x.TypeKind == TypeKind.Interface)
                                                    .FirstOrDefault(x => x.Name == $"{CommonTokens.IPaginatedDataQuery}");

            var paginatedDataQuery = availableTypes.Where(x => x.TypeKind == TypeKind.Class)
                                                   .FirstOrDefault(x => x.Interfaces.Contains(iPaginatedDataQuery, new NamedTypeSymbolComparer()));

            var iPaginatedDataResponse = availableTypes.Where(x => x.TypeKind == TypeKind.Interface)
                                                       .FirstOrDefault(x => x.Name == $"{CommonTokens.IPaginatedDataResponse}");

            var paginatedDataResponse = availableTypes.Where(x => x.TypeKind == TypeKind.Class)
                                                      .FirstOrDefault(x => x.Interfaces.Contains(iPaginatedDataResponse, new NamedTypeSymbolComparer()));

            PaginatedDataQueryInterface = new EntityInterfaceWrapper(iPaginatedDataQuery);
            PaginatedDataQueryClass = new EntityClassWrapper(paginatedDataQuery);
            PaginatedDataResponseInterface = new EntityInterfaceWrapper(iPaginatedDataResponse);
            PaginatedDataResponseClass = new EntityClassWrapper(paginatedDataResponse);

            LoadComplete();
        }

        private void LoadComplete()
        {
            _loadIsComplete = true;
            NotifyPropertyChanged(nameof(FeatureIsAvailable));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
