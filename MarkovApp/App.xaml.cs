using MarkovApp.Configuration;
using MarkovApp.Infrastructure;
using MarkovApp.Services;
using MarkovApp.Services.Interfaces;
using MarkovApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace MarkovApp
{
    public partial class App : Application
    {
        private IConfiguration _configuration = null!;
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup DI container
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Create and show main window
            var mainVM = _serviceProvider.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow
            {
                DataContext = mainVM
            };
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register configuration sections
            services.Configure<GraphSettings>(_configuration.GetSection("GraphSettings"));
            services.Configure<CalculationSettings>(_configuration.GetSection("CalculationSettings"));

            // Initialize static configuration accessor
            var graphSettings = _configuration.GetSection("GraphSettings").Get<GraphSettings>()!;
            var calcSettings = _configuration.GetSection("CalculationSettings").Get<CalculationSettings>()!;
            AppConfig.Initialize(
                Microsoft.Extensions.Options.Options.Create(graphSettings),
                Microsoft.Extensions.Options.Options.Create(calcSettings)
            );

            // Register services
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IValidationService, ValidationService>();
            services.AddSingleton<IAppStateService, AppStateService>();
            services.AddSingleton<IGraphDataService, GraphDataService>();
            services.AddSingleton<IMarkovCalculatorService, MarkovCalculatorService>();
            services.AddSingleton<IGraphLogicService, GraphLogicService>();

            // Register ViewModels
            services.AddTransient<GraphViewModel>();
            services.AddTransient<MainViewModel>();
        }
    }
}