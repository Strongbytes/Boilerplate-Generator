using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.RoslynWrappers
{
    public interface IProjectWrapper
    {
        string Name { get; }

        string Namespace { get; }

        string RootDirectory { get; }

        bool GeneratedFileAlreadyExists(string classNamespace, string className);

        Task<CompilationUnitSyntax> GetExistingFileClass(string classNamespace, string className);
    }
}