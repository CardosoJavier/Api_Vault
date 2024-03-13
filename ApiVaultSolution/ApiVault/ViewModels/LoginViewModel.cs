using ApiVault.DataModels;
using Cassandra;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        /* - - - - - - - - - - Routing - - - - - - - - - - */
        // Routing interface needed properties.
        // Sets the view identifier to "login"
        public IScreen? HostScreen { get; }
        public string? UrlPathSegment { get; } = "login";


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

        /* - - - - - - - - - - Constructors - - - - - - - - - - */
        public LoginViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        public LoginViewModel() 
        {
            // initialze database connection
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
            dbConnection = new AstraDbConnection();
            InitializeAsync();

        }

        ~LoginViewModel()
        {
            dbConnection.DisposeDb();
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
            // get session form pool session
            var session = dbConnection.GetSession().Result;

            // check if credentials are valid
            bool validCredentials = await verifyCredentials(Username, Password, session);

            // move to dashboard or display error message
            if (validCredentials)
            {
                Debug.Print("Access granted");
                return;
            }

            StatusMessage = "Wrong username or password";
        }

        // verify credentials
        private async Task<bool> verifyCredentials(string username, string password, ISession session)
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
            var prepareStatement = await session.PrepareAsync(query).ConfigureAwait(false);
            var bindStatement = prepareStatement.Bind(username);

            // execute query
            var exeQuery = await session.ExecuteAsync(bindStatement);

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
    }
}
