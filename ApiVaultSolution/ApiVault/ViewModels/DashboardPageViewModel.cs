using ApiVault.Services;

namespace ApiVault.ViewModels
{
    internal class DashboardPageViewModel : ViewModelBase
    {
        // - - - - - - - - - - Class Varibles - - - - - - - -  - - 
        private readonly IUserSessionService _userSessionService;
        public string? UserName { get; set; } = string.Empty;
        

        // - - - - - - - - - - Constructors - - - - - - - -  - - 
        public DashboardPageViewModel(IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            UserName = _userSessionService.Username;
        }

        // - - - - - - - - - - Methods - - - - - - - -  - - 


    }
}
