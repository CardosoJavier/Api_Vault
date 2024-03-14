using ApiVault.ViewModels;
using Avalonia.Controls;

namespace ApiVault.Views
{
    public partial class Add_KeyPageView : UserControl
    {
        public Add_KeyPageView()
        {
            InitializeComponent();
            DataContext = new Add_KeyPageViewModel();
        }
    }
}
