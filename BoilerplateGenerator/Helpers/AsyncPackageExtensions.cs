using EnvDTE;
using EnvDTE80;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;

namespace BoilerplateGenerator.Helpers
{
    public static class AsyncPackageExtensions
    {
        //public static void GetVisualStudioWorkspace(this AsyncPackage package)
        //{
        //    var componentModel = (IComponentModel)package.GetService<SComponentModel>();
        //    var workspace = componentModel.GetService<VisualStudioWorkspace>();
        //}

        public static ITypeDiscoveryService GetTypeDiscoveryService(this AsyncPackage package)
        {
            return package.GetService<DynamicTypeService>()
                          .GetTypeDiscoveryService(RetrieveIVsHierarchy(package));
        }

        public static ITypeResolutionService GetTypeResolutionService(this AsyncPackage package)
        {
            return package.GetService<DynamicTypeService>()
                          .GetTypeResolutionService(RetrieveIVsHierarchy(package));
        }

        private static IVsHierarchy RetrieveIVsHierarchy(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            package.GetService<IVsSolution>()
                   .GetProjectOfUniqueName(RetrieveProjectBasedOnSelectedItem(package).UniqueName, out IVsHierarchy hier);

            return hier;
        }

        private static Project RetrieveProjectBasedOnSelectedItem(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (package.GetService<DTE>() as DTE2).SelectedItems.Item(1).ProjectItem.ContainingProject;
        }
    }
}
