using Microsoft.CodeAnalysis.CSharp;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class PropertyAccessorDefinitionModel
    {
        internal SyntaxKind AccessorType { get; set; }

        internal SyntaxKind AccessorModifier { get; set; } = SyntaxKind.PublicKeyword;
    }
}
