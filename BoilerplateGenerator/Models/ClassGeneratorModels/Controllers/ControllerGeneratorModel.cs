using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Controllers
{
    public class ControllerGeneratorModel : BaseGenericGeneratorModel
    {
        public override IEnumerable<string> Usings => new List<string>
        {
            "System",
            "System.Collections.Generic",
            "System.Net.Mime",
            "System.Threading.Tasks",
            "MediatR",
            "Microsoft.AspNetCore.Http",
            "Microsoft.AspNetCore.Mvc"
        }.Union(AssetToNamespaceMapping.Values);

        public override IEnumerable<PropertyDefinitionModel> AvailableProperties => new PropertyDefinitionModel[] { };

        public override AssetKind GeneratedClassKind => AssetKind.Controller;

        public override IEnumerable<string> BaseTypes => new string[] { "ControllerBase" };

        public override IEnumerable<ParameterDefinitionModel> ConstructorParameters => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = "IMediator",
                Name = "mediator"
            }
        };

        public override IEnumerable<AttributeDefinitionModel> Attributes => new List<AttributeDefinitionModel>
        {
            new AttributeDefinitionModel($"{CommonTokens.ApiController}"),
            new AttributeDefinitionModel($"{CommonTokens.Produces}")
            {
                Values = new string[] { "MediaTypeNames.Application.Json" }
            },
            new AttributeDefinitionModel($"{CommonTokens.Route}")
            {
                Values = new string[] { "\"api/[controller]\"" }
            }
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
                        },
                        Body = GetAllQueryBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.GetById}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerInvariant()}}}\"" }
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
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}"
                            }
                        },
                        Body = GetByIdQueryBody
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
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity],
                                Name = "model"
                            }
                        },
                        Body = CreateCommandBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Update}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpPut}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerInvariant()}}}\"" }
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
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}"
                            },
                            new ParameterDefinitionModel
                            {
                                ReturnType = AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity],
                                Name = "model"
                            },
                        },
                        Body = UpdateCommandBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Delete}",
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpDelete}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerInvariant()}}}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status204NoContent }
                            }
                        },
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerInvariant()}"
                            },
                        },
                        Body = DeleteCommandBody
                    }
                };
            }
        }

        private IEnumerable<string> GetAllQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {AssetToClassNameMapping[AssetKind.GetAllQuery]}()));"
                };
            }
        }

        private IEnumerable<string> GetByIdQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {AssetToClassNameMapping[AssetKind.GetByIdQuery]}({BaseEntityPrimaryKey.Name.ToLowerInvariant()})));"
                };
            }
        }

        private IEnumerable<string> CreateCommandBody
        {
            get
            {
                return new string[]
                {
                    $"{AssetToClassNameMapping[AssetKind.ResponseEntityDomainModel]} newEntity = await _mediator.Send(new {AssetToClassNameMapping[AssetKind.CreateCommand]}(model));",
                    $"var routeValue = new {{ {BaseEntityPrimaryKey.Name.ToLowerInvariant()} = newEntity.{BaseEntityPrimaryKey.Name} }};",
                    $"return CreatedAtAction(nameof({$"{CommonTokens.GetById}"}), routeValue, newEntity);"
                };
            }
        }

        private IEnumerable<string> UpdateCommandBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {AssetToClassNameMapping[AssetKind.UpdateCommand]}({BaseEntityPrimaryKey.Name.ToLowerInvariant()}, model)));"
                };
            }
        }

        private IEnumerable<string> DeleteCommandBody
        {
            get
            {
                return new string[]
                {
                    $"await _mediator.Send(new {AssetToClassNameMapping[AssetKind.DeleteCommand]}({BaseEntityPrimaryKey.Name.ToLowerInvariant()}));",
                    "return NoContent();"
                };
            }
        }

        public ControllerGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
