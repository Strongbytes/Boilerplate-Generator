using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ClassGeneratorModels
{
    public class DomainEntityGeneratorModel : GenericGeneratorModel
    {
        public override string RootClassName => $"{RootClass.Name}DomainModel";

        public DomainEntityGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
