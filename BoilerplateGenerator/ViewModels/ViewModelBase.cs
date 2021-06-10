using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.TreeView;
using BoilerplateGenerator.Services;
using Microsoft.VisualStudio.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

                    foreach (IGenericGeneratorModel availableModels in await _generatorModelsManagerService.RetrieveAvailableGeneratorModels().ConfigureAwait(false))
                    {
                        var generatedClass = await new ClassGenerationService(availableModels).GetGeneratedClass().ConfigureAwait(false);
                        rootNode.GenerateDirectoryClassTree(generatedClass);
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    DirectoriesTree.Clear();
                    DirectoriesTree.Add(rootNode);
                });

                return _generateCodeCommand;
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
