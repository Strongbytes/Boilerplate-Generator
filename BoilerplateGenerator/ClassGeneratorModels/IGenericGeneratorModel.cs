using BoilerplateGenerator.Models;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BoilerplateGenerator.ClassGeneratorModels
{
    public interface IGenericGeneratorModel
    {
        IEnumerable<string> Usings { get; }

        string RootClassName { get; }

        string Namespace { get; }

        SyntaxKind RootClassModifier { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<EntityPropertyWrapper> AvailableProperties { get; }
    }
}
