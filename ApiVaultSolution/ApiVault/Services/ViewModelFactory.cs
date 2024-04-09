using ApiVault.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiVault.Services
{
    internal class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ViewModelBase CreateViewModel(Type viewModelType)
        {
            return (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
        }
        
    }
}
