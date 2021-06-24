using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.ExtraFeatures.Pagination;
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
        private readonly IPaginationRequirements _paginationRequirements;

        public ControllerGeneratorModel
        (
            IViewModelBase viewModelBase,
            IMetadataGenerationService metadataGenerationService,
            IPaginationRequirements paginationRequirements
        )
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
            _metadataGenerationService = metadataGenerationService;
            _paginationRequirements = paginationRequirements;
        }

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
            UsingTokens.MediatR,
            UsingTokens.SystemCollectionsGeneric,
            UsingTokens.SystemNetMime,
            UsingTokens.SystemThreadingTasks,
            UsingTokens.MicrosoftAspNetCoreHttp,
            UsingTokens.MicrosoftAspNetCoreMvc,
            _paginationRequirements.PaginatedDataResponseInterface.Namespace,
            _paginationRequirements.PaginatedDataQueryClass.Namespace,
        }.Union(_metadataGenerationService.AvailableNamespaces)
         .Union(base.UsingsBuilder);

        protected override IProjectWrapper TargetModule => _viewModelBase.SelectedControllersProject;

        public override AssetKind Kind => AssetKind.Controller;

        public override CompilationUnitDefinitionModel CompilationUnitDefinition => new CompilationUnitDefinitionModel
        {
            DefinedInheritanceTypes = new string[] { nameof(CommonTokens.ControllerBase) },
            DefinedAttributes = new List<AttributeDefinitionModel>
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
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> InjectedDependenciesBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{CommonTokens.IMediator}",
                Name = $"{nameof(CommonTokens.Mediator).ToLowerCamelCase()}"
            }
        };

        protected override IEnumerable<MethodDefinitionModel> DefinedMethodsBuilder => new MethodDefinitionModel[]
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
                        Values = new string[] { $"typeof({CommonTokens.IEnumerable}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>)", StatusCodeTokens.Status200OK }
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
                            $"typeof({CommonTokens.IPaginatedDataResponse}<{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]}>)", StatusCodeTokens.Status200OK
                        }
                    }
                },
                Parameters = new ParameterDefinitionModel[]
                {
                    new ParameterDefinitionModel
                    {
                        ReturnType = _paginationRequirements.PaginatedDataQueryClass.Name,
                        Name = _paginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase()
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
                        Values = new string[] { $"typeof({_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status200OK }
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
                        Values = new string[] { $"typeof({_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status201Created }
                    }
                },
                Parameters = new ParameterDefinitionModel[]
                {
                    new ParameterDefinitionModel
                    {
                        ReturnType = _metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.CreateRequestDomainEntity],
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
                        Values = new string[] { $"typeof({_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]})", StatusCodeTokens.Status200OK }
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
                        ReturnType = _metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UpdateRequestDomainEntity],
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

        private IEnumerable<string> GetAllQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.GetAllQuery]}()));"
                };
            }
        }

        private IEnumerable<string> GetPaginatedQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.GetPaginatedQuery]}({_paginationRequirements.PaginatedDataQueryClass.Name.ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> GetByIdQueryBody
        {
            get
            {
                return new string[]
                {
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.GetByIdQuery]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> CreateCommandBody
        {
            get
            {
                return new string[]
                {
                    $"{_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.ResponseDomainEntity]} newEntity = await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.CreateCommand]}({nameof(CommonTokens.Model).ToLowerCamelCase()}));",
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
                    $"return Ok(await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.UpdateCommand]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()}, {nameof(CommonTokens.Model).ToLowerCamelCase()})));"
                };
            }
        }

        private IEnumerable<string> DeleteCommandBody
        {
            get
            {
                return new string[]
                {
                    $"await _mediator.Send(new {_metadataGenerationService.AssetToCompilationUnitNameMapping[AssetKind.DeleteCommand]}({BaseEntityPrimaryKey.Name.ToLowerCamelCase()}));",
                    "return NoContent();"
                };
            }
        }
    }
}
