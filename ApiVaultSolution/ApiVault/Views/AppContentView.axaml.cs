using Avalonia.Controls;
using ApiVault.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ApiVault.Views
{
    public partial class AppContentView : UserControl
    {
        public AppContentView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<AppContentViewModel>();
        }   
    }
}
