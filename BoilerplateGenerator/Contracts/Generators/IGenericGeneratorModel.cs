using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGenericGeneratorModel
    {
        bool CanBeCreated { get; }

        AssetKind Kind { get; }

        string Name { get; }

        string TargetProjectName { get; }

        string Namespace { get; }

        bool EnableBaseClassChanging { get; }

        bool FileExistsInProject { get; }

        bool MergeWithExistingAsset { get; }

        IEnumerable<string> Usings { get; }

        IEnumerable<PropertyDefinitionModel> DefinedProperties { get; }

        CompilationUnitDefinitionModel CompilationUnitDefinition { get; }

        IEnumerable<ParameterDefinitionModel> InjectedDependencies { get; }

        IEnumerable<ConstructorDefinitionModel> DefinedConstructors { get; }

        IEnumerable<MethodDefinitionModel> DefinedMethods { get; }

        Task<CompilationUnitSyntax> LoadExistingAssetFromFile();

        Task ExportAssetAsFile(string content);
    }
}
