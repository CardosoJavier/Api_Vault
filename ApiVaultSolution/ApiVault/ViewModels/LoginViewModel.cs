using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        // Routing interface needed properties.
        // Sets the view identifier to "login"
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "login";

        // Database connection

        // Constructor
        public LoginViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
