using ApiVault.Services;
using ApiVault.Views;
using Avalonia.Controls;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;

namespace ApiVault.ViewModels
{
    public class ApiKeyGroupViewModel : ViewModelBase
    {
        private string? _group;
        public string? Group
        {
            set => this.RaiseAndSetIfChanged(ref _group, value);
            get => _group;
        }

        public ObservableCollection<ApiKeyViewModel> GroupApiKeys;

        public ReactiveCommand<Unit, Unit> ReviewGroupCommand { get; }

        public ApiKeyGroupViewModel(string group, ObservableCollection<ApiKeyViewModel> apiKeys) 
        {
            Group = group;
            GroupApiKeys = apiKeys;

            ReviewGroupCommand = ReactiveCommand.Create(() => ReviewGroup(apiKeys));

        }

        private void ReviewGroup(ObservableCollection<ApiKeyViewModel> apiKeys)
        {
            var groupDetailsWindow = new GroupDetailsView
            {
                DataContext = new GroupDetailsViewModel(apiKeys)
            };

            // Set the size of the window
            groupDetailsWindow.Width = 800; // Set your desired width
            groupDetailsWindow.Height = 600; // Set your desired height

            // Center the window on the screen
            groupDetailsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            groupDetailsWindow.Show();
        }
    }
}
