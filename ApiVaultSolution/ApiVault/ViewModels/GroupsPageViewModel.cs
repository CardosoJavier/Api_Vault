using ApiVault.Services;
using ApiVault.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.ViewModels
{
    public class GroupsPageViewModel : ViewModelBase
    {
        public IUserSessionService _userSessionService;

        public ObservableCollection<ApiKeyGroupViewModel> GroupList => _userSessionService.GroupList;

        public GroupsPageViewModel(IUserSessionService userSession)
        {
            _userSessionService = userSession;

            // Trigger UI update when GroupList changes.
            this.WhenAnyValue(vm => vm._userSessionService.GroupList)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(GroupList)));
        }
    }
}
