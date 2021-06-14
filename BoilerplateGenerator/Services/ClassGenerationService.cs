using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.Models.TreeView;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Services
{
    public class ClassGenerationService : IClassGenerationService
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;
        private CompilationUnitSyntax _compilationUnit;

        private BlockSyntax NotImplementedBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();"));

        public ClassGenerationService(IGenericGeneratorModel genericGeneratorModel)
        {
            _genericGeneratorModel = genericGeneratorModel;
        }

        private async Task<CompilationUnitSyntax> RetrieveCompilationUnit()
        {
            if (_genericGeneratorModel.MergeWithExistingClass)
            {
                return await _genericGeneratorModel.LoadClassFromExistingFile().ConfigureAwait(false);
            }

            return SyntaxFactory.CompilationUnit();
        }

        public async Task<IGeneratedClass> GetGeneratedClass()
        {
            _compilationUnit = await RetrieveCompilationUnit();

            string generatedCode = await Task.Run(() => _compilationUnit.WithUsings(GenerateUsings())
                                                                        .WithMembers(GenerateClassWithNamespace())
                                                                        .NormalizeWhitespace()
                                                                        .ToFullString()).ConfigureAwait(false);

            return new GeneratedClass(_genericGeneratorModel, generatedCode);
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
                ?? SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_genericGeneratorModel.ClassNamespace));
        }

        private SyntaxList<MemberDeclarationSyntax> GenerateClassWithNamespace()
        {
            return SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                RetrieveNamespaceDeclaration().WithMembers(GenerateClassDeclaration())
            });
        }

        private ClassDeclarationSyntax GenerateConstructors(ClassDeclarationSyntax classDeclarationSyntax)
        {
            IEnumerable<ConstructorDeclarationSyntax> existingConstructors = classDeclarationSyntax.RetrieveClassMembers<ConstructorDeclarationSyntax>(SyntaxKind.ConstructorDeclaration);

            foreach (MethodDefinitionModel constructorDeclaration in _genericGeneratorModel.Constructors)
            {
                ConstructorDeclarationSyntax existingConstructor = existingConstructors.RetrieveExistingMember(constructorDeclaration);

                if (existingConstructor != null)
                {
                    classDeclarationSyntax = classDeclarationSyntax.ReplaceNode(existingConstructor, existingConstructor.AddBodyStatements(GenerateBodyStatements(constructorDeclaration.Body).ToArray()));
                    continue;
                }

                var newConstructor = SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.GeneratedClassName)
                                                  .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                  .WithBody(GenerateMethodBody(GenerateBodyStatements(constructorDeclaration.Body).Union(existingConstructor?.Body?.Statements)));

                classDeclarationSyntax = classDeclarationSyntax.AddMembers(newConstructor);
            }

            return classDeclarationSyntax;
        }

        private ConstructorDeclarationSyntax GenerateConstructorWithParameters()
        {
            return SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.GeneratedClassName)
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                .AddParameterListParameters(GenerateParameters(_genericGeneratorModel.ConstructorParameters))
                                .WithBody(GenerateMethodBody(_genericGeneratorModel.ConstructorParameters));
        }

        private MethodDeclarationSyntax[] GenerateMethods()
        {
            return (from methodDeclaration in _genericGeneratorModel.AvailableMethods
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
                    select SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name))
                                        .WithType(SyntaxFactory.ParseTypeName(parameter.ReturnType))).ToArray();
        }

        private FieldDeclarationSyntax[] GenerateFields(IEnumerable<ParameterDefinitionModel> parameters)
        {
            return (from parameter in parameters
                    where !parameter.MapToClassProperty
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
                               select SyntaxFactory.ExpressionStatement
                               (
                                   SyntaxFactory.AssignmentExpression
                                   (
                                       SyntaxKind.SimpleAssignmentExpression,
                                       SyntaxFactory.IdentifierName(parameter.MapToClassProperty ? $"{parameter.Name.ToTitleCase()}" : $"_{parameter.Name}"),
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

            return SyntaxFactory.Block(bodyStatements);
        }

        private IEnumerable<StatementSyntax> GenerateBodyStatements(IEnumerable<string> bodyStatements)
        {
            if (bodyStatements == null || !bodyStatements.Any())
            {
                return Enumerable.Empty<StatementSyntax>();
            }

            return from statement in bodyStatements
                   select SyntaxFactory.ParseStatement(statement)
                                       .NormalizeWhitespace(eol: string.Empty);
        }

        private ClassDeclarationSyntax RetrieveBaseClassDeclaration()
        {
            return _compilationUnit.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()
                ?? SyntaxFactory.ClassDeclaration(_genericGeneratorModel.GeneratedClassName)
                                .AddModifiers(SyntaxFactory.Token(_genericGeneratorModel.RootClassModifier))
                                .AddAttributeLists(GenerateAttributeList(_genericGeneratorModel.Attributes));
        }

        private SyntaxList<MemberDeclarationSyntax> GenerateClassDeclaration()
        {
            ClassDeclarationSyntax classDeclarationSyntax = RetrieveBaseClassDeclaration();

            if (_genericGeneratorModel.BaseTypes != null && _genericGeneratorModel.BaseTypes.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddBaseListTypes(GenerateBaseTypes());
            }

            if (_genericGeneratorModel.AvailableProperties != null && _genericGeneratorModel.AvailableProperties.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddMembers(GenerateProperties());
            }

            if (_genericGeneratorModel.ConstructorParameters != null && _genericGeneratorModel.ConstructorParameters.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddMembers(GenerateFields(_genericGeneratorModel.ConstructorParameters))
                                                               .AddMembers(GenerateConstructorWithParameters());
            }

            if (_genericGeneratorModel.Constructors != null && _genericGeneratorModel.Constructors.Any())
            {
                classDeclarationSyntax = GenerateConstructors(classDeclarationSyntax);
            }

            if (_genericGeneratorModel.AvailableMethods != null && _genericGeneratorModel.AvailableMethods.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddMembers(GenerateMethods());
            }

            return SyntaxFactory.List(new MemberDeclarationSyntax[] { classDeclarationSyntax });
        }

        private BaseTypeSyntax[] GenerateBaseTypes()
        {
            return (from baseType in _genericGeneratorModel.BaseTypes
                    select SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType))).ToArray();
        }

        private PropertyDeclarationSyntax[] GenerateProperties()
        {
            return (from propertyDefinition in _genericGeneratorModel.AvailableProperties
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
