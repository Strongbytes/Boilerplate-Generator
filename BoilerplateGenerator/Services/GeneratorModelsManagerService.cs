using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.ClassGeneratorModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BoilerplateGenerator.Services
{
    public class GeneratorModelsManagerService : IGeneratorModelsManagerService
    {
        private readonly IServiceProvider _serviceProvider;

        public GeneratorModelsManagerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IGenericGeneratorModel> AvailableGeneratorModels
        {
            get
            {
                var generatorModels = Assembly.GetExecutingAssembly()
                                              .GetTypes()
                                              .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseGenericGeneratorModel)));

                List<IGenericGeneratorModel> models = new List<IGenericGeneratorModel>();

                foreach (var model in generatorModels)
                {
                    object[] args = RetrieveDependencyParameters(model);
                    models.Add((IGenericGeneratorModel)Activator.CreateInstance(model, args));
                }

                return models;
            }
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
