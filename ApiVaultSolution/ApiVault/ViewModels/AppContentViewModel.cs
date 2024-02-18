using ApiVault.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace ApiVault.ViewModels
{
    public partial class AppContentViewModel : ObservableObject
    {
        // Expand and hide left size navigation bar
        [ObservableProperty]
        private bool _isPaneOpen;

        [RelayCommand]
        private void ToggleMenu()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        // Current display page
        [ObservableProperty]
        private ViewModelBase _currentPage = new DashboardPageViewModel();

        // Define collection of navbar items
        public ObservableCollection<ListItemTemplate> NavBarBtns { get; } = new()
        {
            new ListItemTemplate(typeof(DashboardPageViewModel), "home_regular"),
            new ListItemTemplate(typeof(GroupsPageViewModel), "group_regular")
        };

        // Keeps track of selected navbar btn
        [ObservableProperty]
        private ListItemTemplate? _selectedNavOption;

        partial void OnSelectedNavOptionChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if (instance is null) return;
            CurrentPage = (ViewModelBase)instance;
        }
    }

    // Class to add navigation among navbar items
    public class ListItemTemplate
    {
        public string Label { get; }
        public Type ModelType { get; set; }
        public StreamGeometry Icon { get; }

        public ListItemTemplate(Type type, string iconKey)
        {
            ModelType = type;
            Label = type.Name.Replace("PageViewModel", "");
            Application.Current.TryFindResource(iconKey, out var navIcon);
            Icon = (StreamGeometry)navIcon!;
        }
    }
}
