using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.Services
{
    internal interface INavigationService
    {
        void NavigateTo(Type viewModelType);
    }
}
