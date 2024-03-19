using ApiVault.ViewModels;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ApiVault.Views
{
    public partial class Add_KeyPageView : UserControl
    {
        public Add_KeyPageView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<Add_KeyPageViewModel>();
        }

        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}
