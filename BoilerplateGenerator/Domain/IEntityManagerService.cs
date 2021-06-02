using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Models;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Domain
{
    public interface IEntityManagerService
    {
        bool IsEntityClassTypeValid { get; }

        Task<string> LoadSelectedEntityDetails();

        Task FindSelectedFileClassType();

        Task<ITreeNode<IBaseSymbolWrapper>> PopulateClassHierarchy();
    }
}