using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class MethodDefinitionModel
    {
        public string Name { get; set; }

        public KeyValuePair<string, string>[] Parameters { get; set; } = new KeyValuePair<string, string>[] { };

        public IEnumerable<AttributeDefinitionModel> Attributes { get; set; } = new AttributeDefinitionModel[] { };

        public string ReturnType { get; set; }

        public SyntaxKind[] Modifiers { get; set; } = new SyntaxKind[] { };
    }
}
