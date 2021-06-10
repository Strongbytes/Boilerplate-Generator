using BoilerplateGenerator.Contracts.RoslynWrappers;
using System.IO;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class SolutionWrapper : ISolutionWrapper
    {
        private readonly EnvDTE.Solution _solution;

        public string Name { get; }

        public SolutionWrapper(EnvDTE.Solution solution)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            _solution = solution;
            Name = Path.GetFileName(_solution.FullName);
        }
    }
}
