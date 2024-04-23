using ApiVault.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.Services
{
    // Dependency injection interface for user session
    public interface IUserSessionService
    {
        string Username { get; set; }
        string Phone { get; set; }
        public ObservableCollection<ApiKeyViewModel> ApiKeysList { get; set; }
        public ObservableCollection<ApiKeyGroupViewModel> GroupList { get; set; }

    }

    // User session implementation
    public class UserSessionService : IUserSessionService
    {
        // Interface variables
        public required string Username {  set; get; }

        public required string Phone { set; get; }

        public required ObservableCollection<ApiKeyViewModel> ApiKeysList { get; set; }

        public required ObservableCollection<ApiKeyGroupViewModel> GroupList { get; set; }


        public void Logout()
        {
            Username = string.Empty;
        }
    }
}
