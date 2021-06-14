using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.TreeView;
using BoilerplateGenerator.Services;
using Microsoft.VisualStudio.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator.ViewModels
{
    public class ViewModelBase : IViewModelBase
    {
        private readonly IEntityManagerService _fileManagerService;
        private readonly IGeneratorModelsManagerService _generatorModelsManagerService;

        public ViewModelBase(IEntityManagerService fileManagerService, IGeneratorModelsManagerService generatorModelsManagerService)
        {
            _fileManagerService = fileManagerService;
            _generatorModelsManagerService = generatorModelsManagerService;
        }

        #region Properties
        private bool _getCommandEnabled = true;
        public bool GetCommandEnabled
        {
            get
            {
                return _getCommandEnabled;
            }

            set
            {
                if (value == _getCommandEnabled)
                {
                    return;
                }

                _getCommandEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _postCommandEnabled = true;
        public bool PostCommandEnabled
        {
            get
            {
                return _postCommandEnabled;
            }

            set
            {
                if (value == _postCommandEnabled)
                {
                    return;
                }

                _postCommandEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _putCommandEnabled = true;
        public bool PutCommandEnabled
        {
            get
            {
                return _putCommandEnabled;
            }

            set
            {
                if (value == _putCommandEnabled)
                {
                    return;
                }

                _putCommandEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private bool _useUnitOfWork = true;
        public bool UseUnitOfWork
        {
            get
            {
                return _useUnitOfWork;
            }

            set
            {
                if (value == _useUnitOfWork)
                {
                    return;
                }

                _useUnitOfWork = value;
                NotifyPropertyChanged();
            }
        }

        private bool _deleteCommandEnabled = true;
        public bool DeleteCommandEnabled
        {
            get
            {
                return _deleteCommandEnabled;
            }

            set
            {
                if (value == _deleteCommandEnabled)
                {
                    return;
                }

                _deleteCommandEnabled = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }


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

        public bool GenerateCodeCommandIsEnabled => SelectedControllersProject != null && SelectedTargetModuleProject != null
                                                && (GetCommandEnabled || PostCommandEnabled || PutCommandEnabled || DeleteCommandEnabled);

        private IProjectWrapper _selectedProject;
        public IProjectWrapper SelectedTargetModuleProject
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
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        private IProjectWrapper _selectedControllersProject;
        public IProjectWrapper SelectedControllersProject
        {
            get
            {
                return _selectedControllersProject;
            }

            set
            {
                if (value == _selectedControllersProject)
                {
                    return;
                }

                _selectedControllersProject = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(GenerateCodeCommandIsEnabled));
            }
        }

        public ISolutionWrapper Solution { get; set; }
        #endregion

        #region Collections
        public ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; } = new ObservableCollection<ITreeNode<IBaseSymbolWrapper>>();

        public ObservableCollection<IProjectWrapper> AvailableModules { get; set; } = new ObservableCollection<IProjectWrapper>();

        public ObservableCollection<ITreeNode<IBaseGeneratedAsset>> DirectoriesTree { get; set; } = new ObservableCollection<ITreeNode<IBaseGeneratedAsset>>();
        #endregion

        #region Commands
        private ICommand _viewEntityHierarchyCommand;
        public ICommand ViewEntityHierarchyCommand
        {
            get
            {
                if (_viewEntityHierarchyCommand != null)
                {
                    return _viewEntityHierarchyCommand;
                }

                _viewEntityHierarchyCommand = new CommandHandler(async (parameter) =>
                {
                    EntityTree.Clear();
                    DirectoriesTree.Clear();

                    await _fileManagerService.FindSelectedFileClassType().ConfigureAwait(false);

                    if (!_fileManagerService.IsEntityClassTypeValid)
                    {
                        return;
                    }

                    ITreeNode<IBaseSymbolWrapper> rootNode = await _fileManagerService.PopulateClassHierarchy();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    EntityTree.Clear();
                    DirectoriesTree.Clear();
                    EntityTree.Add(rootNode);
                });

                return _viewEntityHierarchyCommand;
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
                    DirectoriesTree.Clear();
                   
                    ITreeNode<IBaseGeneratedAsset> rootNode = new TreeNode<IBaseGeneratedAsset>
                    {
                        Current = new GeneratedDirectory(Solution.Name),
                    };

                    foreach (IGenericGeneratorModel availableModel in await _generatorModelsManagerService.RetrieveAvailableGeneratorModels().ConfigureAwait(false))
                    {
                        IGeneratedClass generatedClass = await new ClassGenerationService(availableModel).GetGeneratedClass().ConfigureAwait(false);
                        rootNode.GenerateDirectoryClassTree(generatedClass);
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    DirectoriesTree.Clear();
                    DirectoriesTree.Add(rootNode);
                });

                return _generateCodeCommand;
            }
        }

        private ICommand _exportGeneratedFilesCommand;
        public ICommand ExportGeneratedFilesCommand
        {
            get
            {
                if (_exportGeneratedFilesCommand != null)
                {
                    return _exportGeneratedFilesCommand;
                }

                _exportGeneratedFilesCommand = new CommandHandler(async (parameter) =>
                {
                    await DirectoriesTree.First().ExportGeneratedFiles().ConfigureAwait(false);
                });

                return _exportGeneratedFilesCommand;
            }
        }
        #endregion

        #region Business

        public async Task PopulateSolutionProjects()
        {
            Solution = await _fileManagerService.RetrieveSolution();

            ReferencedEntityName = await _fileManagerService.LoadSelectedEntityDetails();

            foreach (IProjectWrapper item in _fileManagerService.RetrieveAllModules())
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
