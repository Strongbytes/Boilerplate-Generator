using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
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
    public class ClassGenerationService : IClassGenerationService
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;

        private BlockSyntax NotImplementedBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();"));

        public ClassGenerationService(IGenericGeneratorModel genericGeneratorModel)
        {
            _genericGeneratorModel = genericGeneratorModel;
        }

        public async Task<IGeneratedClass> GetGeneratedClass()
        {
            return new GeneratedClass(_genericGeneratorModel, await Task.Run(() => SyntaxFactory.CompilationUnit()
                                                     .AddUsings(GenerateUsings())
                                                     .AddMembers(GenerateNamespace())
                                                     .NormalizeWhitespace()
                                                     .ToFullString()).ConfigureAwait(false));
        }

        private UsingDirectiveSyntax[] GenerateUsings()
        {
            return (from usingDirective in _genericGeneratorModel.Usings
                    select SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingDirective))).ToArray();
        }

        private NamespaceDeclarationSyntax GenerateNamespace()
        {
            return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_genericGeneratorModel.Namespace))
                                .AddMembers(GenerateClassDeclaration());
        }

        private ConstructorDeclarationSyntax GenerateConstructor()
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
                                        .WithBody(GenerateMethodBody(methodDeclaration.Body))).ToArray();
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

        private BlockSyntax GenerateMethodBody(IEnumerable<string> body)
        {
            if (body == null || !body.Any())
            {
                return NotImplementedBody;
            }

            return SyntaxFactory.Block
            (
                from statement in body
                select SyntaxFactory.ParseStatement(statement)
                                    .NormalizeWhitespace(eol: string.Empty)
            );
        }

        private ClassDeclarationSyntax GenerateClassDeclaration()
        {
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(_genericGeneratorModel.GeneratedClassName)
                                                                         .AddModifiers(SyntaxFactory.Token(_genericGeneratorModel.RootClassModifier))
                                                                         .AddAttributeLists(GenerateAttributeList(_genericGeneratorModel.Attributes));

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
                                                               .AddMembers(GenerateConstructor());
            }

            if (_genericGeneratorModel.AvailableMethods != null && _genericGeneratorModel.AvailableMethods.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddMembers(GenerateMethods());
            }

            return classDeclarationSyntax;
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
