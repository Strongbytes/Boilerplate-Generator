using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.ExtraFeatures.Pagination;
using BoilerplateGenerator.Models.Enums;
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

        private ICollection<INamedTypeSymbol> _visitedClasses;

        private INamedTypeSymbol[] _availableTypes;
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

        public IEnumerable<IProjectWrapper> RetrieveAllModules()
        {
            return from project in _visualStudioWorkspace.CurrentSolution.Projects
                   select new ProjectWrapper(project);
        }

        public async Task<INamedTypeSymbol[]> RetrieveAllAvailableProjectTypes()
        {
            if (_availableTypes?.Any() ?? false)
            {
                return _availableTypes;
            }

            _availableTypes = (await Task.WhenAll
            (
                _visualStudioWorkspace.CurrentSolution.Projects
                    .SelectMany(project => project.Documents)
                    .Select(async document => new
                    {
                        Model = await document.GetSemanticModelAsync().ConfigureAwait(false),
                        Declarations = (await document.GetSyntaxRootAsync().ConfigureAwait(false))?.DescendantNodes().OfType<TypeDeclarationSyntax>()
                    }))
            ).SelectMany(pair => pair.Declarations.Select(declaration => pair.Model.GetDeclaredSymbol(declaration) as INamedTypeSymbol))
             .ToArray();

            return _availableTypes;
        }

        public async Task<ISolutionWrapper> RetrieveSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new SolutionWrapper(_packageAutomation.Solution);
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
                ITreeNode<IBaseSymbolWrapper> rootNode = new EntityHierarchyTreeNode
                {
                    Current = new EntityClassWrapper(_entityClassType),
                };

                _visitedClasses = new HashSet<INamedTypeSymbol>
                {
                    _entityClassType
                };

                PopulateClassProperties(_entityClassType, rootNode);
                PopulateParentClasses(_entityClassType, rootNode);

                return rootNode;
            }).ConfigureAwait(false);
        }

        private void PopulateParentClasses(INamedTypeSymbol referencedClass, ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            while (referencedClass.BaseType != null && !referencedClass.BaseType.Name.Equals(nameof(Object)))
            {
                EntityHierarchyTreeNode childNode = new EntityHierarchyTreeNode
                {
                    Current = new EntityClassWrapper(referencedClass.BaseType),
                    Parent = rootNode
                };

                _visitedClasses.Add(referencedClass.BaseType);

                PopulateClassProperties(referencedClass.BaseType, childNode);
                rootNode.Children.Add(childNode);

                rootNode = childNode;
                referencedClass = referencedClass.BaseType;
            }
        }

        private void PopulateClassProperties(INamedTypeSymbol referencedClass, ITreeNode<IBaseSymbolWrapper> parent)
        {
            foreach (ISymbol member in referencedClass.GetMembers())
            {
                if (member.Kind != SymbolKind.Property)
                {
                    continue;
                }

                IPropertySymbol property = member as IPropertySymbol;

                if (property.Type.Name == nameof(ICollection))
                {
                    continue;
                }

                if (property.Type is INamedTypeSymbol innerClass 
                    && innerClass.BaseType != null 
                    && innerClass.BaseType.MetadataName == referencedClass.BaseType.MetadataName)
                {
                    if (_visitedClasses.Contains(innerClass))
                    {
                        continue;
                    }

                    _visitedClasses.Add(innerClass);

                    EntityHierarchyTreeNode childNode = new EntityHierarchyTreeNode
                    {
                        Current = new EntityClassWrapper(innerClass),
                        Parent = parent
                    };

                    PopulateClassProperties(innerClass, childNode);
                    parent.Children.Add(childNode);
                    continue;
                }

                EntityHierarchyTreeNode child = new EntityHierarchyTreeNode
                {
                    Current = new EntityPropertyWrapper(property),
                    Parent = parent
                };

                parent.Children.Add(child);
            }
        }
    }
}

