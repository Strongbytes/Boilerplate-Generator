using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.Enums;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.ViewModels;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Models.ClassGeneratorModels.ApplicationModule.MediatorRequestsModels
{
    public class DeleteCommandGeneratorModel : BaseGenericGeneratorModel
    {
        private readonly IViewModelBase _viewModelBase;

        public DeleteCommandGeneratorModel(IViewModelBase viewModelBase, IMetadataGenerationService metadataGenerationService)
            : base(viewModelBase, metadataGenerationService)
        {
            _viewModelBase = viewModelBase;
        }

        public override bool CanBeCreated => _viewModelBase.DeleteCommandIsEnabled;

        public override AssetKind GeneratedClassKind => AssetKind.DeleteCommand;

        protected override IEnumerable<string> UsingsBuilder => new string[]
        {
           UsingTokens.MediatR,
        }.Union(base.UsingsBuilder);

        protected override IEnumerable<string> BaseTypesBuilder => new string[]
        {
            $"{CommonTokens.IRequest}<{CommonTokens.Unit}>"
        };

        protected override IEnumerable<PropertyDefinitionModel> AvailablePropertiesBuilder => new PropertyDefinitionModel[]
        {
            new PropertyDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name}",
                Modifiers = new SyntaxKind [] { SyntaxKind.InternalKeyword }
            }
        };

        protected override IEnumerable<ParameterDefinitionModel> ConstructorParametersBuilder => new ParameterDefinitionModel[]
        {
            new ParameterDefinitionModel
            {
                ReturnType = $"{BaseEntityPrimaryKey.ReturnType}",
                Name = $"{BaseEntityPrimaryKey.Name.ToLowerCamelCase()}",
                MapToClassProperty = true
            }
        };
    }
}
