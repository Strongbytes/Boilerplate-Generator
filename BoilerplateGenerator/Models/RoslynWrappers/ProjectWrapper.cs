using BoilerplateGenerator.Contracts.RoslynWrappers;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class ProjectWrapper : IProjectWrapper
    {
        private readonly Project _project;

        public string Name { get; }

        public string Namespace { get; }

        public string RootDirectory => $"{Directory.GetParent(_project.FilePath)}";

        public bool GeneratedFileAlreadyExists(string classNamespace, string className)
        {
            if (string.IsNullOrEmpty(_project.FilePath))
            {
                return false;
            }

            string namespacePath = string.Join("\\", classNamespace.Replace(Namespace, string.Empty).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries));
            string completeFilePath = $"{RootDirectory}\\{namespacePath}\\{className}.cs";

            return File.Exists(completeFilePath);
        }

        public ProjectWrapper(Project project)
        {
            _project = project;
            Name = _project.Name;
            Namespace = _project.DefaultNamespace ?? Name;
        }

        public async Task<INamedTypeSymbol> GetExistingFileClass(string fullyQualifiedMetadataName)
        {
            Compilation compilation = await _project.GetCompilationAsync();
            return compilation?.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }
    }
}
