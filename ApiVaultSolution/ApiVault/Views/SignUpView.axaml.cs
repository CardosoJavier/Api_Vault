using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ApiVault.Views
{
    public partial class SignUpView : UserControl
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        // Navigate to sign in by reasingning main window content
        public void NavToSignIn(object source, RoutedEventArgs args)
        {
            if (this.Parent is Window window)
            {
                window.Content = new LoginView();
            }
        }
    }
}
