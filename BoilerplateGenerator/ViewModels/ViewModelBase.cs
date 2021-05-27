using BoilerplateGenerator.Domain;
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
        private readonly IFileManagerService _fileManagerService;

        public ViewModelBase(IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }

        #region Collections

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
