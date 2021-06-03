using BoilerplateGenerator.Models.Contracts;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Domain
{
    public interface IClassGenerationService
    {
        Task<IGeneratedClass> GetGeneratedClass();
    }
}
