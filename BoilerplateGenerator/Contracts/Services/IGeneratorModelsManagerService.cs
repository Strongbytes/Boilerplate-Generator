using BoilerplateGenerator.Contracts.Generators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Services
{
    public interface IGeneratorModelsManagerService
    {
        Task<IEnumerable<IGenericGeneratorModel>> RetrieveAvailableGeneratorModels();
    }
}