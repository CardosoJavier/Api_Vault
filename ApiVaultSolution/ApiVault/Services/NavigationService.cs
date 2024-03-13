using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Window mainWindow;

        public NavigationService(Window mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void NavigateTo(Type viewModelType)
        {
            // Create an instance of the ViewModel
            var viewModel = Activator.CreateInstance(viewModelType);

            // Assume each ViewModel has a corresponding View named {ViewModelName}View
            var viewType = Type.GetType(viewModelType.FullName.Replace("ViewModel", "View"));
            var view = (Control)Activator.CreateInstance(viewType);

            view.DataContext = viewModel;
            mainWindow.Content = view; // Assumes mainWindow is your primary Window
        }
    }

}
