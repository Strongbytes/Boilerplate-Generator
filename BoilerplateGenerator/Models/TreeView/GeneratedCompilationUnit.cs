using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Services;
using Microsoft.VisualStudio.LanguageServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BoilerplateGenerator.Models.TreeView
{
    public class GeneratedCompilationUnit : IGeneratedCompilationUnit
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;
        private readonly VisualStudioWorkspace _visualStudioWorkspace;

        public GeneratedCompilationUnit(IGenericGeneratorModel genericGeneratorModel, VisualStudioWorkspace visualStudioWorkspace, string generatedCode)
        {
            _genericGeneratorModel = genericGeneratorModel;
            _visualStudioWorkspace = visualStudioWorkspace;
            Code = generatedCode;
        }

        public AssetKind AssetKind => _genericGeneratorModel.Kind;

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (value == _code)
                {
                    return;
                }

                _code = value;
                NotifyPropertyChanged();
            }
        }

        public string AssetName => $"{_genericGeneratorModel.Name}.cs";

        public bool FileExistsInProject => _genericGeneratorModel.FileExistsInProject;

        #region Class Options
        public bool EnableBaseClassChanging => _genericGeneratorModel.EnableBaseClassChanging;

        public bool EnableOptionsPanel => EnableBaseClassChanging;

        private ICommand _regenerateCodeCommand;
        public ICommand RegenerateCodeCommand
        {
            get
            {
                if (_regenerateCodeCommand != null)
                {
                    return _regenerateCodeCommand;
                }

                _regenerateCodeCommand = new CommandHandler(async (parameter) =>
                {
                    if (!(parameter is EntityClassWrapper entityClassWrapper))
                    {
                        return;
                    }

                    if (_genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes.Count() > 1)
                    {
                        return;
                    }

                    _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes = !entityClassWrapper.SymbolWasFound 
                        ? Enumerable.Empty<EntityClassWrapper>() 
                        : (new EntityClassWrapper[] { entityClassWrapper });

                    Code = await new CompilationUnitGenerationService(_genericGeneratorModel, _visualStudioWorkspace).GetGeneratedCode().ConfigureAwait(false);
                });

                return _regenerateCodeCommand;
            }
        }
        #endregion

        public async Task ExportAssetAsFile() => await _genericGeneratorModel.ExportAssetAsFile(Code);

        public IEnumerable<string> ParentDirectoryHierarchy => new string[]
        {
            _genericGeneratorModel.TargetProjectName
        }.Union
        (
            _genericGeneratorModel.Namespace.Replace(_genericGeneratorModel.TargetProjectName, string.Empty)
                                            .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
        );

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
