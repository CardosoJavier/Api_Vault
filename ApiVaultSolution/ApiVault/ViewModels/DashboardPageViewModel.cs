using ApiVault.DataModels;
using ApiVault.Services;
using Avalonia.Controls;
using Cassandra;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    internal class DashboardPageViewModel : ViewModelBase
    {
        // - - - - - - - - - - Class Varibles - - - - - - - -  - - 
        private readonly IUserSessionService _userSessionService;
        public string? UserName { get; set; } = string.Empty;

        // Display keys
        public ObservableCollection<ApiKeyViewModel> ApiKeysList { get; }
        public ObservableCollection<string> Groups { get; }


        private bool? _isLoading;
        public bool? IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        // Search, filter, and sort
        private string _filterCriteria;
        public string FilterCriteria
        {
            get => _filterCriteria;
            set
            {
                if (value != null)
                {
                    Debug.WriteLine($"Filter: {value}");
                    this.RaiseAndSetIfChanged(ref _filterCriteria, value);
                    
                    if (value != string.Empty) 
                    {
                        _ = ApplyFilterAsync();
                    }
                }
            }
        }


        private ComboBoxItem? _sortCriteria;
        public ComboBoxItem? SortCriteria
        {
            get => _sortCriteria;
            set
            {
                // The value was changed, now do something as a result
                if (value != null && value.Content is not null)
                {
                    Debug.WriteLine($"sort: {value.Content.ToString()}");
                    this.RaiseAndSetIfChanged(ref _sortCriteria, value);
                    _ = ApplySortAsync();
                }

                else
                {
                    this.RaiseAndSetIfChanged(ref _sortCriteria, null);
                }
            }
        }

        private string? _searchQuery = string.Empty;
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value != null)
                {
                    Debug.WriteLine($"search: {value}");
                    this.RaiseAndSetIfChanged(ref _searchQuery, value);
                    _ = ApplySearch();
                }
            }
        }

        // Http connection
        /* - - - - - - - - - - HTTP Client - - - - - - - - - - */
        HttpClient httpClient = new HttpClient();

        private readonly string? astraDbId = Environment.GetEnvironmentVariable("ASTRA_DB_ID");
        private readonly string? astraDbRegion = Environment.GetEnvironmentVariable("ASTRA_DB_REGION");
        private readonly string? astraDbKeyspace = Environment.GetEnvironmentVariable("ASTRA_DB_KEYSPACE");
        private readonly string? astraDbApplicationToken = Environment.GetEnvironmentVariable("ASTRA_DB_APPLICATION_TOKEN");
        private readonly string? _table = "apikeys";

        // Commands
        public ReactiveCommand<Unit, Unit> ResetFiltersCommand { get; }


        // - - - - - - - - - - Constructors - - - - - - - -  - - 
        public DashboardPageViewModel(IUserSessionService userSessionService)
        {   
            // session
            _userSessionService = userSessionService;
            UserName = _userSessionService.Username;

            // commands
            ResetFiltersCommand = ReactiveCommand.CreateFromTask(ResetFilters);
            
            // Dashboard Lists
            ApiKeysList = new ObservableCollection<ApiKeyViewModel>();
            Groups = new ObservableCollection<string>();

            // Get api keys
            GetAllApiKeys();

        }

        // - - - - - - - - - - Methods - - - - - - - -  - - 
        private async Task GetAllApiKeys()
        {
            if (astraDbId != null && astraDbRegion != null && astraDbKeyspace != null && astraDbApplicationToken != null && _table != null && _userSessionService.Username != null)
            {
                AstraDbService dbService = new AstraDbService(httpClient, astraDbId, astraDbRegion, astraDbKeyspace, astraDbApplicationToken);
                string getContent = await dbService.GetApiKeys(_table, _userSessionService.Username);

                // Convert content to JSON
                JObject keys = JObject.Parse(getContent);

                // Get API key object' data
                if (getContent != null)
                {
                    IsLoading = false;

                    for (int i = 0; i < int.Parse((string)keys["count"]); i++)
                    {
                        var keyId = (Guid)keys["data"][i]["keyid"];
                        var apiName = (string)keys["data"][i]["apiname"];
                        var apiKey = (string)keys["data"][i]["apikey"];
                        var apiGroup = (string)keys["data"][i]["apigroup"];
                        var replaceDate = (string)keys["data"][i]["replacedate"];

                        ApiKeysList.Add(new ApiKeyViewModel(keyId, apiName, apiKey, apiGroup, replaceDate));

                        if (!Groups.Contains(apiGroup))
                        {
                            Groups.Add(apiGroup);
                        }
                    }
                }

                else
                {
                    Debug.WriteLine("Null content from API fetch");
                }
            }

            else
            {
                Debug.WriteLine("Null variables in request");
                return;
            }
        }

        // Search method
        private async Task ApplySearch()
        {

            // Example filter application (implement based on actual properties and needs)
            ApiKeysList.Clear();
            await GetAllApiKeys();

            var searchValues = ApiKeysList
                .Where(apiKey => string.IsNullOrEmpty(SearchQuery) || apiKey.ApiKeyName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Now, update ApiKeysList based on filteredAndSorted data
            // Make sure to clear and add items back to notify UI for changes.
            ApiKeysList.Clear();
            foreach (var item in searchValues)
            {
                ApiKeysList.Add(item);
            }

            // Hangle empty bar
            if (_searchQuery == string.Empty) 
            {
                ApiKeysList.Clear();
                await GetAllApiKeys();
            }
        }

        // Filter keys
        private async Task ApplyFilterAsync()
        {
            // Ensure that we start with a fresh list
            ApiKeysList.Clear();
            await GetAllApiKeys();

            // If there's no filter set, just return
            if (string.IsNullOrWhiteSpace(_filterCriteria))
            {
                return;
            }

            // Apply the filter
            var filteredList = ApiKeysList
                .Where(apiKey => apiKey.Group.Equals(_filterCriteria, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Clear the list and add the filtered items
            ApiKeysList.Clear();
            foreach (var item in filteredList)
            {
                ApiKeysList.Add(item);
            }
        }


        // Sort method
        private async Task ApplySortAsync()
        {
            List<ApiKeyViewModel> sortedList = new();
            Debug.WriteLine(_sortCriteria);

            if (_sortCriteria is not null && _sortCriteria.Content is not null)
            {
                switch (_sortCriteria.Content.ToString())
                {
                    case "Name":
                        sortedList = ApiKeysList.OrderBy(apiKey => apiKey.ApiKeyName, StringComparer.Ordinal).ToList();
                        break;
                    case "Group":
                        // Ordering by ASCII values explicitly
                        sortedList = ApiKeysList.OrderBy(apiKey => apiKey.Group, StringComparer.Ordinal).ToList();
                        break;
                    case "Newest":
                        // Directly using DateTimeOffset for comparison, assuming ReplaceDate is already a DateTimeOffset
                        sortedList = ApiKeysList.OrderByDescending(apiKey => DateTimeOffset.ParseExact(apiKey.ReplaceDate, "yyyy-MM-dd HH:mm:ss", null)).ToList();
                        break;
                    case "Oldest":
                        sortedList = ApiKeysList.OrderBy(apiKey => DateTimeOffset.ParseExact(apiKey.ReplaceDate, "yyyy-MM-dd HH:mm:ss", null)).ToList();
                        break;
                    default:
                        sortedList = ApiKeysList.ToList();
                        break;
                }

                ApiKeysList.Clear();
                foreach (var item in sortedList)
                {
                    ApiKeysList.Add(item);
                }
            }
        }

        // Reset filters
        private async Task ResetFilters()
        {
            ApiKeysList.Clear();

            // Reset all filtering and sorting criteria
            FilterCriteria = string.Empty;
            SortCriteria = null; 
            SearchQuery = null; 

            // Refresh the list
            await GetAllApiKeys();
        }


    }
}
