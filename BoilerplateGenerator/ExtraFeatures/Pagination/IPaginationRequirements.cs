using BoilerplateGenerator.Models.RoslynWrappers;

namespace BoilerplateGenerator.ExtraFeatures.Pagination
{
    public interface IPaginationRequirements : IBaseFeatureRequirements
    {
        EntityInterfaceWrapper PaginatedDataQueryInterface { get; }

        EntityClassWrapper PaginatedDataQueryClass { get; }

        EntityInterfaceWrapper PaginatedDataResponseInterface { get; }

        EntityClassWrapper PaginatedDataResponseClass { get; }
    }
}
