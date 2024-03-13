using ApiVault.DataModels;
using ApiVault.Services;
using ApiVault.ViewModels;
using ApiVault.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace ApiVault
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            // Connect to database before launching application
            // AstraDbConnection.Connect();

            base.OnFrameworkInitializationCompleted();
        }
    }
}