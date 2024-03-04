using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiVault.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        /* - - - - - - - - - - Routing - - - - - - - - - - */
        // Routing interface needed properties.
        // Sets the view identifier to "login"
        public IScreen? HostScreen { get; }
        public string? UrlPathSegment { get; } = "login";


        /* - - - - - - - - - - Binding View Variables - - - - - - - - - - */
        private string? username;
        public string? Username
        {
            get => username;
            set
            {
                this.RaiseAndSetIfChanged(ref username, value);
                UpdateCanSubmit();
            }
        }

        private string? password;
        public string? Password
        {
            get => password;
            set
            {
                this.RaiseAndSetIfChanged(ref password, value);
                UpdateCanSubmit();
            }
        }

        public bool CanSubmit { get; set; } = false;

        /* - - - - - - - - - - Constructors - - - - - - - - - - */
        public LoginViewModel(IScreen screen)
        {
            HostScreen = screen;
        }

        public LoginViewModel() { }

        /* - - - - - - - - - - Methods - - - - - - - - - - */
        
        // Function that checks credentials
        public void AttempLogin()
        {

        }

        // Enables user to submit once all form fields are filled
        private void UpdateCanSubmit()
        {
            CanSubmit = !string.IsNullOrWhiteSpace(Username)
                        && !string.IsNullOrWhiteSpace(Password);

            this.RaisePropertyChanged(nameof(CanSubmit));
        }
    }
}
