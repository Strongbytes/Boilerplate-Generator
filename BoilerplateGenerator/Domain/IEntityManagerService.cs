using System.Threading.Tasks;

namespace BoilerplateGenerator.Domain
{
    public interface IEntityManagerService
    {
        bool IsEntityClassTypeValid { get; }

        Task<string> LoadSelectedEntityDetails();

        Task FindSelectedFileClassType();

        Task GetValidEntityProperties();
    }
}