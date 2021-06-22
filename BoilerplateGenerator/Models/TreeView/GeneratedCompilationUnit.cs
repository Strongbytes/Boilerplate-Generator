using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.TreeView
{
    public class GeneratedCompilationUnit : IGeneratedCompilationUnit
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;

        public GeneratedCompilationUnit(IGenericGeneratorModel genericGeneratorModel, string generatedCode)
        {
            _genericGeneratorModel = genericGeneratorModel;
            Code = generatedCode;
        }

        public AssetKind AssetKind => _genericGeneratorModel.Kind;

        public string Code { get; set; }

        public string AssetName => $"{_genericGeneratorModel.Name}.cs";

        public bool FileExistsInProject => _genericGeneratorModel.FileExistsInProject;

        public async Task ExportAssetAsFile() => await _genericGeneratorModel.ExportAssetAsFile(Code);

        public IEnumerable<string> ParentDirectoryHierarchy
        {
            get
            {
                return new string[] 
                { 
                    _genericGeneratorModel.TargetProjectName 
                }.Union
                (
                    _genericGeneratorModel.Namespace.Replace(_genericGeneratorModel.TargetProjectName, string.Empty)
                                                              .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
