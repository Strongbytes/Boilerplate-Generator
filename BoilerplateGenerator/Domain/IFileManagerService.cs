using System.Threading.Tasks;

namespace BoilerplateGenerator.Domain
{
    public interface IFileManagerService
    {
        Task<string> LoadSelectedEntityDetails();
    }
}