using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.RoslynWrappers
{
    public interface IProjectWrapper
    {
        string Name { get; }

        string Namespace { get; }

        string RootDirectory { get; }

        bool GeneratedFileAlreadyExists(string classNamespace, string className);

        Task<INamedTypeSymbol> GetExistingFileClass(string fullyQualifiedMetadataName);
    }
}