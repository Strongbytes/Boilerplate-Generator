using Microsoft.VisualStudio.Shell.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Helpers
{
    internal static class TypeResolutionExtensions
    {
        public static Type GetTypeFromClassName(this ITypeResolutionService typeResolutionService)
        {
            return null;
            //dynamicTypeService.

            //((Microsoft.VisualStudio.Design.VSTypeResolutionService)_typeResolutionService).LoadedEntries)

            //dynamicTypeService.LoadedEntries
        }
    }
}
