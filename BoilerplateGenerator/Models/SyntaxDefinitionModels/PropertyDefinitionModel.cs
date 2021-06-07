using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class PropertyDefinitionModel
    {
        private readonly IDictionary<string, string> LookupAttributes = new Dictionary<string, string> 
        {
            { nameof(CommonTokens.Key), nameof(CommonTokens.Required) },
            { nameof(CommonTokens.Required), nameof(CommonTokens.Required) },
        };

        public string Name { get; set; }

        public ICollection<AttributeDefinitionModel> Attributes { get; set; } = new List<AttributeDefinitionModel>();

        public string ReturnType { get; set; }

        public bool IsPrimaryKey { get; set; }

        public SyntaxKind[] Modifiers { get; set; } = new SyntaxKind[] { SyntaxKind.PublicKeyword };

        public PropertyDefinitionModel()
        {
        }

        public PropertyDefinitionModel(EntityPropertyWrapper entityPropertyWrapper)
        {
            Name = entityPropertyWrapper.Name;
            ReturnType = entityPropertyWrapper.Type;
            IsPrimaryKey = entityPropertyWrapper.IsPrimaryKey;

            foreach (string attribute in from attribute in entityPropertyWrapper.Attributes
                                         where LookupAttributes.Keys.Contains(attribute)
                                         select LookupAttributes[attribute])
            {
                Attributes.Add(new AttributeDefinitionModel(attribute));
            }
        }
    }
}
