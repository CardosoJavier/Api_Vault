using ApiVault.DataModels;
using ApiVault.Services;
using Cassandra;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        /* - - - - - - - - - - Binding View Variables - - - - - - - - - - */
        private string? username;
        public string? Username
        {
            get => username;
            set
            {
                this.RaiseAndSetIfChanged(ref username, value);
                UpdateCanSubmit();
            }
        }

        private string? password;
        public string? Password
        {
            get => password;
            set
            {
                this.RaiseAndSetIfChanged(ref password, value);
                UpdateCanSubmit();
            }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }
        public bool CanSubmit { get; set; } = false;

        private AstraDbConnection dbConnection;
        private ISession InitPoolSession;

        public event EventHandler? LoginSuccessful;

        private readonly IUserSessionService _userSessionService;


        /* - - - - - - - - - - Constructors - - - - - - - - - - */
        public LoginViewModel(IUserSessionService userSessionService)
        {
            //HostScreen = screen;
            _userSessionService = userSessionService;

            // initialze database connection
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
            dbConnection = new AstraDbConnection();
            InitializeAsync();
        }

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }


        /* - - - - - - - - - - Methods - - - - - - - - - - */

        // Connect to database
        public async Task InitializeAsync()
        {
            await dbConnection.InitializeConnection();
            InitPoolSession = await dbConnection.GetSession();
        }

        // Function that checks credentials
        private async Task Login()
        {
            // check if credentials are valid
            bool validCredentials = await verifyCredentials(Username, Password);

            // move to dashboard or display error message
            if (validCredentials)
            {
                Debug.Print("Access granted");
                _userSessionService.Username = username;
                OnLoginSuccess();
            }

            else
            {
                StatusMessage = "Wrong username or password";
            }
        }

        // verify credentials
        private async Task<bool> verifyCredentials(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty");
            }

            // Prepare the query
            var query = "SELECT password FROM users WHERE username = ?";
            var prepareStatement = await InitPoolSession.PrepareAsync(query).ConfigureAwait(false);
            var bindStatement = prepareStatement.Bind(username);

            // execute query
            var exeQuery = await InitPoolSession.ExecuteAsync(bindStatement);

            // get first row from returning set
            var rowData =  exeQuery.FirstOrDefault();

            if (rowData != null)
            {
                // get password data
                var rowPassword = rowData.GetValue<string>("password");

                // compare passwords
                return BCrypt.Net.BCrypt.Verify(password, rowPassword);
            }

            // return false if username does not exist
            return false;
        }


        // Enables user to submit once all form fields are filled
        private void UpdateCanSubmit()
        {
            CanSubmit = !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Password);

            this.RaisePropertyChanged(nameof(CanSubmit));
        }

        // login event
        private void OnLoginSuccess()
        {
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }

    }
}
