using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models
{
    public class EntityPropertyWrapper : BaseSymbolWrapper<IPropertySymbol>
    {
        public string Type { get; set; }

        public EntityPropertyWrapper(IPropertySymbol symbol) : base(symbol)
        {
            Type = symbol.Type.Name;
        }
    }
}
