using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ApiVault.Views
{
    public partial class DisconnectPageView : UserControl
    {
        public DisconnectPageView()
        {
            InitializeComponent();
        }

        // Navigate to sign up view
        private void NavToSignUpView(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window mainWindow)
            {
                mainWindow.Content = new LoginView();
            }
        }
    }
}
