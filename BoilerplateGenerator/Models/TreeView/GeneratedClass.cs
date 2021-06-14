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
    public class GeneratedClass : IGeneratedClass
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;

        public GeneratedClass(IGenericGeneratorModel genericGeneratorModel, string generatedCode)
        {
            _genericGeneratorModel = genericGeneratorModel;
            Code = generatedCode;
        }

        public AssetKind AssetKind => _genericGeneratorModel.GeneratedClassKind;

        public string Code { get; set; }

        public string AssetName => $"{_genericGeneratorModel.GeneratedClassName}.cs";

        public bool FileExistsInProject => _genericGeneratorModel.FileExistsInProject;

        public async Task ExportFile() => await _genericGeneratorModel.ExportFile(Code);

        public IEnumerable<string> ParentDirectoryHierarchy
        {
            get
            {
                return new string[] 
                { 
                    _genericGeneratorModel.TargetProjectName 
                }.Union
                (
                    _genericGeneratorModel.ClassNamespace.Replace(_genericGeneratorModel.TargetProjectName, string.Empty)
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
