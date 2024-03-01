using ApiVault.DataModels;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class SignUpViewModel : ReactiveObject, IRoutableViewModel
    {
        // Routing interface needed properties.
        // Sets the view identifier to "login"
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "signUp";

        // New user variables for Data Binding
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        // public string StatusMessage { get; set; }
        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; }

        /* - - - - - - - - - - - Constructors - - - - - - - - - - - */
        public SignUpViewModel()
        {
            SignUpCommand = ReactiveCommand.CreateFromTask(SignUp);
        }

        public SignUpViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        /* - - - - - - - - - - - Methods - - - - - - - - - - - */

        /*
         * Create new user account
         */
        private async Task SignUp()
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) ||
                string.IsNullOrEmpty(Phone))
            {
                StatusMessage = "One or more fields are empty";
                Debug.Print("One or more fields are empty");
                return;
            }

            // Check for matching passwords
            else if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords are not the same";
                Debug.Print("Passwords are not the same");
                return;
            }

            // Check for special characters and numbers in passwords
            else if (!IsValidPassword(Password))
            {
                StatusMessage = "Password needs at least one number and special character";
                Debug.Print("Password needs at least one number and special characters");
                return;
            }

            // Check if password has right length ( 8 > 0)
            else if (Password.Length < 8)
            {
                StatusMessage = "Password must have at least 8 characters";
                Debug.Print("Password must have at least 8 characters");
                return;
            }

            else
            {
                // Insert user logic here
                var success = await InsertUser(Email, Username, Password, Phone);
                if (success)
                {
                    // Navigate to sign in or update UI accordingly
                    StatusMessage = "User created successfully";
                    Debug.Print("User created successfully");

                    Email = String.Empty;
                    Username = String.Empty;
                    Password = String.Empty;

                }
                else
                {
                    StatusMessage = "Failed to create user";
                    Debug.Print("Failed to create user");
                }
            }
        }


        /*
         * Insert new user in the database
         */
        private static async Task<bool> InsertUser(string email, string username, string password, string phone)
        {
            try
            {
                // Stablish database connection
                var session = await AstraDbConnection.Connect();

                // New user query
                var inserteUser = session.Prepare("INSERT INTO apivault_space.Users (username, email, password, phone) VALUES (?, ?, ?, ?)");
                session.Execute(inserteUser.Bind(username, email, password, phone));

                // TODO: Verify user was inserted in table correctly

                // TODO: Clear fields after successfull sign up
               
                return true;
            }

            catch 
            {
                // TODO: Handle not successfull insert
                return false;
            }
        }

        // Check if password has special characters and numbers
        public bool IsValidPassword(string password)
        {
            // Check for a number
            var hasNumber = new Regex(@"[0-9]+");

            // Check for a special character
            var hasSpecialChar = new Regex(@"[!@#$%^&*()_+<>?]+");

            return hasNumber.IsMatch(password) && hasSpecialChar.IsMatch(password);
        }
    }
}
