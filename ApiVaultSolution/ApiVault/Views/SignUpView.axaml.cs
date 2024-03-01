using ApiVault.DataModels;
using ApiVault.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace ApiVault.Views
{
    public partial class SignUpView : UserControl
    {
        // Constructor
        public SignUpView()
        {
            InitializeComponent();
            DataContext = new SignUpViewModel();    
        }

        /*
         * Navigate to sign in by reasingning main window content
         */
        public void NavToSignIn(object source, RoutedEventArgs args)
        {
            if (this.Parent is Window window)
            {
                window.Content = new LoginView();
            }
        }
    }
}
