using ApiVault.DataModels;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
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
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) ||
                string.IsNullOrEmpty(Phone))
            {
                StatusMessage = "One or more fields are empty/null";
                Debug.Print("One or more fields are empty/null");
                return;
            }

            else if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords are not the same";
                Debug.Print("Passwords are not the same");
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
    }
}
