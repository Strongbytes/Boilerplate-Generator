﻿using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using BoilerplateGenerator.Models.TreeView;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
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
        private BlockSyntax EmptyBody => SyntaxFactory.Block(SyntaxFactory.ParseStatement(string.Empty));

        public CompilationUnitGenerationService(IGenericGeneratorModel genericGeneratorModel)
        {
            _genericGeneratorModel = genericGeneratorModel;
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
            _compilationUnit = await RetrieveCompilationUnit();

            string generatedCode = await Task.Run(() => _compilationUnit.WithUsings(GenerateUsings())
                                                                        .WithMembers(GenerateCompilationUnitWithNamespace())
                                                                        .NormalizeWhitespace()
                                                                        .W‌​ithAdditionalAnnotat‌​ions(Formatter.Annot‌​ation)
                                                                        .ToFullString()).ConfigureAwait(false);

            return new GeneratedCompilationUnit(_genericGeneratorModel, generatedCode);
        }

        private SyntaxList<UsingDirectiveSyntax> GenerateUsings()
        {
            return SyntaxFactory.List((from usingDirective in _genericGeneratorModel.Usings
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
                    throw new Exception("Not a valid Compilation Unit Type");
            }
        }

        private ConstructorDeclarationSyntax GenerateConstructor(ConstructorDeclarationSyntax existingConstructor, ConstructorDefinitionModel constructorDeclaration)
        {
            ConstructorDeclarationSyntax newConstructor = existingConstructor
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

            foreach (ConstructorDefinitionModel constructorDeclaration in _genericGeneratorModel.DefinedConstructors)
            {
                ConstructorDeclarationSyntax existingConstructor = existingConstructors.RetrieveExistingMember(constructorDeclaration);
                ConstructorDeclarationSyntax newConstructor = GenerateConstructor(existingConstructor, constructorDeclaration);

                classDeclarationSyntax = existingConstructor == null
                    ? classDeclarationSyntax.AddMembers(newConstructor)
                    : classDeclarationSyntax.ReplaceNode(existingConstructor, newConstructor.WithTriviaFrom(existingConstructor));
            }

            return classDeclarationSyntax;
        }

        private ConstructorDeclarationSyntax GenerateConstructorWithInjectedDependencies()
        {
            return SyntaxFactory.ConstructorDeclaration(_genericGeneratorModel.Name)
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                .AddParameterListParameters(GenerateParameters(_genericGeneratorModel.InjectedDependencies))
                                .WithBody(GenerateParametersAssignmentMethodBody(_genericGeneratorModel.InjectedDependencies));
        }

        private MethodDeclarationSyntax GenerateMethod(MethodDeclarationSyntax existingMethod, MethodDefinitionModel methodDeclaration)
        {
            MethodDeclarationSyntax newMethod = existingMethod
                ?? SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(methodDeclaration.ReturnType), methodDeclaration.Name)
                                .AddAttributeLists(GenerateAttributeList(methodDeclaration.Attributes))
                                .AddModifiers(GenerateModifiers(methodDeclaration.Modifiers))
                                .AddParameterListParameters(GenerateParameters(methodDeclaration.Parameters))
                                .W‌​ithAdditionalAnnotat‌​ions(Formatter.Annot‌​ation);

            return newMethod.WithBody
            (
                GenerateMethodBody
                (
                    GenerateBodyStatements(methodDeclaration.Body).Union(existingMethod?.Body?.Statements ?? Enumerable.Empty<StatementSyntax>())
                )
            );
        }

        private TypeDeclarationSyntax GenerateMethods(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            if (!(typeDeclarationSyntax is ClassDeclarationSyntax classDeclarationSyntax))
            {
                return typeDeclarationSyntax;
            }

            IEnumerable<MethodDeclarationSyntax> existingMethods = classDeclarationSyntax.RetrieveClassMembers<MethodDeclarationSyntax>(SyntaxKind.MethodDeclaration);

            foreach (MethodDefinitionModel methodDeclaration in _genericGeneratorModel.DefinedMethods.Where(x => x.IsEnabled))
            {
                MethodDeclarationSyntax existingMethod = existingMethods.RetrieveExistingMember(methodDeclaration);
                MethodDeclarationSyntax newMethod = GenerateMethod(existingMethod, methodDeclaration);

                classDeclarationSyntax = existingMethod == null
                    ? classDeclarationSyntax.AddMembers(newMethod)
                    : classDeclarationSyntax.ReplaceNode(existingMethod, newMethod.WithTriviaFrom(existingMethod));
            }

            return classDeclarationSyntax;
        }

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

        private BlockSyntax GenerateParametersAssignmentMethodBody(IEnumerable<ParameterDefinitionModel> parameters)
        {
            return SyntaxFactory.Block(from parameter in parameters
                                       where parameter.IsEnabled
                                       let leftAssignment = parameter.MapToClassProperty
                                           ? $"{parameter.Name.ToUpperCamelCase()}"
                                           : $"_{parameter.Name}"
                                       let rightAssignment = parameter.ThrowExceptionWhenNull
                                            ? $"{parameter.Name} ?? throw new ArgumentNullException(nameof({parameter.Name}))"
                                            : parameter.Name
                                       select SyntaxFactory.ParseStatement($"{leftAssignment} = {rightAssignment};"));
        }

        private BlockSyntax GenerateMethodBody(IEnumerable<StatementSyntax> bodyStatements)
        {
            if (!bodyStatements.Any())
            {
                return NotImplementedBody;
            }

            return SyntaxFactory.Block(bodyStatements.Distinct(new StatementSyntaxComparer()))
                                .W‌​ithAdditionalAnnotat‌​ions(Formatter.Annot‌​ation);
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
            T typeDeclarationSyntax = RetrieveExistingTypeDeclaration<T>();

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
                typeDeclarationSyntax = (T)typeDeclarationSyntax.AddMembers(GenerateFields(_genericGeneratorModel.InjectedDependencies))
                                                                .AddMembers(GenerateConstructorWithInjectedDependencies());
            }

            if (_genericGeneratorModel.DefinedConstructors != null && _genericGeneratorModel.DefinedConstructors.Any())
            {
                typeDeclarationSyntax = (T)GenerateConstructors(typeDeclarationSyntax);
            }

            if (_genericGeneratorModel.DefinedMethods != null && _genericGeneratorModel.DefinedMethods.Any(x => x.IsEnabled))
            {
                typeDeclarationSyntax = (T)GenerateMethods(typeDeclarationSyntax);
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
                    let newNode = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseType))
                    where !baseList.Types.Contains(newNode, new BaseTypeSyntaxComparer())
                    select newNode
                )
            );
        }

        private PropertyDeclarationSyntax[] GenerateProperties(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return (from propertyDefinition in _genericGeneratorModel.DefinedProperties
                    select SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(propertyDefinition.ReturnType), propertyDefinition.Name)
                                        .AddModifiers(GenerateModifiers(propertyDefinition.Modifiers))
                                        .AddAttributeLists(GenerateAttributeList(propertyDefinition.Attributes))
                                        .AddAccessorListAccessors(GeneratePropertyAccessors(propertyDefinition.Accessors)))
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
