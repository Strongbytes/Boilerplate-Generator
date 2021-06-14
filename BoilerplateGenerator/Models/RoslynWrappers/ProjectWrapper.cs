using BoilerplateGenerator.Contracts.RoslynWrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class ProjectWrapper : IProjectWrapper
    {
        private readonly Project _project;

        public string Name { get; }

        public string Namespace { get; }

        public string RootDirectory => $"{Directory.GetParent(_project.FilePath)}";

        private string GenerateCompleteFilePath(string classNamespace, string className)
        {
            string namespacePath = string.Join("\\", classNamespace.Replace(Namespace, string.Empty).Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries));
            return Path.GetFullPath($"{string.Join("\\", new string[] { RootDirectory, namespacePath, className }.Where(x => !string.IsNullOrEmpty(x)))}.cs");
        }

        public bool GeneratedFileAlreadyExists(string classNamespace, string className)
        {
            if (string.IsNullOrEmpty(_project.FilePath))
            {
                return false;
            }

            return File.Exists(GenerateCompleteFilePath(classNamespace, className));
        }

        public async Task ExportFile(string classNamespace, string className, string content)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(_project.FilePath))
                {
                    throw new Exception("There was an error with the parent project.");
                }

                string filePath = GenerateCompleteFilePath(classNamespace, className);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, content);
            });
        }

        public ProjectWrapper(Project project)
        {
            _project = project;
            Name = _project.Name;
            Namespace = _project.DefaultNamespace ?? Name;
        }

        public async Task<CompilationUnitSyntax> GetExistingFileClass(string classNamespace, string className)
        {
            if (string.IsNullOrEmpty(_project.FilePath))
            {
                return null;
            }

            Compilation compilation = await _project.GetCompilationAsync();
            string completeFilePath = GenerateCompleteFilePath(classNamespace, className);

            SyntaxTree selectedFileSyntaxTree = compilation.SyntaxTrees.FirstOrDefault(x => x.FilePath.Equals(completeFilePath));
            if (selectedFileSyntaxTree == null)
            {
                return null;
            }

            return (await selectedFileSyntaxTree.GetRootAsync()) as CompilationUnitSyntax;
        }
    }
}
