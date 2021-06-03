using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class ProjectWrapper
    {
        private readonly Project _project;

        public string Name { get; set; }

        public string Namespace { get; set; }

        public ProjectWrapper(Project project)
        {
            _project = project;
            Name = _project.Name;
            Namespace = _project.DefaultNamespace ?? Name;
        }
    }
}
