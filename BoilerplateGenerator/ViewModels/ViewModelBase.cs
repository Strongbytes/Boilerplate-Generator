using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Services;
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

namespace BoilerplateGenerator.ViewModels
{
    public class ViewModelBase : IViewModelBase
    {
        private readonly IEntityManagerService _fileManagerService;

        public ViewModelBase(IEntityManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }

        #region Collections

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

                    await _fileManagerService.GetValidEntityProperties();
                });

                return _validateSelectedFileCommand;
            }
        }
        #endregion

        #region Business

        public async Task PopulateSolutionProjects()
        {
            ReferencedEntityName = await _fileManagerService.LoadSelectedEntityDetails();
        }
        #endregion

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
