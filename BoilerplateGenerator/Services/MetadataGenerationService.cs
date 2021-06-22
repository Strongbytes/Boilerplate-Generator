using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Services
{
    public class MetadataGenerationService : IMetadataGenerationService
    {
        private readonly object _locker = new object();

        private readonly IViewModelBase _viewModelBase;

        private string BaseEntityPluralizedName => new Pluralizer().Pluralize(BaseEntity.Name);

        private EntityClassWrapper _baseEntity;
        private EntityClassWrapper BaseEntity
        {
            get
            {
                lock (_locker)
                {
                    if (_baseEntity != null)
                    {
                        return _baseEntity;
                    }

                    _baseEntity = _viewModelBase.EntityTree.First().Current as EntityClassWrapper;
                    return _baseEntity;
                }
            }
        }

        public MetadataGenerationService(IViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        public string NamespaceByAssetKind(AssetKind referencedAsset)
        {
            if (!AssetToNamespaceMapping.ContainsKey(referencedAsset))
            {
                throw new Exception($"{referencedAsset} does not have an entry in AssetToNamespaceMapping");
            }

            return AbsoluteNamespace(referencedAsset);
        }

        private string AbsoluteNamespace(AssetKind referencedAsset)
        {
            NamespaceDefinitionModel relativeNamespace = AssetToNamespaceMapping[referencedAsset];

            return string.Join(".", new string[]
            {
                RetrieveBaseNamespace(referencedAsset),
                relativeNamespace.IsEnabled ? relativeNamespace.Content : string.Empty
            }.Where(x => !string.IsNullOrEmpty(x)));
        }

        public IEnumerable<string> AvailableNamespaces => AssetToNamespaceMapping.Keys.Select(AbsoluteNamespace);

        public IDictionary<AssetKind, string> AssetToClassNameMapping => new Dictionary<AssetKind, string>
        {
            { AssetKind.ResponseDomainEntity, $"{BaseEntity.Name}{CommonTokens.DomainModel}" },
            { AssetKind.Controller, $"{BaseEntityPluralizedName}{CommonTokens.Controller}" },
            { AssetKind.CreateRequestDomainEntity, $"{CommonTokens.Create}{BaseEntity.Name}{CommonTokens.RequestModel}" },
            { AssetKind.UpdateRequestDomainEntity, $"{CommonTokens.Update}{BaseEntity.Name}{CommonTokens.RequestModel}" },
            { AssetKind.GetAllQuery, $"{CommonTokens.GetAll}{BaseEntityPluralizedName}{CommonTokens.Query}" },
            { AssetKind.GetPaginatedQuery, $"{CommonTokens.GetPaginated}{BaseEntityPluralizedName}{CommonTokens.Query}" },
            { AssetKind.GetByIdQuery, $"{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ByIdQuery}" },
            { AssetKind.CreateCommand, $"{CommonTokens.Create}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.UpdateCommand, $"{CommonTokens.Update}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.DeleteCommand, $"{CommonTokens.Delete}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.GetAllQueryHandler, $"{CommonTokens.GetAll}{BaseEntityPluralizedName}{CommonTokens.QueryHandler}" },
            { AssetKind.GetPaginatedQueryHandler, $"{CommonTokens.GetPaginated}{BaseEntityPluralizedName}{CommonTokens.QueryHandler}" },
            { AssetKind.GetByIdQueryHandler, $"{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ByIdQueryHandler}" },
            { AssetKind.CreateCommandHandler, $"{CommonTokens.Create}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.UpdateCommandHandler, $"{CommonTokens.Update}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.DeleteCommandHandler, $"{CommonTokens.Delete}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.ProfileMapper, $"{_viewModelBase.SelectedTargetModuleProject.Name.Split(new char[] {'.' }, StringSplitOptions.RemoveEmptyEntries).Last()}{CommonTokens.Mapper}" },
        };

        private IDictionary<AssetKind, NamespaceDefinitionModel> AssetToNamespaceMapping => new Dictionary<AssetKind, NamespaceDefinitionModel>
        {
            {
                AssetKind.ResponseDomainEntity,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Domain}.{NamespaceTokens.Models}"
                }
            },
            {
                AssetKind.Controller,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Controllers}"
                }
            },
            {
                AssetKind.CreateRequestDomainEntity,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}.{NamespaceTokens.Models}",
                    IsEnabled = _viewModelBase.CreateCommandIsEnabled
                }
            },
            {
                AssetKind.UpdateRequestDomainEntity,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}.{NamespaceTokens.Models}",
                    IsEnabled = _viewModelBase.UpdateCommandIsEnabled
                }
            },
            {
                AssetKind.GetAllQuery,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetAll}{BaseEntityPluralizedName}",
                    IsEnabled = _viewModelBase.GetAllQueryIsEnabled
                }
            },
            {
                AssetKind.GetPaginatedQuery,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetPaginated}{BaseEntityPluralizedName}",
                    IsEnabled = _viewModelBase.GetPaginatedQueryIsEnabled
                }
            },
            {
                AssetKind.GetByIdQuery,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ById}",
                    IsEnabled = _viewModelBase.GetByIdQueryIsEnabled
                }
            },
            {
                AssetKind.CreateCommand,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}",
                    IsEnabled = _viewModelBase.CreateCommandIsEnabled
                }
            },
            {
                AssetKind.UpdateCommand,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}",
                    IsEnabled = _viewModelBase.UpdateCommandIsEnabled
                }
            },
            {
                AssetKind.DeleteCommand,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Delete}",
                    IsEnabled = _viewModelBase.DeleteCommandIsEnabled
                }
            },
            {
                AssetKind.GetAllQueryHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetAll}{BaseEntityPluralizedName}",
                    IsEnabled = _viewModelBase.GetAllQueryIsEnabled
                }
            },
            {
                AssetKind.GetPaginatedQueryHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetPaginated}{BaseEntityPluralizedName}",
                    IsEnabled = _viewModelBase.GetPaginatedQueryIsEnabled
                }
            },
            {
                AssetKind.GetByIdQueryHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ById}",
                    IsEnabled = _viewModelBase.GetByIdQueryIsEnabled
                }
            },
            {
                AssetKind.CreateCommandHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}",
                    IsEnabled = _viewModelBase.CreateCommandIsEnabled
                }
            },
            {
                AssetKind.UpdateCommandHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}",
                    IsEnabled = _viewModelBase.UpdateCommandIsEnabled
                }
            },
            {
                AssetKind.DeleteCommandHandler,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Delete}",
                    IsEnabled = _viewModelBase.UpdateCommandIsEnabled
                }
            },
            {
                AssetKind.ProfileMapper,
                new NamespaceDefinitionModel
                {
                    IsEnabled = _viewModelBase.GenerateAutoMapperProfile
                }
            },
            {
                AssetKind.IUnitOfWork,
                new NamespaceDefinitionModel
                {
                    Content = $"{NamespaceTokens.Domain}",
                    IsEnabled = _viewModelBase.UseUnitOfWork
                }
            },
        };

        private string RetrieveBaseNamespace(AssetKind referencedAsset)
        {
            switch (referencedAsset)
            {
                case AssetKind.Controller:
                    return _viewModelBase.SelectedControllersProject.Namespace;

                default:
                    return _viewModelBase.SelectedTargetModuleProject.Namespace;
            }
        }
    }
}
