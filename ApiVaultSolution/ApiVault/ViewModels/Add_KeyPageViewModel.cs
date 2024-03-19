using ApiVault.DataModels;
using ApiVault.Services;
using Cassandra;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
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

        public ObservableCollection<string> _apiGroups = new ObservableCollection<string>
        {
            "Group 1",
            "Group 2",
            "Group 3"
        };

        public ObservableCollection<string> ApiGroups
        {
            get => _apiGroups;
            set => this.RaiseAndSetIfChanged(ref _apiGroups, value);
        }

        private string? apiGroup;
        public string? ApiGroup
        {
            get => apiGroup;
            set
            {
                this.RaiseAndSetIfChanged(ref apiGroup, value);
                UpdateCanSubmit();
            }
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

            AddKeyCommand = ReactiveCommand.Create(AddKey);
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
            }
        }

        private void AddKey() 
        {
            // generate UUID
            var UUID = Guid.NewGuid();

            // Expiration date
            var expirationDate = GenerateExpirationDate();

            var inserteUser = session.Prepare("INSERT INTO apivault_space.apikeys (keyid,  apigroup, apikey, apiname, replacedate, username) VALUES (?, ?, ?, ?, ?, ?)");
            session.Execute(inserteUser.Bind(UUID, apiGroup, apiKey, apiName, expirationDate, _userSessionService.Username));
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
                        && !string.IsNullOrWhiteSpace(ApiGroup);

            this.RaisePropertyChanged(nameof(CanSubmit));
        }

    }
}
