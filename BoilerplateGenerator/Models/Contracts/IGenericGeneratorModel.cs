using BoilerplateGenerator.Models.ClassGeneratorModels;
using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BoilerplateGenerator.ClassGeneratorModels
{
    public interface IGenericGeneratorModel
    {
        IEnumerable<string> Usings { get; }

        string RootClassName { get; }

        string Namespace { get; }

        string TargetProjectName { get; }

        SyntaxKind RootClassModifier { get; }

        AssetKind AssetKind { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<EntityPropertyWrapper> AvailableProperties { get; }
    }
}
