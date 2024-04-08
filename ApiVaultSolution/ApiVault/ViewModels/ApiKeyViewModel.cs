using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class ApiKeyViewModel: ReactiveObject
    {
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

        public ApiKeyViewModel(string name, string key, string apiGroup)
        {
            ApiKeyName = name;
            ApiKey = key;
            Group = apiGroup;
        }

    }
}
