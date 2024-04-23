using ApiVault.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ApiVault.Views
{
    public partial class ApiKeyView : UserControl
    {
        public ApiKeyView()
        {
            InitializeComponent();
        }

        public void CopyClipboard(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ApiKeyViewModel;

            if (viewModel != null )
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

                if ( clipboard != null )
                {
                    clipboard.SetTextAsync(viewModel.ApiKey);
                }
            }
        }
    }
}
