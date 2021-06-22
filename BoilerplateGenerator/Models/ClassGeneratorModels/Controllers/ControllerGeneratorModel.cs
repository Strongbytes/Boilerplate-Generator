using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.Controllers
{
    public class ControllerGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;
        private readonly IMetadataGenerationService _metadataGenerationService;

        public ControllerGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
        }

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
            UsingTokens.MediatR,
            UsingTokens.SystemCollectionsGeneric,
            UsingTokens.SystemNetMime,
            UsingTokens.SystemThreadingTasks,
            UsingTokens.MicrosoftAspNetCoreHttp,
            UsingTokens.MicrosoftAspNetCoreMvc,
            _viewModelBase.PaginationRequirements.PaginatedDataResponseInterface.Namespace,
            _viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Namespace,
        }.Union(_metadataGenerationService.AvailableNamespaces)
         .Union(base.UsingsBuilder);

        protected override IProjectWrapper TargetModule => _viewModelBase.SelectedControllersProject;

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => new PropertyDefinitionModel[] { };

        public override AssetKind GeneratedAssetKind => AssetKind.Controller;

        protected override IEnumerable<string> BaseTypesBuilder => new string[] { nameof(CommonTokens.ControllerBase) };

        protected override IEnumerable<ParameterDefinitionModel> ConstructorParametersBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IMediator}",
                Name = $"{nameof(CommonTokens.Mediator).ToLowerCamelCase()}"
            }
        };

        protected override IEnumerable<AttributeDefinitionModel> AttributesBuilder => new List<AttributeDefinitionModel>
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

        protected override IEnumerable<MethodDefinitionModel> AvailableMethodsBuilder
        {
            get
            {
                return new MethodDefinitionModel[]
                {
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.GetAll}",
                        IsEnabled = _viewModelBase.GetAllQueryIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}"),
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({CommonTokens.IEnumerable}<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>)", StatusCodeTokens.Status200OK }
                            }
                        },
                        Body = GetAllQueryBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.GetPaginated}",
                        IsEnabled = _viewModelBase.GetPaginatedQueryIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}"),
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[]
                                {
                                    $"typeof({CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]}>)", StatusCodeTokens.Status200OK 
                                }
                            }
                        },
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = _viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Name,
                                Name = _viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase()
                            }
                        },
                        Body = GetPaginatedQueryBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.GetById}",
                        IsEnabled = _viewModelBase.GetByIdQueryIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpGet}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}}}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status404NotFound }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status200OK }
                            }
                        },
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}"
                            }
                        },
                        Body = GetByIdQueryBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Create}",
                        IsEnabled = _viewModelBase.CreateCommandIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpPost}"),
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status400BadRequest }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status201Created }
                            }
                        },
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = _metadataGenerationService.AssetToClassNameMapping[AssetKind.CreateRequestDomainEntity],
                                Name = $"{nameof(CommonTokens.Model).ToLowerCamelCase()}"
                            }
                        },
                        Body = CreateCommandBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Update}",
                        IsEnabled = _viewModelBase.UpdateCommandIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpPut}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}}}\"" }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { StatusCodeTokens.Status404NotFound }
                            },
                            new AttributeDefinitionModel($"{CommonTokens.ProducesResponseType}")
                            {
                                Values = new string[] { $"typeof({_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status200OK }
                            }
                        },
                        Parameters = new ParameterDefinitionModel[]
                        {
                            new ParameterDefinitionModel
                            {
                                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}"
                            },
                            new ParameterDefinitionModel
                            {
                                ReturnType = _metadataGenerationService.AssetToClassNameMapping[AssetKind.UpdateRequestDomainEntity],
                                Name = $"{nameof(CommonTokens.Model).ToLowerCamelCase()}"
                            },
                        },
                        Body = UpdateCommandBody
                    },
                    new MethodDefinitionModel
                    {
                        Name = $"{CommonTokens.Delete}",
                        IsEnabled = _viewModelBase.DeleteCommandIsEnabled,
                        Attributes = new List<AttributeDefinitionModel>
                        {
                            new AttributeDefinitionModel($"{CommonTokens.HttpDelete}")
                            {
                                Values = new string[] { $"\"{{{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}}}\"" }
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
                                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}"
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
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.GetAllQuery]}()));"
                };
            }
        }

        private IEnumerable<string> GetPaginatedQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.GetPaginatedQuery]}({_viewModelBase.PaginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> GetByIdQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.GetByIdQuery]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> CreateCommandBody
        {
            get
            {
                return new string[]
                {
                    $"{_metadataGenerationService.AssetToClassNameMapping[AssetKind.ResponseDomainEntity]} newEntity = await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.CreateCommand]}({nameof(CommonTokens.Model).ToLowerCamelCase()}));",
                    $"var routeValue = new {{ {BaseEntityPrimaryKey.Name.ToLowerCamelCase()} = newEntity.{BaseEntityPrimaryKey.Name} }};",
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
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.UpdateCommand]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()}, {nameof(CommonTokens.Model).ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> DeleteCommandBody
        {
            get
            {
                return new string[]
                {
                    $"await _mediator.Send(new {_metadataGenerationService.AssetToClassNameMapping[AssetKind.DeleteCommand]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()}));",
                    "return NoContent();"
                };
            }
        }
    }
}
