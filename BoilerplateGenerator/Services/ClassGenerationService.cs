using BoilerplateGenerator.ClassGeneratorModels;
using BoilerplateGenerator.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace BoilerplateGenerator.Services
{
    public class ClassGenerationService : IClassGenerationService
    {
        private readonly IGenericGeneratorModel _genericGeneratorModel;

        public ClassGenerationService(IGenericGeneratorModel genericGeneratorModel)
        {
            _genericGeneratorModel = genericGeneratorModel;
        }

        public string GeneratedClass => SyntaxFactory.CompilationUnit()
                                                     .AddUsings(GenerateUsings())
                                                     .AddMembers(GenerateNamespace())
                                                     .NormalizeWhitespace()
                                                     .ToFullString();

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

        private ClassDeclarationSyntax GenerateClassDeclaration()
        {
            ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(_genericGeneratorModel.RootClassName)
                                                                         .AddModifiers(SyntaxFactory.Token(_genericGeneratorModel.RootClassModifier));

            if (_genericGeneratorModel.BaseTypes != null && _genericGeneratorModel.BaseTypes.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddBaseListTypes(GenerateBaseTypes());
            }

            if (_genericGeneratorModel.AvailableProperties != null && _genericGeneratorModel.AvailableProperties.Any())
            {
                classDeclarationSyntax = classDeclarationSyntax.AddMembers(GenerateProperties());
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
            return (from entityPropertyWrapper in _genericGeneratorModel.AvailableProperties
                    select SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(entityPropertyWrapper.Type), entityPropertyWrapper.Name)
                                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
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
