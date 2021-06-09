using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts
{
    public interface IClassGenerationService
    {
        Task<IGeneratedClass> GetGeneratedClass();
    }
}
