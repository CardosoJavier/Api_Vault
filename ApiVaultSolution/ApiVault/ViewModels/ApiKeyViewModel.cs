using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
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

        private bool _showFullApiKey;
        public bool ShowFullApiKey
        {
            get => _showFullApiKey;
            set => this.RaiseAndSetIfChanged(ref _showFullApiKey, value);
        }

        public string DisplayApiKey => ApiKey != null ? new string('*', ApiKey.Length - 4) + ApiKey.Substring(ApiKey.Length - 4) : string.Empty;

        public string VisibleApiKey => ShowFullApiKey ? ApiKey : DisplayApiKey;

        public ReactiveCommand<Unit, Unit> ToggleApiKeyVisibilityCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteKey { get; }



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
            ToggleApiKeyVisibilityCommand = ReactiveCommand.Create(ToggleApiKeyVisibility);
        }


        private void ToggleApiKeyVisibility()
        {
            _showFullApiKey = !_showFullApiKey;
            this.RaisePropertyChanged(nameof(VisibleApiKey));
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
