using ApiVault.Services;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        /* - - - - - - - - - - Binding variables - - - - - - - - - - */
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

        private string? statusMessage;
        public string? StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }

        /* - - - - - - - - - - Events and session - - - - - - - - - - */
        public bool? CanSubmit { get; set; } = false;
        public event EventHandler? LoginSuccessful;
        private readonly IUserSessionService _userSessionService;

        /* - - - - - - - - - - HTTP Client - - - - - - - - - - */
        HttpClient httpClient = new HttpClient();

        private readonly string? astraDbId = Environment.GetEnvironmentVariable("ASTRA_DB_ID");
        private readonly string? astraDbRegion = Environment.GetEnvironmentVariable("ASTRA_DB_REGION");
        private readonly string? astraDbKeyspace = Environment.GetEnvironmentVariable("ASTRA_DB_KEYSPACE");
        private readonly string? astraDbApplicationToken = Environment.GetEnvironmentVariable("ASTRA_DB_APPLICATION_TOKEN");
        private readonly string _table = "users";

        /* - - - - - - - - - - Constructors - - - - - - - - - - */
        public LoginViewModel(IUserSessionService userSessionService)
        {
            //HostScreen = screen;
            _userSessionService = userSessionService;

            // initialze database connection
            LoginCommand = ReactiveCommand.CreateFromTask(Login);
        }

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }


        /* - - - - - - - - - - Methods - - - - - - - - - - */

        // Function that checks credentials
        private async Task Login()
        {
            // check if credentials are valid
            if(Username != null && Password != null)
            {
                bool validCredentials = await verifyCredentials(Username, Password);

                // move to dashboard or display error message
                if (validCredentials && username != null)
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

            else
            {
                Debug.WriteLine("Username or passward are null");
                return;
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

            // Set up request
            if (astraDbId != null && astraDbRegion != null && astraDbKeyspace != null && astraDbApplicationToken != null)
            {
                AstraDbService dbService = new AstraDbService(httpClient, astraDbId, astraDbRegion, astraDbKeyspace, astraDbApplicationToken);

                // Get request
                HttpContent? getCredentialRequest = await dbService.GetUserCredentials(_table, username);

                // verify content
                if (getCredentialRequest == null)
                {
                    Debug.WriteLine("Null GET request content");
                    return false;
                }

                // Keep with verification
                else
                {
                    // Get request content
                    string getContent = await getCredentialRequest.ReadAsStringAsync();

                    JObject credentials = JObject.Parse(getContent);

                    // check for any result
                    if (int.Parse((string)credentials["count"]) == 0)
                    {
                        return false;
                    }

                    else
                    {

                        // verify password
                        if (BCrypt.Net.BCrypt.Verify(password, (string)credentials["data"][0]["password"]))
                        {
                            _userSessionService.Phone = (string)credentials["data"][0]["phone"];
                            return true;
                        }

                        return false;
                    }
                }
            }

            else
            {
                Debug.WriteLine("Problem with GET request variables");
                return false;
            }
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
