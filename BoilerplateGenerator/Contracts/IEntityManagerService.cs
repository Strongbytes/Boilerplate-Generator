using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Models.RoslynWrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts
{
    public interface IEntityManagerService
    {
        bool IsEntityClassTypeValid { get; }

        Task<string> LoadSelectedEntityDetails();

        IEnumerable<ProjectWrapper> RetrieveAllModules();

        Task FindSelectedFileClassType();

        Task<ITreeNode<IBaseSymbolWrapper>> PopulateClassHierarchy();
    }
}