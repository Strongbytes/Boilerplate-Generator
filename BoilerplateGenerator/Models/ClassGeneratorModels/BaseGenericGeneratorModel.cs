using BoilerplateGenerator.ClassGeneratorModels;
using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Models.Contracts;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public abstract class BaseGenericGeneratorModel : IGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public Dictionary<AssetKind, string> AssetToClassNameMapping => new Dictionary<AssetKind, string>
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
        };

        public Dictionary<AssetKind, string> AssetToNamespaceMapping => new Dictionary<AssetKind, string>
        {
            { AssetKind.ResponseEntityDomainModel, $"{_viewModelBase.SelectedProject.Namespace}.Domain.Models" },
            { AssetKind.Controller, $"{_viewModelBase.SelectedProject.Namespace}.Controllers" },
            { AssetKind.CreateRequestDomainEntity, $"{_viewModelBase.SelectedProject.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Create.Models" },
            { AssetKind.UpdateRequestDomainEntity, $"{_viewModelBase.SelectedProject.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Update.Models" },
            { AssetKind.GetAllQuery, $"{_viewModelBase.SelectedProject.Namespace}.Application.Queries.GetAll{BaseEntityPluralizedName}" },
            { AssetKind.GetByIdQuery, $"{_viewModelBase.SelectedProject.Namespace}.Application.Queries.Get{BaseEntity.Name}ById" },
            { AssetKind.CreateCommand, $"{_viewModelBase.SelectedProject.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Create" },
            { AssetKind.UpdateCommand, $"{_viewModelBase.SelectedProject.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Update" },
            { AssetKind.DeleteCommand, $"{_viewModelBase.SelectedProject.Namespace}.Application.Commands.{BaseEntityPluralizedName}.Delete" },
        };

        public virtual IEnumerable<string> Usings => new List<string> { "System" };

        public string Namespace => AssetToNamespaceMapping[GeneratedClassKind];

        public string GeneratedClassName => AssetToClassNameMapping[GeneratedClassKind];

        private string BaseEntityPluralizedName => new Pluralizer().Pluralize(BaseEntity.Name);

        public virtual IEnumerable<string> BaseTypes { get; } = new string[] { };

        private IEnumerable<PropertyDefinitionModel> _availableProperties;
        public virtual IEnumerable<PropertyDefinitionModel> AvailableProperties
        {
            get
            {
                lock (_viewModelBase)
                {
                    if (_availableProperties != null)
                    {
                        return _availableProperties;
                    }

                    _availableProperties = FilterTreeProperties(_viewModelBase.EntityTree.First());
                    return _availableProperties;
                }
            }
        }

        private PropertyDefinitionModel _baseEntityPrimaryKey;
        protected PropertyDefinitionModel BaseEntityPrimaryKey
        {
            get
            {
                lock (_viewModelBase)
                {
                    if (_baseEntityPrimaryKey != null)
                    {
                        return _baseEntityPrimaryKey;
                    }

                    _baseEntityPrimaryKey = FilterTreeProperties(_viewModelBase.EntityTree.First()).FirstOrDefault(x => x.IsPrimaryKey) ?? new PropertyDefinitionModel
                    {
                        Name = $"{CommonTokens.Id}",
                        IsPrimaryKey = true,
                        ReturnType = "Int32"
                    };

                    return _baseEntityPrimaryKey;
                }
            }
        }

        private EntityClassWrapper BaseEntity => _viewModelBase.EntityTree.First().Current as EntityClassWrapper;

        public virtual SyntaxKind RootClassModifier => SyntaxKind.PublicKeyword;

        public string TargetProjectName => _viewModelBase.SelectedProject.Name;

        public abstract AssetKind GeneratedClassKind { get; }

        public virtual IEnumerable<ParameterDefinitionModel> ConstructorParameters { get; } = Enumerable.Empty<ParameterDefinitionModel>();

        public virtual IEnumerable<MethodDefinitionModel> AvailableMethods { get; } = Enumerable.Empty<MethodDefinitionModel>();

        public virtual IEnumerable<AttributeDefinitionModel> Attributes { get; } = Enumerable.Empty<AttributeDefinitionModel>();

        protected BaseGenericGeneratorModel(IViewModelBase viewModelBase)
        {
            _viewModelBase = viewModelBase;
        }

        private IEnumerable<PropertyDefinitionModel> FilterTreeProperties(ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            List<PropertyDefinitionModel> symbols = new List<PropertyDefinitionModel>();

            foreach (ITreeNode<IBaseSymbolWrapper> treeNode in rootNode.Children)
            {
                switch (treeNode.Current.GetType().Name)
                {
                    case nameof(EntityClassWrapper):
                        symbols.AddRange(FilterTreeProperties(treeNode));
                        break;

                    default:
                        if (!(treeNode.Current is EntityPropertyWrapper entityPropertyWrapper) || !entityPropertyWrapper.IsChecked)
                        {
                            break;
                        }

                        symbols.Add(new PropertyDefinitionModel(entityPropertyWrapper));
                        break;
                }
            }

            return symbols;
        }
    }
}
