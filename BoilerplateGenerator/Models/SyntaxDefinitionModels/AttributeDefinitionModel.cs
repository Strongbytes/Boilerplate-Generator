namespace BoilerplateGenerator.Models.SyntaxDefinitionModels
{
    public class AttributeDefinitionModel
    {
        public string Name { get; set; }

        public string[] Values { get; set; } = new string[] { };

        public AttributeDefinitionModel(string name)
        {
            Name = name;
        }
    }
}
