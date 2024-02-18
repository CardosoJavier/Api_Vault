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
        // Routing interface needed properties
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = "login";

        // Constructor
        public LoginViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
