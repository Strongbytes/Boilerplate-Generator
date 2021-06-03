using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Models.RoslynWrappers
{
    public class EntityClassWrapper : BaseSymbolWrapper<INamedTypeSymbol>
    {
        public EntityClassWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }
    }
}
