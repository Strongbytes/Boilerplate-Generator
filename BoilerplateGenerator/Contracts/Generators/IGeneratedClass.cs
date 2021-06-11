using System.Collections.Generic;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGeneratedClass : IBaseGeneratedAsset
    {
        string Code { get; set; }

        IEnumerable<string> ParentDirectoryHierarchy { get; }

        bool FileExistsInProject { get; }
    }
}
