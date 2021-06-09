namespace BoilerplateGenerator.Contracts
{
    public interface IBaseSymbolWrapper
    {
        string Name { get; set; }

        bool? IsChecked { get; set; }
    }
}
