using ApiVault.DataModels;
using Cassandra;
using ReactiveUI;
using System.Data.Common;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class Add_KeyPageViewModel : ViewModelBase
    {
        /* - - - - - - - - - - - Class variables - - - - - - - - - - - */
        private string? apiName;
        private string? ApiName {  get; set; }

        private string? apiKey;
        private string? ApiKey { get; set; }

        private string? apiGroup;
        private string? ApiGroup { get; set; }

        // database connections
        private AstraDbConnection? dbConnection;
        private ISession? session;

        /* - - - - - - - - - - - Constructors - - - - - - - - - - - */
        public Add_KeyPageViewModel()
        {
            AddKeyCommand = ReactiveCommand.Create(AddKey);
            dbConnection = new AstraDbConnection();
            InitializeAsync();
        }

        /* - - - - - - - - - - - Commands - - - - - - - - - - - */
        // Command for Sign Up
        private ReactiveCommand<Unit, Unit> AddKeyCommand { get; }


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

        }

    }
}
