namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class ConstructorDefinitionModel : MethodDefinitionModel
    {
        public ConstructorDefinitionModel()
        {
            ReturnType = string.Empty;
        }

        public bool CallBaseConstructor { get; set; }
    }
}
