using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Contracts.Generators
{
    public interface IGenericGeneratorModel
    {
        IEnumerable<string> Usings { get; }

        string GeneratedClassName { get; }

        string TargetProjectName { get; }

        string Namespace { get; }

        bool FileExistsInProject { get; }

        SyntaxKind RootClassModifier { get; }

        AssetKind GeneratedClassKind { get; }

        IEnumerable<string> BaseTypes { get; }

        IEnumerable<PropertyDefinitionModel> AvailableProperties { get; }

        IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; }

        IEnumerable<MethodDefinitionModel> AvailableMethods { get; }

        IEnumerable<AttributeDefinitionModel> Attributes { get; }
    }
}
