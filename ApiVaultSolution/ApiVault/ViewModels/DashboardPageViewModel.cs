using ApiVault.DataModels;
using ApiVault.Services;
using Avalonia.Controls;
using Avalonia.Threading;
using Cassandra;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
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

        // Display keys
        public ObservableCollection<ApiKeyViewModel> ApiKeysList { get; }
        public List<string> Groups { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        // Search, filter, and sort
        private ComboBoxItem _filterCriteria;
        public ComboBoxItem FilterCriteria
        {
            get => _filterCriteria;
            set
            {
                
                this.RaiseAndSetIfChanged(ref _filterCriteria, value);
                Debug.WriteLine($"filter: {FilterCriteria.Content.ToString()}");
                _ = ApplyFilterAsync();
            }
        }

        private ComboBoxItem _sortCriteria;
        public ComboBoxItem SortCriteria
        {
            get => _sortCriteria;
            set
            {
                // The value was changed, now do something as a result
                Debug.WriteLine($"sort: {value.Content.ToString()}");
                this.RaiseAndSetIfChanged(ref _sortCriteria, value);
                _ = ApplySortAsync();
            }
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                Debug.WriteLine($"search: {value}");
                this.RaiseAndSetIfChanged(ref _searchQuery, value);
                _ = ApplySearch();
            }
        }

        // - - - - - - - - - - Constructors - - - - - - - -  - - 
        public DashboardPageViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            UserName = _userSessionService.Username;

            dbConnection = new AstraDbConnection();

            InitializeAsync();

            ApiKeysList = new ObservableCollection<ApiKeyViewModel>();
            Groups = new List<string>();

        }

        // - - - - - - - - - - Methods - - - - - - - -  - - 
        // Connect to database
        public async Task InitializeAsync()
        {
            await dbConnection.InitializeConnection();
            session = await dbConnection.GetSession();
            await GetAllApiKeys();
            IsLoading = false;
        }

        
        private async Task GetAllApiKeys()
        {
            var query = session.Prepare("SELECT apiname, apikey, apigroup, replacedate FROM apikeys WHERE username = ? ALLOW FILTERING");
            var apiKeys = session.Execute(query.Bind(_userSessionService.Username));

            // TODO: Print table
            // Print header
            // Debug.WriteLine($"{"Api Name",-30} {"Api Key",-36} {"Api Group",-20} {"Replace Date",-30}");

            // Iterate through the RowSet and print each row
            foreach (var row in apiKeys)
            {
                var apiName = row.GetValue<string>("apiname");
                var apiKey = row.GetValue<string>("apikey");
                var apiGroup = row.GetValue<string>("apigroup");
                var replaceDate = row.GetValue<DateTimeOffset>("replacedate").ToString("yyyy-MM-dd HH:mm:ss");

                ApiKeysList.Add(new ApiKeyViewModel(apiName, apiKey, apiGroup, replaceDate));
                if (!Groups.Contains(apiGroup))
                {
                    Groups.Add(apiGroup);
                }

                // Print table row
                // Debug.WriteLine($"{apiName,-30} {apiKey,-36} {apiGroup,-20} {replaceDate,-30}");
            }

            Groups.Sort();
        }

        // Search method
        private async Task ApplySearch()
        {

            // Example filter application (implement based on actual properties and needs)
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
            var filterValue = ApiKeysList
                .Where(apiKey => string.IsNullOrEmpty(FilterCriteria.Content.ToString()) || apiKey.Group.Contains(FilterCriteria.Content.ToString(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Now, update ApiKeysList based on filteredAndSorted data
            // Make sure to clear and add items back to notify UI for changes.
            ApiKeysList.Clear();
            foreach (var item in filterValue)
            {
                ApiKeysList.Add(item);
            }

            
        }
        
        // Sort method
        private async Task ApplySortAsync()
        {
            List<ApiKeyViewModel> sortedList = new List<ApiKeyViewModel>();
            Debug.WriteLine(_sortCriteria);
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
}
