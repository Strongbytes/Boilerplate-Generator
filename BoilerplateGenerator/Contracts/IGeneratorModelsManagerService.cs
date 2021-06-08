using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts
{
    public interface IGeneratorModelsManagerService
    {
        IEnumerable<IGenericGeneratorModel> AvailableGeneratorModels { get; }
    }
}