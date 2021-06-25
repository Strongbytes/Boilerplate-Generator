using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Extensions;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.Models.TreeView;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.VisualStudio.LanguageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Services
{
    public class CompilationUnitGenerationService : ICompilationUnitGenerationService
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;
        private readonly VisualStudioWorkspace _visualStudioWorkspace;
        private CompilationUnitSyntax _compilationUnit;

        private BlockSyntax NotImplementedBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();"));
        private BlockSyntax EmptyBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement(string.Empty));

        public CompilationUnitGenerationService(IGenericGeneratorModel genericGeneratorModel, VisualStudioWorkspace visualStudioWorkspace)
        {
            _genericGeneratorModel = genericGeneratorModel;
            _visualStudioWorkspace = visualStudioWorkspace;
        }

        private async Task<CompilationUnitSyntax> RetrieveCompilationUnit()
        {
            if (_genericGeneratorModel.MergeWithExistingAsset && _genericGeneratorModel.FileExistsInProject)
            {
                return await _genericGeneratorModel.LoadExistingAssetFromFile().ConfigureAwait(false);
            }

            return SyntaxFactory.CompilationUnit();
        }

        public async Task<IGeneratedCompilationUnit> GetGeneratedCompilationUnit()
        {
            string generatedCode = await GetGeneratedCode().ConfigureAwait(false);

            return new GeneratedCompilationUnit(_genericGeneratorModel, _visualStudioWorkspace, generatedCode);
        }

        public async Task<string> GetGeneratedCode()
        {
            _compilationUnit = await RetrieveCompilationUnit().ConfigureAwait(false);

            return await Task.Run
            (
                () => Formatter.Format
                (
                    _compilationUnit.WithUsings(GenerateUsings())
                                    .WithMembers(GenerateCompilationUnitWithNamespace()),
                    _visualStudioWorkspace
                ).ToFullString()
            ).ConfigureAwait(false);
        }

        private SyntaxList<UsingDirectiveSyntax> GenerateUsings()
        {
            HashSet<string> existingnamespacesInBaseTypes = (from baseDeclaration in _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes
                                                             where baseDeclaration.SymbolWasFound
                                                             select baseDeclaration.ReferencedSymbol.GetMembers().OfType<IPropertySymbol>())
                                                            .SelectMany(x => x)
                                                            .Select(x => x.ContainingNamespace.ToString())
                                                            .ToHashSet();

            return SyntaxFactory.List((from usingDirective in _genericGeneratorModel.Usings.Union(existingnamespacesInBaseTypes)
                                       select SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingDirective)))
                                .Union(_compilationUnit.Usings.ToArray() ?? Enumerable.Empty<UsingDirectiveSyntax>())
                                .DistinctBy(x => x.Name.ToFullString()));
        }

        private NamespaceDeclarationSyntax RetrieveNamespaceDeclaration()
        {
            return _compilationUnit.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()
                ?? SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_genericGeneratorModel.Namespace));
        }

        private SyntaxList<MemberDeclarationSyntax> GenerateCompilationUnitWithNamespace()
        {
            return SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                RetrieveNamespaceDeclaration().WithMembers(RetrieveCompilationUnitBasedOnType())
            });
        }

        private SyntaxList<MemberDeclarationSyntax> RetrieveCompilationUnitBasedOnType()
        {
            switch (_genericGeneratorModel.CompilationUnitDefinition.Type)
            {
                case SyntaxKind.ClassDeclaration:
                    return GenerateCompilationUnitDeclaration<ClassDeclarationSyntax>();

                case SyntaxKind.InterfaceDeclaration:
                    return GenerateCompilationUnitDeclaration<InterfaceDeclarationSyntax>();

                default:
                    throw new NotImplementedException("Not a valid Compilation Unit Type");
            }
        }

        private TypeDeclarationSyntax GenerateWithMembers<T>(TypeDeclarationSyntax typeDeclarationSyntax, IEnumerable<MethodDefinitionModel> definedMembers) where T : BaseMethodDeclarationSyntax
        {
            if (!(typeDeclarationSyntax is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return typeDeclarationSyntax;
            }

            IEnumerable<T> availableClassMembers = classDeclarationSyntax.Members.OfType<T>().ToArray();

            foreach (MethodDefinitionModel definedMember in definedMembers.Where(x => x.IsEnabled))
            {
                T existingMember = availableClassMembers.RetrieveExistingMember(definedMember);
                T newMember = (T)GenerateMember(existingMember, definedMember);

                classDeclarationSyntax = existingMember != null
                    ? classDeclarationSyntax.WithMembers
                    (
                        classDeclarationSyntax.Members.Replace(classDeclarationSyntax.Members.FirstOrDefault(x => x.IsEquivalentTo(existingMember)), newMember)
                    )
                    : classDeclarationSyntax.AddMembers(newMember);
            }

            return classDeclarationSyntax;
        }

        #region Generate Method or Constructor
        private BaseMethodDeclarationSyntax GenerateMember<T>(T existingMember, MethodDefinitionModel definedMember) where T : BaseMethodDeclarationSyntax
        {
            switch (definedMember)
            {
                case ConstructorDefinitionModel constructorDeclaration:
                    return GenerateConstructor(existingMember, constructorDeclaration);

                case MethodDefinitionModel methodDeclaration:
                    return GenerateMethod(existingMember, methodDeclaration);

                default:
                    throw new NotImplementedException();
            }
        }

        private BaseMethodDeclarationSyntax GenerateConstructor(BaseMethodDeclarationSyntax existingConstructor, ConstructorDefinitionModel constructorDeclaration)
        {
            BaseMethodDeclarationSyntax newConstructor = existingConstructor
                ?? SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.Name)
                                .WithInitializer
                                (constructorDeclaration.CallBaseConstructor ?
                                    SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                                                 .AddArgumentListArguments(GenerateArguments(constructorDeclaration.Parameters)) : default
                                )
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                .W‌​ithAdditionalAnnotat‌​ions(Formatter.Annot‌​ation)
                                .AddParameterListParameters(GenerateParameters(constructorDeclaration.Parameters));

            if (!constructorDeclaration.Body.Any() || (!existingConstructor?.Body?.Statements.Any() ?? false))
            {
                return newConstructor.WithBody(EmptyBody);
            }

            return newConstructor.WithBody
            (
                GenerateBodyBlockSyntax
                (
                    MergeBodyStatements(GenerateBodyStatements(constructorDeclaration.Body), existingConstructor?.Body?.Statements ?? Enumerable.Empty<StatementSyntax>())
                )
            );
        }

        private BaseMethodDeclarationSyntax GenerateMethod(BaseMethodDeclarationSyntax existingMethod, MethodDefinitionModel methodDeclaration)
        {
            BaseMethodDeclarationSyntax newMethod = existingMethod
                ?? SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(methodDeclaration.ReturnType), methodDeclaration.Name)
                                .AddAttributeLists(GenerateAttributeList(methodDeclaration.Attributes))
                                .AddModifiers(GenerateModifiers(methodDeclaration.Modifiers))
                                .AddParameterListParameters(GenerateParameters(methodDeclaration.Parameters))
                                .W‌​ithAdditionalAnnotat‌​ions(Formatter.Annot‌​ation);

            return newMethod.WithBody
            (
                GenerateBodyBlockSyntax
                (
                    MergeBodyStatements(GenerateBodyStatements(methodDeclaration.Body), existingMethod?.Body?.Statements ?? Enumerable.Empty<StatementSyntax>())
                )
            );
        }

        private IEnumerable<StatementSyntax> MergeBodyStatements(IEnumerable<StatementSyntax> generatedStatements, IEnumerable<StatementSyntax> existingStatements)
        {
            if (!existingStatements.Any())
            {
                return generatedStatements;
            }

            List<StatementSyntax> joinedStatements = new List<StatementSyntax>(existingStatements);

            joinedStatements.AddRange(from generatedStatement in generatedStatements
                                      where !existingStatements.Any(l => l.NormalizeWhitespace().GetText().ToString().Replace(";", string.Empty).Contains(generatedStatement.NormalizeWhitespace().GetText().ToString().Replace(";", string.Empty)))
                                      select generatedStatement);

            return joinedStatements;
        }
        #endregion

        private SyntaxToken[] GenerateModifiers(IEnumerable<SyntaxKind> modifiers)
        {
            return (from modifier in modifiers
                    select SyntaxFactory.Token(modifier)).ToArray();
        }

        private AttributeListSyntax[] GenerateAttributeList(IEnumerable<AttributeDefinitionModel> attributeDefinitionModels)
        {
            return (from attributeDefinition in attributeDefinitionModels
                    select SyntaxFactory.AttributeList
                    (
                        SyntaxFactory.SingletonSeparatedList
                        (
                            SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(attributeDefinition.Name))
                                         .WithArgumentList(!attributeDefinition.Values.Any() ? default : SyntaxFactory.AttributeArgumentList
                                         (
                                             SyntaxFactory.SeparatedList
                                             (
                                                 from attributeValue in attributeDefinition.Values
                                                 select SyntaxFactory.AttributeArgument
                                                 (
                                                     SyntaxFactory.ParseExpression
                                                     (
                                                         attributeValue
                                                     )
                                                 )
                                             )
                                         ))
                        )
                    )).ToArray();
        }

        private ParameterSyntax[] GenerateParameters(IEnumerable<ParameterDefinitionModel> parameters)
        {
            return (from parameter in parameters
                    where parameter.IsEnabled
                    select SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name))
                                        .WithType(SyntaxFactory.ParseTypeName(parameter.ReturnType))).ToArray();
        }

        private ArgumentSyntax[] GenerateArguments(IEnumerable<ParameterDefinitionModel> parameters)
        {
            return (from parameter in parameters
                    where parameter.IsEnabled
                    select SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Name))).ToArray();
        }

        private FieldDeclarationSyntax[] GenerateFields(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return (from parameter in _genericGeneratorModel.InjectedDependencies
                    where !parameter.MapToClassProperty
                    where parameter.IsEnabled
                    select SyntaxFactory.FieldDeclaration
                    (
                        SyntaxFactory.VariableDeclaration
                        (
                            SyntaxFactory.ParseTypeName(parameter.ReturnType),
                            SyntaxFactory.SeparatedList(new[] { SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier($"_{parameter.Name}")) })
                        )
                    ).AddModifiers(new[] { SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword) })
                     .NormalizeWhitespace()
                     .WithTrailingTrivia(SyntaxFactory.LineFeed))
                     .Except(typeDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>(), new FieldDeclarationSyntaxComparer())
                     .ToArray();
        }

        private BlockSyntax GenerateBodyBlockSyntax(IEnumerable<StatementSyntax> bodyStatements)
        {
            return !bodyStatements.Any() ? NotImplementedBody : SyntaxFactory.Block(bodyStatements.Distinct(new StatementSyntaxComparer()));
        }

        private IEnumerable<StatementSyntax> GenerateBodyStatements(IEnumerable<string> bodyStatements)
        {
            return bodyStatements == null || !bodyStatements.Any()
                ? Enumerable.Empty<StatementSyntax>()
                : (from statement in bodyStatements
                   select SyntaxFactory.ParseStatement(statement)
                                       .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));
        }

        private T RetrieveExistingTypeDeclaration<T>() where T : TypeDeclarationSyntax
        {
            return (T)(_compilationUnit.DescendantNodes().OfType<T>().FirstOrDefault()
                ?? SyntaxFactory.TypeDeclaration(_genericGeneratorModel.CompilationUnitDefinition.Type, _genericGeneratorModel.Name)
                                .AddModifiers(SyntaxFactory.Token(_genericGeneratorModel.CompilationUnitDefinition.AccessModifier))
                                .AddAttributeLists(GenerateAttributeList(_genericGeneratorModel.CompilationUnitDefinition.DefinedAttributes)));
        }

        private SyntaxList<MemberDeclarationSyntax> GenerateCompilationUnitDeclaration<T>() where T : TypeDeclarationSyntax
        {
            TypeDeclarationSyntax typeDeclarationSyntax = RetrieveExistingTypeDeclaration<T>();

            if (_genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes != null && _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes.Any())
            {
                typeDeclarationSyntax = (T)typeDeclarationSyntax.WithBaseList(GenerateBaseTypes(typeDeclarationSyntax));
            }

            if (_genericGeneratorModel.DefinedProperties != null && _genericGeneratorModel.DefinedProperties.Any())
            {
                typeDeclarationSyntax = (T)typeDeclarationSyntax.AddMembers(GenerateProperties(typeDeclarationSyntax));
            }

            if (_genericGeneratorModel.InjectedDependencies != null && _genericGeneratorModel.InjectedDependencies.Any())
            {
                typeDeclarationSyntax = (T)typeDeclarationSyntax.AddMembers(GenerateFields(typeDeclarationSyntax));
            }

            if (_genericGeneratorModel.DefinedConstructors != null && _genericGeneratorModel.DefinedConstructors.Any())
            {
                typeDeclarationSyntax = GenerateWithMembers<ConstructorDeclarationSyntax>(typeDeclarationSyntax, _genericGeneratorModel.DefinedConstructors);
            }

            if (_genericGeneratorModel.DefinedMethods != null && _genericGeneratorModel.DefinedMethods.Any(x => x.IsEnabled))
            {
                typeDeclarationSyntax = GenerateWithMembers<MethodDeclarationSyntax>(typeDeclarationSyntax, _genericGeneratorModel.DefinedMethods);
            }

            return SyntaxFactory.List(new MemberDeclarationSyntax[] { typeDeclarationSyntax });
        }

        private BaseListSyntax GenerateBaseTypes(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            BaseListSyntax baseList = typeDeclarationSyntax.BaseList ?? SyntaxFactory.BaseList();

            return baseList.WithTypes
            (
                baseList.Types.AddRange
                (
                    from baseType in _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes
                    let newNode = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType.Name))
                    where !baseList.Types.Contains(newNode, new BaseTypeSyntaxComparer())
                    select newNode
                )
            );
        }

        private PropertyDeclarationSyntax[] GenerateProperties(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            HashSet<string> existingPropertiesInBaseTypes = (from baseDeclaration in _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes
                                                             where baseDeclaration.SymbolWasFound
                                                             select baseDeclaration.ReferencedSymbol.GetMembers().OfType<IPropertySymbol>())
                                                             .SelectMany(x => x)
                                                             .Select(x => x.Name)
                                                             .ToHashSet();

            return (from propertyDefinition in _genericGeneratorModel.DefinedProperties
                    where !existingPropertiesInBaseTypes.Contains(propertyDefinition.Name)
                    select SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyDefinition.ReturnType), propertyDefinition.Name)
                                        .AddModifiers(GenerateModifiers(propertyDefinition.Modifiers))
                                        .AddAttributeLists(GenerateAttributeList(propertyDefinition.Attributes))
                                        .AddAccessorListAccessors(GeneratePropertyAccessors(propertyDefinition.Accessors))
                                        .WithTrailingTrivia(SyntaxFactory.LineFeed)
                                        .WithLeadingTrivia(SyntaxFactory.LineFeed))
                                        .Except(typeDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>(), new PropertyDeclarationSyntaxComparer())
                                        .ToArray();
        }

        private AccessorDeclarationSyntax[] GeneratePropertyAccessors(IEnumerable<PropertyAccessorDefinitionModel> accessorsDefinition)
        {
            return (from accessorDefinition in accessorsDefinition
                    select SyntaxFactory.AccessorDeclaration(accessorDefinition.AccessorType)
                                        .AddModifiers(GenerateAccessorModifier(accessorDefinition.AccessorModifier))
                                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                                        .ToArray();
        }

        private static SyntaxToken[] GenerateAccessorModifier(SyntaxKind accessorModifier)
        {
            if (accessorModifier == SyntaxKind.None || accessorModifier == SyntaxKind.PublicKeyword)
            {
                return new SyntaxToken[] { };
            }

            return new SyntaxToken[] { SyntaxFactory.Token(accessorModifier) };
        }
    }
}
