using ApiVault.DataModels;
using ApiVault.Services;
using Cassandra;
using ReactiveUI;
using System;
using System.Data.Common;
using System.Dynamic;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class DashboardPageViewModel : ViewModelBase
    {
        // - - - - - - - - - - Class Varibles - - - - - - - -  - - 
        private readonly IUserSessionService _userSessionService;
        public string? UserName { get; set; } = string.Empty;

        // Db connection
        private AstraDbConnection dbConnection;
        private ISession session;

        // - - - - - - - - - - Constructors - - - - - - - -  - - 
        public DashboardPageViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            UserName = _userSessionService.Username;

            dbConnection = new AstraDbConnection();
            InitializeAsync();

            /*
            if (dbConnection != null && session != null) 
            {
                GetAllApiKeys();
            }
            */
        }

        // - - - - - - - - - - Methods - - - - - - - -  - - 
        // Connect to database
        public async Task InitializeAsync()
        {
            await dbConnection.InitializeConnection();
            session = await dbConnection.GetSession();
        }

        
        private void GetAllApiKeys()
        {
            var query = session.Prepare("SELECT apiname, apikey, apigroup, replacedate FROM apikeys WHERE username = ? ALLOW FILTERING");
            var apiKeys = session.Execute(query.Bind(_userSessionService.Username));

            // TODO: Print table
            // Print header
            Console.WriteLine($"{"Api Name",-30} {"Api Key",-36} {"Api Group",-20} {"Replace Date",-30}");

            // Iterate through the RowSet and print each row
            foreach (var row in apiKeys)
            {
                var apiName = row.GetValue<string>("apiname");
                var apiKey = row.GetValue<string>("apikey");
                var apiGroup = row.GetValue<string>("apigroup");
                var replaceDate = row.GetValue<DateTimeOffset>("replacedate").ToString("yyyy-MM-dd HH:mm:ss zzz");

                // Print table row
                Console.WriteLine($"{apiName,-30} {apiKey,-36} {apiGroup,-20} {replaceDate,-30}");
            }
        }

    }
}
