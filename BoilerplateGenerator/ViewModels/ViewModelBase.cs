using BoilerplateGenerator.ClassGeneratorModels;
using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models;
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

        private string _ReferencedEntityName;
        public string ReferencedEntityName
        {
            get
            {
                return _ReferencedEntityName;
            }

            set
            {
                if (value == _ReferencedEntityName)
                {
                    return;
                }

                _ReferencedEntityName = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Collections
        public ObservableCollection<ITreeNode<IBaseSymbolWrapper>> EntityTree { get; set; } = new ObservableCollection<ITreeNode<IBaseSymbolWrapper>>();

        public ObservableCollection<ProjectWrapper> AvailableModules { get; set; } = new ObservableCollection<ProjectWrapper>();
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

                    var y = new ClassGenerationService(new DomainEntityGeneratorModel(this));
                    var z = y.GeneratedClass;
                });

                return _validateSelectedFileCommand;
            }
        }
        #endregion

        #region Business

        public async Task PopulateSolutionProjects()
        {
            ReferencedEntityName = await _fileManagerService.LoadSelectedEntityDetails();

            foreach(ProjectWrapper item in _fileManagerService.RetrieveAllModules())
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
