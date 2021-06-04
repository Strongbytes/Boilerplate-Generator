using BoilerplateGenerator.Domain;

namespace BoilerplateGenerator.Models.ClassGeneratorModels
{
    public class DomainEntityGeneratorModel : GenericGeneratorModel
    {
        public const string DomainEntitySuffix = "DomainModel";

        public override string RootClassName => $"{RootClass.Name}{DomainEntitySuffix}";

        public override string Namespace => $"{base.Namespace}.Domain.Models";

        public override AssetKind AssetKind => AssetKind.DomainEntity;

        public DomainEntityGeneratorModel(IViewModelBase viewModelBase) : base(viewModelBase)
        {
        }
    }
}
