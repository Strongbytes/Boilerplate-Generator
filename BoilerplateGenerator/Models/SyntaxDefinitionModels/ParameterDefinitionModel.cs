namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class ParameterDefinitionModel
    {
        public string Name { get; set; }

        public string ReturnType { get; set; }

        public bool MapToClassProperty { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool ThrowExceptionWhenNull { get; set; }

        public ParameterDefinitionModel()
        {
        }
    }
}
