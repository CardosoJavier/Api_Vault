using ApiVault.DataModels;
using Cassandra;
using ReactiveUI;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class SignUpViewModel : ReactiveObject, IRoutableViewModel
    {
        // Routing interface needed properties.
        // Sets the view identifier to "signUp"
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "signUp";

        // pre-compile validation Regex
        private static readonly Regex hasNumber = new Regex(@"[0-9]+", RegexOptions.Compiled);
        private static readonly Regex hasSpecialChar = new Regex(@"[!@#$%^&*()_+<>?]+", RegexOptions.Compiled);

        // New user variables for Data Binding
        private string username;
        public string Username
        {  get => username;
            set
            {
                this.RaiseAndSetIfChanged(ref username, value);
                UpdateCanSubmit();
            }
        }

        private string email;
        public string Email 
        {
            get => email;
            set
            {
                this.RaiseAndSetIfChanged(ref email, value);
                UpdateCanSubmit();
            }

        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                this.RaiseAndSetIfChanged(ref password, value);
                UpdateCanSubmit();
            }
        }

        private string confirmpassword;
        public string ConfirmPassword
        {
            get => confirmpassword;
            set
            { 
                this.RaiseAndSetIfChanged(ref confirmpassword, value); 
                UpdateCanSubmit(); 
            }
        }

        private string phone;
        public string Phone 
        {
            get => phone;
            set 
            { 
                this.RaiseAndSetIfChanged(ref phone, value);
                UpdateCanSubmit(); 
            }
        }

        public bool CanSubmit { get; set; } = false;

        // Database variables
        private AstraDbConnection dbConnection;
        private ISession session;

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
            dbConnection = new AstraDbConnection();
            InitializeAsync();
        }

        public SignUpViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        /* - - - - - - - - - - - Methods - - - - - - - - - - - */

        public async Task InitializeAsync()
        {

            await dbConnection.InitializeConnection();
            session = await dbConnection.GetSession();
        }

        /*
         * Create new user account
         */
        private async Task SignUp()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var session = dbConnection.GetSession().Result;
            try
            {
                // database connection
                var watchDb = System.Diagnostics.Stopwatch.StartNew();
                // var session = AstraDbConnection.GetSession().Result;
                watchDb.Stop();
                Debug.Print($"Connect to DB: {watchDb.ElapsedMilliseconds} ms");

                // Check for empty fields
                if (await verifyInput(Email, Username, Password, ConfirmPassword, Phone, session))
                {
                    // Insert user logic here
                    var success = await InsertUser(Email, Username, Password, Phone, session);
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

                // close database connection
            }

            catch 
            {
                Debug.Print("Error in database connection");
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Print($"Overall Elapsed {elapsedMs} ms");
        }


        /*
         * Insert new user in the database
         */
        private static async Task<bool> InsertUser(string email, string username, string password, string phone, ISession session)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                // Encrypt password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Add new user query
                var inserteUser = session.Prepare("INSERT INTO apivault_space.Users (username, email, password, phone) VALUES (?, ?, ?, ?)");
                session.Execute(inserteUser.Bind(username, email, hashedPassword, phone));

                // TODO: Clear fields after successfull sign up

                // TODO: Verify user was inserted in table correctly
                var veryfyUserQuery = session.Prepare("SELECT * FROM apivault_space.Users WHERE username = ?");
                var checkNewUser = session.Execute(veryfyUserQuery.Bind(username));

                watch.Stop();
                var timePassed = watch.ElapsedMilliseconds;
                Debug.Print($"Insert User Elapsed [t]: {timePassed} ms");

                // Check if user was created
                return checkNewUser.Any();
            }

            catch 
            {
                watch.Stop();
                Debug.Print($"Insert User Elapsed [C]: {watch.ElapsedMilliseconds * 100} ms");
                // TODO: Handle not successfull insert
                return false;
            }
        }

        // verify username, email, phone number, and password
        private async Task<bool> verifyInput(string Email, string Username, string Password, string ConfirmPassword, string Phone, ISession session)
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) ||
                string.IsNullOrEmpty(Phone))
            {
                StatusMessage = "One or more fields are empty";
                Debug.Print("One or more fields are empty");
                return false;
            }

            // check username length 
            else if (Username.Length < 4)
            {
                StatusMessage = "Username must have at least 4 characters";
                Debug.Print("Username must have at least 4 characters");
                return false;
            }

            // check username availability
            else if (await checkUsernameAvailability(Username, session))
            {
                StatusMessage = Username + " is already taken";
                Debug.Print("Username + \" is already taken\"");
                return false;
            }

            // Check for matching passwords
            else if (Password != ConfirmPassword)
            {
                StatusMessage = "Passwords are not the same";
                Debug.Print("Passwords are not the same");
                return false;
            }

            // Check for special characters and numbers in passwords
            else if (!IsValidPassword(Password))
            {
                StatusMessage = "Password needs at least one number and special character";
                Debug.Print("Password needs at least one number and special characters");
                return false;
            }

            // Check if password has right length ( 8 > 0)
            else if (Password.Length < 8)
            {
                StatusMessage = "Password must have at least 8 characters";
                Debug.Print("Password must have at least 8 characters");
                return false;
            }

            return true;
        }

        // Check if password has special characters and numbers
        private bool IsValidPassword(string password)
        { 
            return hasNumber.IsMatch(password) && hasSpecialChar.IsMatch(password);
        }

        // Check username availability
        private async Task<bool> checkUsernameAvailability(string username, ISession session)
        {
            var reponse = Task.Run(() =>
            {
                // search username queries
                var seachUsernameQuery = session.Prepare("SELECT * FROM Users WHERE username = ?");
                var searchUsernameResult = session.Execute(seachUsernameQuery.Bind(username));

                // return search result
                return searchUsernameResult.Any();
            });

            return await reponse;
        }

        // Enables user to submit once all form fields are filled
        private void UpdateCanSubmit()
        {
            CanSubmit = !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Email)
                        && !string.IsNullOrWhiteSpace(Password)
                        && !string.IsNullOrWhiteSpace(ConfirmPassword)
                        && !string.IsNullOrWhiteSpace(Phone);

            this.RaisePropertyChanged(nameof(CanSubmit));
        }
    }
}
