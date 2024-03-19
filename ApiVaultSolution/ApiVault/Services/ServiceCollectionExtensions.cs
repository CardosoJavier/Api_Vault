using ApiVault.ViewModels;
using ApiVault.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ApiVault.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<IUserSessionService, UserSessionService>();
            collection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            collection.AddTransient<MainWindowViewModel> ();
            collection.AddTransient<LoginViewModel>();
            collection.AddTransient<AppContentViewModel>();
            collection.AddTransient<DashboardPageViewModel>();
            collection.AddTransient<GroupsPageViewModel>();
            collection.AddTransient<Add_KeyPageViewModel>();
            collection.AddTransient<AppContentViewModel>();
        }
    }
}
