using Avalonia.Controls;
using ApiVault.ViewModels;

namespace ApiVault.Views
{
    public partial class AppContentView : UserControl
    {
        public AppContentView()
        {
            InitializeComponent();
            DataContext = new AppContentViewModel();
        }   
    }
}
