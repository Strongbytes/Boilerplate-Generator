namespace BoilerplateGenerator.Models.Contracts
{
    public interface IGeneratedClass : IBaseGeneratedAsset
    {
        string Code { get; }

        string[] ParentDirectoryHierarchy { get; }
    }
}
