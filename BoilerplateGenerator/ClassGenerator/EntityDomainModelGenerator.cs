using BoilerplateGenerator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ClassGenerator
{
    public class EntityDomainModelGenerator
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IClassGenerationService _classGenerationService;

        public EntityDomainModelGenerator(IViewModelBase viewModelBase, IClassGenerationService classGenerationService)
        {
            _viewModelBase = viewModelBase;
            _classGenerationService = classGenerationService;
        }


    }
}
