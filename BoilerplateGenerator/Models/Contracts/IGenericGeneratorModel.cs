using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BoilerplateGenerator.ClassGeneratorModels
{
    public interface IGenericGeneratorModel
    {
        IEnumerable<string> Usings { get; }

        string GeneratedClassName { get; }

        string Namespace { get; }

        string TargetProjectName { get; }

        SyntaxKind RootClassModifier { get; }

        AssetKind AssetKind { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<PropertyDefinitionModel> AvailableProperties { get; }

        KeyValuePair<string, string>[] ConstructorParameters { get; }

        IEnumerable<MethodDefinitionModel> AvailableMethods { get; }
    }
}
