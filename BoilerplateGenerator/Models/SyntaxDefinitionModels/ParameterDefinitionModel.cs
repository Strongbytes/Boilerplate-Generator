namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class ParameterDefinitionModel
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public bool MapToClassProperty { get; set; }

        public ParameterDefinitionModel()
        {
        }
    }
}
