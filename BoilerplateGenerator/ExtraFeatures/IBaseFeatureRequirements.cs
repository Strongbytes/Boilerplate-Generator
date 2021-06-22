using System.ComponentModel;
using System.Threading.Tasks;

namespace BoilerplateGenerator.ExtraFeatures
{
    public interface IBaseFeatureRequirements : INotifyPropertyChanged
    {
        bool? FeatureIsAvailable { get; }

        Task RetrieveFeatureRequirements();
    }
}
