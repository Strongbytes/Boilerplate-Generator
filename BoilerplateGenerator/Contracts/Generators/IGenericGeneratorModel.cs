using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGenericGeneratorModel
    {
        IEnumerable<string> Usings { get; }

        string GeneratedClassName { get; }

        string TargetProjectName { get; }

        string ClassNamespace { get; }

        bool FileExistsInProject { get; }

        bool MergeWithExistingClass { get; }

        SyntaxKind RootClassModifier { get; }

        AssetKind GeneratedClassKind { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<PropertyDefinitionModel> AvailableProperties { get; }

        IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; }

        IEnumerable<MethodDefinitionModel> Constructors { get; }

        IEnumerable<MethodDefinitionModel> AvailableMethods { get; }

        IEnumerable<AttributeDefinitionModel> Attributes { get; }

        Task<CompilationUnitSyntax> LoadClassFromExistingFile();
    }
}
