using BoilerplateGenerator.ClassGeneratorModels;
using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.ClassGeneratorModels;
using BoilerplateGenerator.Models.Contracts;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Services;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.ViewModels
{
    public class ViewModelBase : IViewModelBase
    {
        private readonly IEntityManagerService _fileManagerService;

        public ViewModelBase(IEntityManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }


        #region Properties

        private Visibility _loaderVisibility = Visibility.Collapsed;
        public Visibility LoaderVisibility
        {
            get
            {
                return _loaderVisibility;
            }

            set
            {
                if (value == _loaderVisibility)
                {
                    return;
                }

                _loaderVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string _referencedEntityName;
        public string ReferencedEntityName
        {
            get
            {
                return _referencedEntityName;
            }

            set
            {
                if (value == _referencedEntityName)
                {
                    return;
                }

                _referencedEntityName = value;
                NotifyPropertyChanged();
            }
        }

        private ProjectWrapper _selectedProject;
        public ProjectWrapper SelectedProject
        {
            get
            {
                return _selectedProject;
            }

            set
            {
                if (value == _selectedProject)
                {
                    return;
                }

                _selectedProject = value;
                NotifyPropertyChanged();
            }
        }

        private ICSharpCode.AvalonEdit.Document.TextDocument _highlitedGeneratedCode = new ICSharpCode.AvalonEdit.Document.TextDocument();
        public ICSharpCode.AvalonEdit.Document.TextDocument HighlitedGeneratedCode
        {
            get
            {
                return _highlitedGeneratedCode;
            }
            set
            {
                _highlitedGeneratedCode = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Collections
        public ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; } = new ObservableCollection<ITreeNode<IBaseSymbolWrapper>>();

        public ObservableCollection<ProjectWrapper> AvailableModules { get; set; } = new ObservableCollection<ProjectWrapper>();

        private List<IGenericGeneratorModel> ClassGeneratorModels => new List<IGenericGeneratorModel>
        {
            new DomainEntityGeneratorModel(this),
            new ControllerGeneratorModel(this)
        };

        public ObservableCollection<ITreeNode<IBaseGeneratedAsset>> DirectoriesTree { get; set; } = new ObservableCollection<ITreeNode<IBaseGeneratedAsset>>();
        #endregion

        #region Commands
        private ICommand _validateSelectedFileCommand;
        public ICommand ValidateSelectedFileCommand
        {
            get
            {
                if (_validateSelectedFileCommand != null)
                {
                    return _validateSelectedFileCommand;
                }

                _validateSelectedFileCommand = new CommandHandler(async (parameter) =>
                {
                    await _fileManagerService.FindSelectedFileClassType().ConfigureAwait(false);

                    if (!_fileManagerService.IsEntityClassTypeValid)
                    {
                        // TODO: Show validation error
                        return;
                    }

                    var rootNode = await _fileManagerService.PopulateClassHierarchy();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    EntityTree.Clear();
                    EntityTree.Add(rootNode);
                });

                return _validateSelectedFileCommand;
            }
        }

        private ICommand _generateCodeCommand;
        public ICommand GenerateCodeCommand
        {
            get
            {
                if (_generateCodeCommand != null)
                {
                    return _generateCodeCommand;
                }

                _generateCodeCommand = new CommandHandler(async (parameter) =>
                {
                    ITreeNode<IBaseGeneratedAsset> rootNode = new TreeNode<IBaseGeneratedAsset>
                    {
                        Current = new GeneratedDirectory(SelectedProject.Name),
                    };

                    foreach (IGenericGeneratorModel availableModels in ClassGeneratorModels)
                    {
                        var generatedClass = await new ClassGenerationService(availableModels).GetGeneratedClass().ConfigureAwait(false);
                        GenerateGeneratedClassTree(rootNode, generatedClass);
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    DirectoriesTree.Clear();
                    DirectoriesTree.Add(rootNode);
                });

                return _generateCodeCommand;
            }
        }

        private static void GenerateGeneratedClassTree(ITreeNode<IBaseGeneratedAsset> rootNode, IGeneratedClass generatedClass)
        {
            foreach (string directory in generatedClass.ParentDirectoryHierarchy)
            {
                if (rootNode.Current is GeneratedDirectory generatedDirectory && generatedDirectory.AssetName.Equals(directory))
                {
                    continue;
                }

                ITreeNode<IBaseGeneratedAsset> directoryNode = new TreeNode<IBaseGeneratedAsset>
                {
                    Current = new GeneratedDirectory(directory),
                    Parent = rootNode
                };

                rootNode.Children.Add(directoryNode);
                rootNode = directoryNode;
            }

            ITreeNode<IBaseGeneratedAsset> childNode = new TreeNode<IBaseGeneratedAsset>
            {
                Current = generatedClass,
                Parent = rootNode
            };

            rootNode.Children.Add(childNode);

            while (rootNode.Parent != null)
            {
                rootNode = rootNode.Parent;
            }
        }

        private ICommand _showCodeFileOnItemSelected;
        public ICommand ShowCodeFileOnItemSelected
        {
            get
            {
                if (_showCodeFileOnItemSelected != null)
                {
                    return _showCodeFileOnItemSelected;
                }

                _showCodeFileOnItemSelected = new CommandHandler((parameter) =>
                {
                    HighlitedGeneratedCode.Text = string.Empty;

                    if (!(parameter is ITreeNode<IBaseGeneratedAsset> treeNode))
                    {
                        return;
                    }

                    if (!(treeNode.Current is IGeneratedClass generatedClass))
                    {
                        return;
                    }

                    HighlitedGeneratedCode.Text = generatedClass.Code;
                });

                return _showCodeFileOnItemSelected;
            }
        }
        #endregion

        #region Business

        public async Task PopulateSolutionProjects()
        {
            ReferencedEntityName = await _fileManagerService.LoadSelectedEntityDetails();

            foreach (ProjectWrapper item in _fileManagerService.RetrieveAllModules())
            {
                AvailableModules.Add(item);
            }
        }
        #endregion


        #region Event Handlers
        public void ResetInterface()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
