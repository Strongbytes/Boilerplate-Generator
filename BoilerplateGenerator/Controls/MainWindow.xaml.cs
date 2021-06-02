using BoilerplateGenerator.Domain;
using System.Windows;

namespace BoilerplateGenerator.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IViewModelBase _viewModelBase;

        public MainWindow(IViewModelBase viewModelBase)
        {
            InitializeComponent();

            _viewModelBase = viewModelBase;
            DataContext = _viewModelBase;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            await _viewModelBase.PopulateSolutionProjects();
        }
    }
}
