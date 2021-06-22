using BoilerplateGenerator.Models.RoslynWrappers;

namespace BoilerplateGenerator.ExtraFeatures.UnitOfWork
{
    public interface IUnitOfWorkRequirements : IBaseFeatureRequirements
    {
        EntityInterfaceWrapper BaseRepositoryInterface { get; }

        EntityClassWrapper BaseRepositoryClass { get; }

        EntityInterfaceWrapper BaseUnitOfWorkInterface { get; }

        EntityClassWrapper BaseUnitOfWorkClass { get; }

        EntityClassWrapper DbContextClass { get; }
    }
}
