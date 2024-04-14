using ReactiveUI;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class ApiKeyViewModel: ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> DeleteKey { get; }

        private Guid? _primaryKey;
        public Guid? PrimaryKey
        {
            set => this.RaiseAndSetIfChanged(ref _primaryKey, value);
            get => _primaryKey;
        }

        private string? _apiKeyName;
        public string? ApiKeyName 
        { set => this.RaiseAndSetIfChanged(ref _apiKeyName, value);
          get => _apiKeyName; 
        }

        private string? _apiKey;
        public string? ApiKey
        {
            set => this.RaiseAndSetIfChanged(ref _apiKey, value);
            get => _apiKey;
        }

        private string? _group;
        public string? Group
        {
            set => this.RaiseAndSetIfChanged(ref _group, value);
            get => _group;
        }

        // ReplaceDate
        private string? _replaceDate;
        public string? ReplaceDate
        {
            set => this.RaiseAndSetIfChanged(ref _replaceDate, value);
            get => _replaceDate;
        }

        /* - - - - - - - - - - HTTP Variables - - - - - - - - - - */
        // Environment variables should be securely retrieved, for example, from a configuration file or environment settings
        private readonly string? astraDbId = Environment.GetEnvironmentVariable("ASTRA_DB_ID");
        private readonly string? astraDbRegion = Environment.GetEnvironmentVariable("ASTRA_DB_REGION");
        private readonly string? astraDbKeyspace = Environment.GetEnvironmentVariable("ASTRA_DB_KEYSPACE");
        private readonly string? astraDbApplicationToken = Environment.GetEnvironmentVariable("ASTRA_DB_APPLICATION_TOKEN");

        HttpClient httpClient = new HttpClient();

        public ApiKeyViewModel(Guid IdKey, string name, string key, string apiGroup, string apiReplaceData)
        {
            PrimaryKey = IdKey;
            ApiKeyName = name;
            ApiKey = key;
            Group = apiGroup;
            ReplaceDate = apiReplaceData;
            DeleteKey = ReactiveCommand.CreateFromTask(DeleteKeyAsync);
        }

        public async Task DeleteKeyAsync()
        {
            if (astraDbId != null && astraDbRegion != null && astraDbKeyspace != null && astraDbApplicationToken != null && PrimaryKey != null)
            {
                var astraDbService = new AstraDbService(httpClient, astraDbId, astraDbRegion, astraDbKeyspace, astraDbApplicationToken);

                bool success = await astraDbService.DeleteApiKeyAsync("apikeys", PrimaryKey.ToString());

                if (success)
                {
                    Debug.WriteLine("Key deleted!");
                }

                else
                {
                    Debug.WriteLine("Failed deleting!");
                }
            }

            else
            {
                Debug.WriteLine("Request variables are null");
            }
        }

    }
}
