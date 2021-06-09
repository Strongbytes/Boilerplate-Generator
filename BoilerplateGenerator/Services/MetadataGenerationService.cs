using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.ViewModels;
using Pluralize.NET;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Services
{
    public class MetadataGenerationService : IMetadataGenerationService
    {
        private readonly object _locker = new object();

        private readonly IViewModelBase _viewModelBase;

        private string BaseEntityPluralizedName => new Pluralizer().Pluralize(BaseEntity.Name);

        private string TargetModuleNamespace => _viewModelBase.SelectedProject.Namespace;

        private EntityClassWrapper _baseEntity;
        private EntityClassWrapper BaseEntity
        {
            get
            {
                lock(_locker)
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

        public IDictionary<AssetKind, string> AssetToClassNameMapping => new Dictionary<AssetKind, string>
        {
            { AssetKind.ResponseEntityDomainModel, $"{BaseEntity.Name}DomainModel" },
            { AssetKind.Controller, $"{BaseEntityPluralizedName}Controller" },
            { AssetKind.CreateRequestDomainEntity, $"Create{BaseEntity.Name}RequestModel" },
            { AssetKind.UpdateRequestDomainEntity, $"Update{BaseEntity.Name}RequestModel" },
            { AssetKind.GetAllQuery, $"GetAll{BaseEntityPluralizedName}Query" },
            { AssetKind.GetByIdQuery, $"Get{BaseEntity.Name}ByIdQuery" },
            { AssetKind.CreateCommand, $"Create{BaseEntity.Name}Command" },
            { AssetKind.UpdateCommand, $"Update{BaseEntity.Name}Command" },
            { AssetKind.DeleteCommand, $"Delete{BaseEntity.Name}Command" },
            { AssetKind.GetAllQueryHandler, $"GetAll{BaseEntityPluralizedName}QueryHandler" },
            { AssetKind.GetByIdQueryHandler, $"Get{BaseEntity.Name}ByIdQueryHandler" },
            { AssetKind.CreateCommandHandler, $"Create{BaseEntity.Name}CommandHandler" },
            { AssetKind.UpdateCommandHandler, $"Update{BaseEntity.Name}CommandHandler" },
            { AssetKind.DeleteCommandHandler, $"Delete{BaseEntity.Name}CommandHandler" },
        };

        public IDictionary<AssetKind, string> AssetToNamespaceMapping => new Dictionary<AssetKind, string>
        {
            { AssetKind.ResponseEntityDomainModel, $"{TargetModuleNamespace}.Domain.Models" },
            { AssetKind.Controller, $"{TargetModuleNamespace}.Controllers" },
            { AssetKind.CreateRequestDomainEntity, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Create.Models" },
            { AssetKind.UpdateRequestDomainEntity, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Update.Models" },
            { AssetKind.GetAllQuery, $"{TargetModuleNamespace}.Application.Queries.GetAll{BaseEntityPluralizedName}" },
            { AssetKind.GetByIdQuery, $"{TargetModuleNamespace}.Application.Queries.Get{BaseEntity.Name}ById" },
            { AssetKind.CreateCommand, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Create" },
            { AssetKind.UpdateCommand, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Update" },
            { AssetKind.DeleteCommand, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Delete" },
            { AssetKind.GetAllQueryHandler, $"{TargetModuleNamespace}.Application.Queries.GetAll{BaseEntityPluralizedName}" },
            { AssetKind.GetByIdQueryHandler, $"{TargetModuleNamespace}.Application.Queries.Get{BaseEntity.Name}ById" },
            { AssetKind.CreateCommandHandler, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Create" },
            { AssetKind.UpdateCommandHandler, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Update" },
            { AssetKind.DeleteCommandHandler, $"{TargetModuleNamespace}.Application.Commands.{BaseEntityPluralizedName}.Delete" },
        };
    }
}
