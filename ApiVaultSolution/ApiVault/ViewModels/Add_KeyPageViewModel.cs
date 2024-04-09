using ApiVault.DataModels;
using ApiVault.Services;
using Avalonia.Controls;
using Cassandra;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class Add_KeyPageViewModel : ViewModelBase
    {
        /* - - - - - - - - - - - Class variables - - - - - - - - - - - */
        private string? apiName;
        public string? ApiName
        {
            get => apiName;
            set
            {
                this.RaiseAndSetIfChanged(ref apiName, value);
                UpdateCanSubmit();
            }
        }

        private string? apiKey;
        public string? ApiKey
        {
            get => apiKey;
            set
            {
                this.RaiseAndSetIfChanged(ref apiKey, value);
                UpdateCanSubmit();
            }
        }

        public ObservableCollection<string>? Groups { get; }

        private string? _selectedGroup;
        public string? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (value != null)
                {
                    this.RaiseAndSetIfChanged(ref _selectedGroup, value);
                    // If a group is selected, clear the NewGroup
                    NewGroup = string.Empty;
                    UpdateCanSubmit();
                }
                else
                {
                    // This handles clearing the selection or resetting filters
                    this.RaiseAndSetIfChanged(ref _selectedGroup, value);
                }
            }
        }

        private string? _newGroup;
        public string? NewGroup
        {
            get => _newGroup;
            set
            {
                // Only proceed if the new value is different from the current one
                if (_newGroup != value)
                {
                    this.RaiseAndSetIfChanged(ref _newGroup, value);
                    // If a new group is typed, clear the selected group
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        SelectedGroup = null;
                    }
                    UpdateCanSubmit();
                }
            }
        }

        

        private string statusMessage;
        public string StatusMessage
        {
            get => statusMessage;
            set => this.RaiseAndSetIfChanged(ref statusMessage, value);
        }

        public bool CanSubmit { get; set; } = false;

        private readonly IUserSessionService _userSessionService;

        // database connections
        private AstraDbConnection? dbConnection;
        private ISession? session;

        /* - - - - - - - - - - - Constructors - - - - - - - - - - - */
        public Add_KeyPageViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            // Initialize DB connection and session
            dbConnection = new AstraDbConnection();
            InitializeAsync();

            Groups = new ObservableCollection<string>();

            AddKeyCommand = ReactiveCommand.CreateFromTask(AddKey);
        }

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        public ReactiveCommand<Unit, Unit> AddKeyCommand { get; }


        /* - - - - - - - - - - Methods - - - - - - - - - - */

        // Connect to database
        private async Task InitializeAsync()
        {
            if (dbConnection != null) 
            {
                await dbConnection.InitializeConnection();
                session = await dbConnection.GetSession();

                await GetApiKeyGroups();
            }
        }

        private async Task GetApiKeyGroups()
        {
            await Task.Run(() =>
            {
                var query = session.Prepare("SELECT apigroup FROM apikeys WHERE username = ? ALLOW FILTERING");
                var apiKeys = session.Execute(query.Bind(_userSessionService.Username));

                // Iterate through the RowSet and print each row
                Groups.Add(string.Empty);
                foreach (var row in apiKeys)
                {
                    var apiGroup = row.GetValue<string>("apigroup");

                    if (!Groups.Contains(apiGroup))
                    {
                        Groups.Add(apiGroup);
                    }
                }

                Groups.OrderDescending();
            });
        }

        private async Task AddKey() 
        {

            var tryInsert = await InsertKey();

            if (tryInsert)
            {
                SelectedGroup = string.Empty;
                NewGroup = string.Empty;
                ApiKey = string.Empty;
                ApiName = string.Empty;

                StatusMessage = "Key stored successfully";
            }

            else
            {
                StatusMessage = "Failed to store api key";
            }

        }

        private async Task<bool> InsertKey()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // generate UUID
                    var UUID = Guid.NewGuid();

                    // Expiration date
                    var expirationDate = GenerateExpirationDate();

                    // Get group
                    var apiGroups = "No group";
                    if (!string.IsNullOrEmpty(NewGroup))
                    {
                        apiGroups = NewGroup;
                    }

                    else
                    {
                        apiGroups = SelectedGroup;
                    }

                    var inserteUser = session.Prepare("INSERT INTO apivault_space.apikeys (keyid,  apigroup, apikey, apiname, replacedate, username) VALUES (?, ?, ?, ?, ?, ?)");
                    session.Execute(inserteUser.Bind(UUID, apiGroups, apiKey, apiName, expirationDate, _userSessionService.Username));

                    // Verify added key
                    var veryfyNewKey = session.Prepare("SELECT * FROM apivault_space.apikeys WHERE keyid = ?");
                    var checkNewUser = session.Execute(veryfyNewKey.Bind(UUID));

                    return checkNewUser.Any();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            });
        }

        // Expiration date calculation
        private DateTime GenerateExpirationDate(int daysToAdd = 90)
        {
            DateTime currentDate = DateTime.Now;
            DateTime expirationDate = currentDate.AddDays(daysToAdd);
            return expirationDate;
        }

        // Enables user to submit once all form fields are filled
        private void UpdateCanSubmit()
        {
            CanSubmit = !string.IsNullOrWhiteSpace(ApiName)
                && !string.IsNullOrWhiteSpace(ApiKey)
                && ((SelectedGroup != null && SelectedGroup != string.Empty) || !string.IsNullOrWhiteSpace(NewGroup));

            this.RaisePropertyChanged(nameof(CanSubmit));
        }

    }
}
