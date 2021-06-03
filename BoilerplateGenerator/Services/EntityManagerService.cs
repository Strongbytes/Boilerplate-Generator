using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Contracts;
using BoilerplateGenerator.Models.RoslynWrappers;
using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.Services
{
    internal class EntityManagerService : IEntityManagerService
    {
        #region Definition
        private readonly DTE2 _packageAutomation;

        private readonly VisualStudioWorkspace _visualStudioWorkspace;

        private SelectedItem _selectedItem;

        private string _parentProjectName;

        private INamedTypeSymbol _entityClassType;
        #endregion

        #region Properties
        public bool IsEntityClassTypeValid => _entityClassType != null;

        #endregion

        public EntityManagerService(DTE2 packageAutomation, VisualStudioWorkspace visualStudioWorkspace)
        {
            _packageAutomation = packageAutomation;
            _visualStudioWorkspace = visualStudioWorkspace;
        }

        public async Task<string> LoadSelectedEntityDetails()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _selectedItem = _packageAutomation.SelectedItems.Item(1);
            _parentProjectName = _selectedItem.ProjectItem.ContainingProject.Name;

            return _selectedItem.Name;
        }

        public IEnumerable<ProjectWrapper> RetrieveAllModules()
        {
            return from project in _visualStudioWorkspace.CurrentSolution.Projects
                   where !project.Name.Equals(_parentProjectName)
                   select new ProjectWrapper(project);
        }

        public async Task FindSelectedFileClassType()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string selectedEntityPath = _selectedItem.ProjectItem.FileNames[1];

            await Task.Run(async () =>
            {
                var project = _visualStudioWorkspace.CurrentSolution.Projects.First(x => x.Name.Equals(_parentProjectName));
                if (project == null)
                {
                    return;
                }

                var compilation = await project.GetCompilationAsync();
                if (compilation == null)
                {
                    return;
                }

                var selectedFileSyntaxTree = compilation.SyntaxTrees.FirstOrDefault(x => x.FilePath.Equals(selectedEntityPath));
                if (selectedFileSyntaxTree == null)
                {
                    return;
                }

                var classSyntax = (await selectedFileSyntaxTree.GetRootAsync()).DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                _entityClassType = compilation.GetSemanticModel(selectedFileSyntaxTree).GetDeclaredSymbol(classSyntax) as INamedTypeSymbol;
            }).ConfigureAwait(false);
        }

        public async Task<ITreeNode<IBaseSymbolWrapper>> PopulateClassHierarchy()
        {
            return await Task.Run(() =>
            {
                ITreeNode<IBaseSymbolWrapper> rootNode = new TreeNode<IBaseSymbolWrapper>()
                {
                    Current = new EntityClassWrapper(_entityClassType),
                };

                PopulateClassProperties(_entityClassType, rootNode);
                PopulateParentClasses(rootNode);

                return rootNode;
            }).ConfigureAwait(false);
        }

        private void PopulateParentClasses(ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            while (_entityClassType.BaseType != null && !_entityClassType.BaseType.Name.Equals(nameof(Object)))
            {
                TreeNode<IBaseSymbolWrapper> childNode = new TreeNode<IBaseSymbolWrapper>()
                {
                    Current = new EntityClassWrapper(_entityClassType.BaseType),
                    Parent = rootNode
                };

                PopulateClassProperties(_entityClassType.BaseType, childNode);
                rootNode.Children.Add(childNode);

                rootNode = childNode;
                _entityClassType = _entityClassType.BaseType;
            }
        }

        public void PopulateClassProperties(INamedTypeSymbol referencedClass, ITreeNode<IBaseSymbolWrapper> parent)
        {
            foreach (TreeNode<IBaseSymbolWrapper> child in from ISymbol member in referencedClass.GetMembers()
                                                           where member.Kind == SymbolKind.Property
                                                           let property = member as IPropertySymbol
                                                           where property.Type.Name != nameof(ICollection)
                                                           select new TreeNode<IBaseSymbolWrapper>()
                                                           {
                                                               Current = new EntityPropertyWrapper(property),
                                                               Parent = parent
                                                           })
            {
                parent.Children.Add(child);
            }
        }
    }
}

