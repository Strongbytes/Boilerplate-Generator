using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.Models.TreeView;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Services
{
    public class CompilationUnitGenerationService : ICompilationUnitGenerationService
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;
        private CompilationUnitSyntax _compilationUnit;

        private BlockSyntax NotImplementedBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();"));

        public CompilationUnitGenerationService(IGenericGeneratorModel genericGeneratorModel)
        {
            _genericGeneratorModel = genericGeneratorModel;
        }

        private async Task<CompilationUnitSyntax> RetrieveCompilationUnit()
        {
            if (_genericGeneratorModel.MergeWithExistingAsset)
            {
                return await _genericGeneratorModel.LoadExistingAssetFromFile().ConfigureAwait(false);
            }

            return SyntaxFactory.CompilationUnit();
        }

        public async Task<IGeneratedCompilationUnit> GetGeneratedCompilationUnit()
        {
            _compilationUnit = await RetrieveCompilationUnit();

            string generatedCode = await Task.Run(() => _compilationUnit.WithUsings(GenerateUsings())
                                                                        .WithMembers(GenerateCompilationUnitWithNamespace())
                                                                        .NormalizeWhitespace(elasticTrivia: true)
                                                                        .ToFullString()).ConfigureAwait(false);

            return new GeneratedCompilationUnit(_genericGeneratorModel, generatedCode);
        }

        private SyntaxList<UsingDirectiveSyntax> GenerateUsings()
        {
            return SyntaxFactory.List((from usingDirective in _genericGeneratorModel.Usings
                                       select SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingDirective)))
                                .Union(_compilationUnit.Usings)
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
                    throw new Exception("Not a valid Compilation Unit Type");
            }
        }

        private ConstructorDeclarationSyntax GenerateConstructorBody(ConstructorDeclarationSyntax existingConstructor, MethodDefinitionModel constructorDeclaration)
        {
            ConstructorDeclarationSyntax newConstructor = existingConstructor
                ?? SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.Name)
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            return newConstructor.WithBody
            (
                GenerateMethodBody
                (
                    GenerateBodyStatements(constructorDeclaration.Body).Union(existingConstructor?.Body?.Statements ?? Enumerable.Empty<StatementSyntax>())
                )
            );
        }

        private TypeDeclarationSyntax GenerateConstructors(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            if (!(typeDeclarationSyntax is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return typeDeclarationSyntax;
            }

            IEnumerable<ConstructorDeclarationSyntax> existingConstructors = classDeclarationSyntax.RetrieveClassMembers<ConstructorDeclarationSyntax>(SyntaxKind.ConstructorDeclaration);

            foreach (MethodDefinitionModel constructorDeclaration in _genericGeneratorModel.DefinedConstructors)
            {
                ConstructorDeclarationSyntax existingConstructor = existingConstructors.RetrieveExistingMember(constructorDeclaration);
                ConstructorDeclarationSyntax newConstructor = GenerateConstructorBody(existingConstructor, constructorDeclaration);

                classDeclarationSyntax = existingConstructor == null
                    ? classDeclarationSyntax.AddMembers(newConstructor)
                    : classDeclarationSyntax.ReplaceNode(existingConstructor, newConstructor);
            }

            return classDeclarationSyntax;
        }

        private ConstructorDeclarationSyntax GenerateConstructorWithInjectedDependencies()
        {
            return SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.Name)
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                .AddParameterListParameters(GenerateParameters(_genericGeneratorModel.InjectedDependencies))
                                .WithBody(GenerateMethodBody(_genericGeneratorModel.InjectedDependencies));
        }

        private MethodDeclarationSyntax[] GenerateMethods()
        {
            return (from methodDeclaration in _genericGeneratorModel.DefinedMethods
                    where methodDeclaration.IsEnabled
                    select SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(methodDeclaration.ReturnType), methodDeclaration.Name)
                                        .AddAttributeLists(GenerateAttributeList(methodDeclaration.Attributes))
                                        .AddModifiers(GenerateModifiers(methodDeclaration.Modifiers))
                                        .AddParameterListParameters(GenerateParameters(methodDeclaration.Parameters))
                                        .WithBody(GenerateMethodBody(GenerateBodyStatements(methodDeclaration.Body)))).ToArray();
        }

        private SyntaxToken[] GenerateModifiers(SyntaxKind[] modifiers)
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
                            SyntaxFactory.Attribute
                            (
                                SyntaxFactory.IdentifierName(attributeDefinition.Name)
                            ).WithArgumentList(!attributeDefinition.Values.Any() ? default : SyntaxFactory.AttributeArgumentList
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

        private FieldDeclarationSyntax[] GenerateFields(IEnumerable<ParameterDefinitionModel> parameters)
        {
            return (from parameter in parameters
                    where !parameter.MapToClassProperty
                    where parameter.IsEnabled
                    select SyntaxFactory.FieldDeclaration
                    (
                        SyntaxFactory.VariableDeclaration
                        (
                            SyntaxFactory.ParseTypeName(parameter.ReturnType),
                            SyntaxFactory.SeparatedList(new[] { SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier($"_{parameter.Name}")) }
                        )
                    )).AddModifiers
                    (
                        new[]
                        {
                            SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
                        }
                    )).ToArray();
        }

        private BlockSyntax GenerateMethodBody(IEnumerable<ParameterDefinitionModel> parameters)
        {
            var assignments = (from parameter in parameters
                               where parameter.IsEnabled
                               select SyntaxFactory.ExpressionStatement
                               (
                                   SyntaxFactory.AssignmentExpression
                                   (
                                       SyntaxKind.SimpleAssignmentExpression,
                                       SyntaxFactory.IdentifierName(parameter.MapToClassProperty ? $"{parameter.Name.ToUpperCamelCase()}" : $"_{parameter.Name}"),
                                       SyntaxFactory.IdentifierName(parameter.Name)
                                    )
                                )).ToArray();

            return SyntaxFactory.Block(assignments);
        }

        private BlockSyntax GenerateMethodBody(IEnumerable<StatementSyntax> bodyStatements)
        {
            if (!bodyStatements.Any())
            {
                return NotImplementedBody;
            }

            return SyntaxFactory.Block(bodyStatements.Distinct(new StatementSyntaxComparer()));
        }

        private IEnumerable<StatementSyntax> GenerateBodyStatements(IEnumerable<string> bodyStatements)
        {
            if (bodyStatements == null || !bodyStatements.Any())
            {
                return Enumerable.Empty<StatementSyntax>();
            }

            return from statement in bodyStatements
                   select SyntaxFactory.ParseStatement(statement);
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
            T classDeclarationSyntax = RetrieveExistingTypeDeclaration<T>();

            if (_genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes != null && _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes.Any())
            {
                classDeclarationSyntax = (T)classDeclarationSyntax.WithBaseList(GenerateBaseTypes(classDeclarationSyntax));
            }

            if (_genericGeneratorModel.DefinedProperties != null && _genericGeneratorModel.DefinedProperties.Any())
            {
                classDeclarationSyntax = (T)classDeclarationSyntax.AddMembers(GenerateProperties());
            }

            if (_genericGeneratorModel.InjectedDependencies != null && _genericGeneratorModel.InjectedDependencies.Any())
            {
                classDeclarationSyntax = (T)classDeclarationSyntax.AddMembers(GenerateFields(_genericGeneratorModel.InjectedDependencies))
                                                                  .AddMembers(GenerateConstructorWithInjectedDependencies());
            }

            if (_genericGeneratorModel.DefinedConstructors != null && _genericGeneratorModel.DefinedConstructors.Any())
            {
                classDeclarationSyntax = (T)GenerateConstructors(classDeclarationSyntax);
            }

            if (_genericGeneratorModel.DefinedMethods != null && _genericGeneratorModel.DefinedMethods.Any(x => x.IsEnabled))
            {
                classDeclarationSyntax = (T)classDeclarationSyntax.AddMembers(GenerateMethods());
            }

            return SyntaxFactory.List(new MemberDeclarationSyntax[] { classDeclarationSyntax });
        }

        private BaseListSyntax GenerateBaseTypes(TypeDeclarationSyntax classDeclarationSyntax)
        {
            BaseListSyntax baseList = classDeclarationSyntax.BaseList ?? SyntaxFactory.BaseList();

            return baseList.WithTypes
            (
                baseList.Types.AddRange
                (
                    from baseType in _genericGeneratorModel.CompilationUnitDefinition.DefinedInheritanceTypes
                    let newNode = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType))
                    where !baseList.Types.Contains(newNode, new BaseTypeSyntaxComparer())
                    select newNode
                )
            );
        }

        private PropertyDeclarationSyntax[] GenerateProperties()
        {
            return (from propertyDefinition in _genericGeneratorModel.DefinedProperties
                    select SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyDefinition.ReturnType), propertyDefinition.Name)
                                        .AddModifiers(GenerateModifiers(propertyDefinition.Modifiers))
                                        .AddAttributeLists(GenerateAttributeList(propertyDefinition.Attributes))
                                        .AddAccessorListAccessors
                                        (
                                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                                         .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                        )).ToArray();
        }
    }
}
