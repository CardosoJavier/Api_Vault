using ApiVault.Services;
using ApiVault.ViewModels;
using ApiVault.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiVault
{
    public partial class App : Application
    {
        // Static property to access ServiceProvider
        public static IServiceProvider ServiceProvider { get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            // Register all services needed for application to run
            var collection = new ServiceCollection();
            collection.AddCommonServices();

            // ServiceProvider
            ServiceProvider = collection.BuildServiceProvider();

            var vm = ServiceProvider.GetRequiredService<MainWindowViewModel>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainWindow
                {
                    DataContext = vm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}