using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;

namespace BoilerplateGenerator.Helpers
{
    public static class AsyncPackageExtensions
    {
        public static VisualStudioWorkspace GetVisualStudioWorkspace(this AsyncPackage package)
        {
            IComponentModel componentModel = (IComponentModel)package.GetService<SComponentModel>();
            return componentModel.GetService<VisualStudioWorkspace>();
        }
    }
}
