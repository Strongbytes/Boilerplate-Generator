using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VSLangProj;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.Services
{
    internal class EntityManagerService : IEntityManagerService
    {
        #region Definition
        private readonly DTE2 _packageAutomation;

        private readonly ITypeResolutionService _typeResolutionService;

        private readonly ITypeDiscoveryService _typeDiscoveryService;

        private SelectedItem _selectedItem;

        private Project _parentProject;

        private Type _entityClassType;
        #endregion

        #region Properties
        public bool IsEntityClassTypeValid => _entityClassType != null;

        #endregion

        public EntityManagerService(DTE2 packageAutomation)
        {
            _packageAutomation = packageAutomation;
        }

        public async Task<string> LoadSelectedEntityDetails()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _selectedItem = _packageAutomation.SelectedItems.Item(1);
            _parentProject = _selectedItem.ProjectItem.ContainingProject;

            return _selectedItem.Name;
        }

        public async Task FindSelectedFileClassType()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string selectedEntityPath = _selectedItem.ProjectItem.FileNames[1];

            string className = await DynamicClassHelper.ExtractClassNameFromFile(selectedEntityPath);

            if (string.IsNullOrEmpty(className))
            {
                return;
            }




            _typeResolutionService.GetTypeFromClassName();

            // (new System.Collections.Generic.Mscorlib_DictionaryDebugView<System.Reflection.Assembly, Microsoft.VisualStudio.Design.VSTypeResolutionService.AssemblyEntry>(((Microsoft.VisualStudio.Design.VSTypeResolutionService)_typeResolutionService).LoadedEntries).Items[0]).Value.Assembly

            className = "LearningSystem.Module.Data.Models." + className;

            var x = _typeResolutionService.GetType(className);
        }

        public async Task GetValidEntityProperties()
        {
            IDictionary<Type, List<PropertyInfo>> props = new Dictionary<Type, List<PropertyInfo>>();
            DynamicClassHelper.GetAllProperties(_entityClassType, props);

            await Task.CompletedTask;
        }
    }
}

