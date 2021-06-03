using Microsoft.CodeAnalysis;

namespace BoilerplateGenerator.Models
{
    public class ProjectWrapper
    {
        private readonly Project _project;

        public string Name { get; set; }

        public ProjectWrapper(Project project)
        {
            _project = project;
            Name = _project.Name;
        }
    }
}
