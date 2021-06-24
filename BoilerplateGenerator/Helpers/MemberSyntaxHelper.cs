using BoilerplateGenerator.EqualityComparers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace BoilerplateGenerator.Helpers
{
    public static class MemberSyntaxHelper
    {
        public static T RetrieveExistingMember<T>(this IEnumerable<T> classMembers, MethodDefinitionModel methodDefinitionModel) where T : BaseMethodDeclarationSyntax
        {
            if (!classMembers.Any(x => x.ParameterList.Parameters.Count == methodDefinitionModel.Parameters.Count(y => y.IsEnabled)))
            {
                return default;
            }

            foreach (BaseMethodDeclarationSyntax classMember in classMembers.Where(x => x.ParameterList.Parameters.Count == methodDefinitionModel.Parameters.Count(y => y.IsEnabled)))
            {
                if (classMember is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.Identifier.Text != methodDefinitionModel.Name)
                {
                    continue;
                }

                var differentParameters = methodDefinitionModel.Parameters.Where(y => y.IsEnabled)
                                                                          .Except(classMember.ParameterList.Parameters.Select(x => new ParameterDefinitionModel
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
