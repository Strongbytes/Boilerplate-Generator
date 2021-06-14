using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Helpers
{
    public static class MemberSyntaxHelper
    {
        public static IEnumerable<T> RetrieveClassMembers<T>(this ClassDeclarationSyntax classDeclarationSyntax, SyntaxKind memberKind) where T : MemberDeclarationSyntax
        {
            return classDeclarationSyntax.Members.Where(x => x.Kind() == memberKind)
                                                 .Cast<T>()
                                                 .ToArray();
        }

        public static T RetrieveExistingMember<T>(this IEnumerable<T> classMembers, MethodDefinitionModel methodDefinitionModel) where T : BaseMethodDeclarationSyntax
        {
            if (!classMembers.Any(x => x.ParameterList.Parameters.Count == methodDefinitionModel.Parameters.Count()))
            {
                return default;
            }

            foreach (BaseMethodDeclarationSyntax classMember in classMembers.Where(x => x.ParameterList.Parameters.Count == methodDefinitionModel.Parameters.Count()))
            {
                var differentParameters = methodDefinitionModel.Parameters.Except(classMember.ParameterList.Parameters.Select(x => new ParameterDefinitionModel
                {
                    Name = x.Identifier.Text,
                    ReturnType = x.Type?.ToString()
                }), new ParameterDefinitionModelComparer());

                if (differentParameters.Any())
                {
                    continue;
                }

                return (T)classMember;
            }

            return default;
        }
    }
}
