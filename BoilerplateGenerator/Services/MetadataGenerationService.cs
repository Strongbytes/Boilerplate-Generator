using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
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
            string relativeNamespace = AssetToNamespaceMapping[referencedAsset];

            return string.Join(".", new string[]
            {
                RetrieveBaseNamespace(referencedAsset),
                relativeNamespace
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
            { AssetKind.GetByIdQuery, $"{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ByIdQuery}" },
            { AssetKind.CreateCommand, $"{CommonTokens.Create}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.UpdateCommand, $"{CommonTokens.Update}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.DeleteCommand, $"{CommonTokens.Delete}{BaseEntity.Name}{CommonTokens.Command}" },
            { AssetKind.GetAllQueryHandler, $"{CommonTokens.GetAll}{BaseEntityPluralizedName}{CommonTokens.QueryHandler}" },
            { AssetKind.GetByIdQueryHandler, $"{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ByIdQueryHandler}" },
            { AssetKind.CreateCommandHandler, $"{CommonTokens.Create}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.UpdateCommandHandler, $"{CommonTokens.Update}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.DeleteCommandHandler, $"{CommonTokens.Delete}{BaseEntity.Name}{CommonTokens.CommandHandler}" },
            { AssetKind.ProfileMapper, $"{BaseEntityPluralizedName}{CommonTokens.Mapper}" },
        };

        private IDictionary<AssetKind, string> AssetToNamespaceMapping => new Dictionary<AssetKind, string>
        {
            { AssetKind.ResponseDomainEntity, $"{NamespaceTokens.Domain}.{NamespaceTokens.Models}" },
            { AssetKind.Controller, $"{NamespaceTokens.Controllers}" },
            { AssetKind.CreateRequestDomainEntity, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}.{NamespaceTokens.Models}" },
            { AssetKind.UpdateRequestDomainEntity, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}.{NamespaceTokens.Models}" },
            { AssetKind.GetAllQuery, $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetAll}{BaseEntityPluralizedName}" },
            { AssetKind.GetByIdQuery, $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ById}" },
            { AssetKind.CreateCommand, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}" },
            { AssetKind.UpdateCommand, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}" },
            { AssetKind.DeleteCommand, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Delete}" },
            { AssetKind.GetAllQueryHandler, $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.GetAll}{BaseEntityPluralizedName}" },
            { AssetKind.GetByIdQueryHandler, $"{NamespaceTokens.Application}.{NamespaceTokens.Queries}.{CommonTokens.Get}{BaseEntity.Name}{CommonTokens.ById}" },
            { AssetKind.CreateCommandHandler, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Create}" },
            { AssetKind.UpdateCommandHandler, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Update}" },
            { AssetKind.DeleteCommandHandler, $"{NamespaceTokens.Application}.{NamespaceTokens.Commands}.{BaseEntityPluralizedName}.{CommonTokens.Delete}" },
            { AssetKind.ProfileMapper, string.Empty },
            { AssetKind.IUnitOfWork, $"{NamespaceTokens.Domain}" },
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
