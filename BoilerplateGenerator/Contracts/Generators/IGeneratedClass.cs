using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGeneratedClass : IBaseGeneratedAsset
    {
        string Code { get; }

        IEnumerable<string> ParentDirectoryHierarchy { get; }

        bool FileExistsInProject { get; }
    }
}
