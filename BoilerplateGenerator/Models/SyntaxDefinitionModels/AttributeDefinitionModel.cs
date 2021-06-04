using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class AttributeDefinitionModel
    {
        public string Name { get; set; }

        public string[] Values { get; set; } = new string[] { };
    }
}
