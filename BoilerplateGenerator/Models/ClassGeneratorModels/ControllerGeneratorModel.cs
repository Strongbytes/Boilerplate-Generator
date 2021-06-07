using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class ControllerGeneratorModel : BaseGenericGeneratorModel
    {
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

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };

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
                        Name = $"{CommonTokens.GetAll}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}"),
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof(List<{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]}>)", StatusCodeTokens.Status200OK }
                            }
                        }
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.GetById}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}")
                            {
                                Values = new string[] { $"\"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status404NotFound }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]})", StatusCodeTokens.Status200OK }
                            }
                        },
                        Parameters = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>($"{BaseEntityPrimaryKey.ReturnType}", $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}")
                        }
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Create}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpPost}"),
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status400BadRequest }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]})", StatusCodeTokens.Status201Created }
                            }
                        },
                        Parameters = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>(AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity], "model")
                        }
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Update}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpPut}")
                            {
                                Values = new string[] { $"\"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status404NotFound }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]})", StatusCodeTokens.Status200OK }
                            }
                        },
                        Parameters = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>($"{BaseEntityPrimaryKey.ReturnType}", $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}"),
                            new KeyValuePair<string, string>(AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity], "model")
                        }
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Delete}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpDelete}")
                            {
                                Values = new string[] { $"\"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status204NoContent }
                            }
                        },
                        Parameters = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>($"{BaseEntityPrimaryKey.ReturnType}", $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}")
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
