using ApiVault.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ApiVault.Views
{
    public partial class LoginView : UserControl
    {
        // Constructor to initialize view component
        public LoginView()
        {
            InitializeComponent();
            var loginViewModel = new LoginViewModel();
            DataContext = loginViewModel;

            loginViewModel.LoginSuccessful += (sender, e) => NavToDashboard();
        }

        private void NavToDashboard()
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new AppContentView(); // Ensure you have a DashboardView defined
            }
        }

        
        // Navigate to sign up view by reasingning main window content
        private void NavToSignUpView(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new SignUpView();
            }
        }

        /*
        // Navigate to dashboard when login success by reasingning main window content
        private void NavToDashboard(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new AppContentView();
            }
        }
        */
    }
}
