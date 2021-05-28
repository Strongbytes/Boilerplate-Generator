using BoilerplateGenerator.Controls;
using BoilerplateGenerator.Domain;
using BoilerplateGenerator.Helpers;
using BoilerplateGenerator.Services;
using BoilerplateGenerator.ViewModels;
using EnvDTE;
using EnvDTE80;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace BoilerplateGenerator
{
    internal sealed class GenerateCRUD
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("09f3f347-dea0-420f-a993-74a311698f7b");

        private readonly AsyncPackage _package;

        private GenerateCRUD(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static GenerateCRUD Instance
        {
            get;
            private set;
        }

        private IServiceProvider _serviceProvider;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GenerateCRUD(package, commandService);
        }

        private void ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(typeof(IViewModelBase), typeof(ViewModelBase));
            services.AddSingleton(typeof(DTE2), _package.GetService<DTE>() as DTE2);
            services.AddSingleton(typeof(IEntityManagerService), typeof(EntityManagerService));
            services.AddSingleton(typeof(ITypeResolutionService), _package.GetTypeResolutionService());
            services.AddSingleton(typeof(ITypeDiscoveryService), _package.GetTypeDiscoveryService());
            services.AddSingleton<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private void Execute(object sender, EventArgs e)
        {
            ConfigureServices();
            ThreadHelper.ThrowIfNotOnUIThread();

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                _serviceProvider.GetService<MainWindow>().Show();
            }
        }
    }
}
