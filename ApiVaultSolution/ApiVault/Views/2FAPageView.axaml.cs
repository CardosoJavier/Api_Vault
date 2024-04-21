using ApiVault.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;

namespace ApiVault.Views
{
    public partial class _2FAPageView : UserControl
    {
        public _2FAPageView()
        {
            InitializeComponent();

            // Dependency injection
            DataContext = App.ServiceProvider.GetService<_2FAPageViewModel>();

            if (DataContext is _2FAPageViewModel viewModel)
            {
                viewModel.LoginSuccessful += (sender, e) => NavToDashboard();
            }
        }

        // Navigates to dashboard
        private void NavToDashboard()
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new AppContentView(); // Ensure you have a DashboardView defined
            }
        }

        // Navigate to sign up view by reasingning main window content
        private void NavToLogin(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new LoginView();
            }
        }
    }
}
