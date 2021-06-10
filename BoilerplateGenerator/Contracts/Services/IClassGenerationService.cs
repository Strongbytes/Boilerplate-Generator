using BoilerplateGenerator.Contracts.Generators;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface IClassGenerationService
    {
        Task<IGeneratedClass> GetGeneratedClass();
    }
}
