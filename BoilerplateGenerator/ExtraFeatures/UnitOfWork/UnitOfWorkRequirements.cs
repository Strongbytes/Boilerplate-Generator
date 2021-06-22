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

namespace BoilerplateGenerator.ExtraFeatures.UnitOfWork
{
    public class UnitOfWorkRequirements : IUnitOfWorkRequirements
    {
        private bool _loadIsComplete;
        private readonly IEntityManagerService _entityManagerService;

        public UnitOfWorkRequirements(IEntityManagerService entityManagerService)
        {
            _entityManagerService = entityManagerService;
        }

        public EntityInterfaceWrapper BaseRepositoryInterface { get; private set; }

        public EntityClassWrapper BaseRepositoryClass { get; private set; }

        public EntityInterfaceWrapper BaseUnitOfWorkInterface { get; private set; }

        public EntityClassWrapper BaseUnitOfWorkClass { get; private set; }

        public EntityClassWrapper DbContextClass { get; private set; }

        public bool? FeatureIsAvailable
        {
            get
            {
                if (!_loadIsComplete)
                {
                    return null;
                }

                return !string.IsNullOrEmpty(BaseRepositoryInterface.Name) &&
                       !string.IsNullOrEmpty(BaseRepositoryClass.Name) &&
                       !string.IsNullOrEmpty(BaseUnitOfWorkInterface.Name) &&
                       !string.IsNullOrEmpty(BaseUnitOfWorkClass.Name) &&
                       !string.IsNullOrEmpty(DbContextClass.Name);
            }
        }

        public async Task RetrieveFeatureRequirements()
        {
            INamedTypeSymbol[] availableTypes = await _entityManagerService.RetrieveAllAvailableProjectTypes();

            var baseRepositoryInterface = availableTypes.Where(x => x.TypeKind == TypeKind.Interface)
                                                        .FirstOrDefault(x => x.Name == $"{CommonTokens.IBaseRepository}");

            var baseRepositoryClass = availableTypes.Where(x => x.TypeKind == TypeKind.Class)
                                                    .FirstOrDefault(x => x.Interfaces.Contains(baseRepositoryInterface, new NamedTypeSymbolComparer()));

            var baseUnitOfWorkInterface = availableTypes.Where(x => x.TypeKind == TypeKind.Interface)
                                                        .FirstOrDefault(x => x.Name == $"{CommonTokens.IBaseUnitOfWork}");

            var baseUnitOfWorkClass = availableTypes.Where(x => x.TypeKind == TypeKind.Class)
                                                    .FirstOrDefault(x => x.Interfaces.Contains(baseUnitOfWorkInterface, new NamedTypeSymbolComparer()));

            var dbContextClass = (from availableType in availableTypes
                                  where availableType.TypeKind == TypeKind.Class
                                  where availableType.Name.EndsWith($"{CommonTokens.DbContext}")
                                  where availableType.Constructors.Any()
                                  let constructors = availableType.Constructors
                                  from constructor in constructors
                                  where constructor.Parameters.Any()
                                  let parameters = constructor.Parameters
                                  from parameter in parameters
                                  where parameter.Type.Name == $"{CommonTokens.DbContextOptions}"
                                  select availableType).FirstOrDefault();

            BaseRepositoryInterface = new EntityInterfaceWrapper(baseRepositoryInterface);
            BaseRepositoryClass = new EntityClassWrapper(baseRepositoryClass);
            BaseUnitOfWorkInterface = new EntityInterfaceWrapper(baseUnitOfWorkInterface);
            BaseUnitOfWorkClass = new EntityClassWrapper(baseUnitOfWorkClass);
            DbContextClass = new EntityClassWrapper(dbContextClass);

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
