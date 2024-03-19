using ApiVault.Services;
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
        // - - - - - - - Class Variables - - - - - - - 
        private readonly IViewModelFactory _viewModelFactory;

        [ObservableProperty]
        private bool _isPaneOpen;

        [RelayCommand]
        private void ToggleMenu() => IsPaneOpen = !IsPaneOpen;

        [ObservableProperty]
        private ViewModelBase _currentPage;

        public ObservableCollection<ListItemTemplate> NavBarBtns { get; }

        [ObservableProperty]
        private ListItemTemplate? _selectedNavOption;


        // - - - - - - - Constructor - - - - - - - 
        public AppContentViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            // Initial currentPage setup through factory
            _currentPage = _viewModelFactory.CreateViewModel(typeof(DashboardPageViewModel));

            NavBarBtns = new ObservableCollection<ListItemTemplate>
            {
            new ListItemTemplate(typeof(DashboardPageViewModel), "home_regular"),
            new ListItemTemplate(typeof(GroupsPageViewModel), "group_regular"),
            new ListItemTemplate(typeof(Add_KeyPageViewModel), "add_square_regular"),
            };
        }

        // - - - - - - - Methods- - - - - - - 
        partial void OnSelectedNavOptionChanged(ListItemTemplate? value)
        {
            if (value == null) return;
            CurrentPage = _viewModelFactory.CreateViewModel(value.ViewModelType);
        }
    }


    // - - - - - - - Helper classes - - - - - - - 
    // Class to add navigation among navbar items
    // It takes the ViewModel file name, removes "ViewModel" and uses the
    // remaining as label
    public class ListItemTemplate
    {
        public string Label { get; }
        public Type ViewModelType { get; }
        public StreamGeometry Icon { get; }

        public ListItemTemplate(Type viewModelType, string iconKey)
        {
            ViewModelType = viewModelType;
            Label = viewModelType.Name.Replace("PageViewModel", "").Replace("_", " ");

            if (Application.Current.TryFindResource(iconKey, out var navIcon))
            {
                Icon = (StreamGeometry)navIcon!;
            }
            else
            {
                throw new InvalidOperationException($"Icon key '{iconKey}' not found.");
            }
        }
    }
}
