using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGenericGeneratorModel
    {
        bool CanBeCreated { get; }

        string GeneratedAssetName { get; }

        AssetKind GeneratedAssetKind { get; }

        string TargetProjectName { get; }

        string ContainingNamespace { get; }

        bool FileExistsInProject { get; }

        bool MergeWithExistingAsset { get; }

        SyntaxKind AccessModifier { get; }

        IEnumerable<string> Usings { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<PropertyDefinitionModel> DefinedProperties { get; }

        IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; }

        IEnumerable<MethodDefinitionModel> DefinedConstructors { get; }

        IEnumerable<MethodDefinitionModel> DefinedMethods { get; }

        IEnumerable<AttributeDefinitionModel> DefinedAttributes { get; }

        Task<CompilationUnitSyntax> LoadExistingAssetFromFile();

        Task ExportAssetAsFile(string content);
    }
}
