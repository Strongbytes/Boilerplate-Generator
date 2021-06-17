using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Contracts.Services;
using BoilerplateGenerator.Models.ClassGeneratorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Services
{
    public class GeneratorModelsManagerService : IGeneratorModelsManagerService
    {
        private readonly IServiceProvider _serviceProvider;

        public GeneratorModelsManagerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<IGenericGeneratorModel>> RetrieveAvailableGeneratorModels()
        {
            return await Task.Run(() =>
            {
                IEnumerable<Type> generatorModels = Assembly.GetExecutingAssembly()
                                                            .GetTypes()
                                                            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseGenericGeneratorModel)));

                return (from model in generatorModels
                        let args = RetrieveDependencyParameters(model)
                        let newClass = (IGenericGeneratorModel)Activator.CreateInstance(model, args)
                        where newClass.CanBeCreated
                        select newClass).ToArray();
            });
        }

        private object[] RetrieveDependencyParameters(Type referencedType)
        {
            ConstructorInfo baseGeneratorConstructor = RetrieveReferencedTypeConstructor(referencedType);

            return (from parameter in baseGeneratorConstructor.GetParameters()
                    select _serviceProvider.GetService(parameter.ParameterType)).ToArray();
        }

        private ConstructorInfo RetrieveReferencedTypeConstructor(Type referencedType)
        {
            return referencedType.GetConstructors().FirstOrDefault(x => x.GetParameters().Length > 0);
        }
    }
}
