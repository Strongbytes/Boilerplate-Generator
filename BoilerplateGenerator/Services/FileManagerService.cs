using BoilerplateGenerator.Domain;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.Services
{
    internal class FileManagerService : IFileManagerService
    {
        private readonly DTE2 _packageAutomation;

        private SelectedItem _selectedItem;

        private Project _parentProject;

        public FileManagerService(DTE2 packageAutomation)
        {
            _packageAutomation = packageAutomation;
        }

        public async Task<string> LoadSelectedEntityDetails()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _selectedItem = _packageAutomation.SelectedItems.Item(1);
            _parentProject = _selectedItem.Project;

            return _selectedItem.Name;
        }
    }
}
