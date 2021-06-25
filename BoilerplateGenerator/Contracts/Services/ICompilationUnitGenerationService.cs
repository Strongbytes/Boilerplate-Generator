using BoilerplateGenerator.Contracts.Generators;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface ICompilationUnitGenerationService
    {
        Task<IGeneratedCompilationUnit> GetGeneratedCompilationUnit();

        Task<string> GetGeneratedCode();
    }
}
