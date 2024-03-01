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

        /*
         * Get input data from form and create a new account
         */
        public void CreateAccount(object source, RoutedEventArgs args) 
        {
            if (!string.IsNullOrEmpty(Email.Text) && !string.IsNullOrEmpty(Username.Text) && !string.IsNullOrEmpty(Password.Text) && !string.IsNullOrEmpty(ConfirmPassword.Text) && !string.IsNullOrEmpty(Phone.Text))
            {
                // user input (order in table)
                string username = Username.Text;
                string email = Email.Text;
                string confirmPassword = ConfirmPassword.Text;
                string password = Password.Text;
                string phone = Phone.Text;

                /* - - - - Verify user input - - - - */

                // TODO: check that email is unique

                // TODO: check that username is unique

                // Todo: Check both passwords are the same
                if (!string.Equals(password, confirmPassword))
                {
                    Debug.Print("Passwords are not the same");
                    return;
                }

                /* - - - - Register new user after field verification - - - - */
                _ = SignUpViewModel.InsertUser(email, username, password, phone);
            }

            else
            {
                // TODO: Handle null or empty input fields in UI
                Debug.Print("One or more fields are empty/null");
            }
        }
    }
}
