using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using Pluralize.NET;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class ControllerGeneratorModel : GenericGeneratorModel
    {
        public override string RootClassName => $"{new Pluralizer().Pluralize(RootClass.Name)}Controller";

        public override string Namespace => $"{base.Namespace}.Controllers";

        public override IEnumerable<string> Usings => new List<string>
        {
            "MediatR",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc",
            "System.Collections.Generic",
            "System.Net.Mime",
            "System.Threading.Tasks",
        }.Union(base.Usings);

        public override IEnumerable<EntityPropertyWrapper> AvailableProperties => new EntityPropertyWrapper[] { };

        public override AssetKind AssetKind => AssetKind.Controller;

        public override KeyValuePair<string, string>[] ConstructorParameters => new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("IMediator", "mediator")
        };

        public override IEnumerable<MethodDefinitionModel> AvailableMethods
        {
            get
            {
                return new List<MethodDefinitionModel>
                {
                    new MethodDefinitionModel
                    {
                        Name = "GetAll",
                        Modifiers = new SyntaxKind[] { SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword },
                        ReturnType = "Task<IActionResult>",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel
                            {
                                Name = "HttpGet"
                            },
                            new AttributeDefinitionModel
                            {
                                Name = "ProducesResponseType",
                                Values = new string[] { "typeof(List<LearningPathDomainModel>)", "StatusCodes.Status200OK" }
                            }
                        }
                    }
                };
            }
        }

        public ControllerGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
