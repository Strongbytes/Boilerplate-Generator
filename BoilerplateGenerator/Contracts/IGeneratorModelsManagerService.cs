using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts
{
    public interface IGeneratorModelsManagerService
    {
        Task<IEnumerable<IGenericGeneratorModel>> RetrieveAvailableGeneratorModels();
    }
}