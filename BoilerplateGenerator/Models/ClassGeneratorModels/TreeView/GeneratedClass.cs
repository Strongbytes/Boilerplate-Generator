using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.Enums;
using System;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.TreeView
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

        public string Code { get; }

        public string AssetName => $"{_genericGeneratorModel.GeneratedClassName}.cs";

        public string[] ParentDirectoryHierarchy
        {
            get
            {
                return _genericGeneratorModel.Namespace.Replace(_genericGeneratorModel.TargetProjectName, string.Empty)
                                                       .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
