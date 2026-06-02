using Microsoft.Extensions.DependencyInjection;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels;
using TravelApp.Frontend.ViewModels.Admin;

namespace TravelApp.Frontend.Services.AuthenticationStrategy
{
    public class AdminAuthStrategy : IAuthStrategy
    {
        public void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser)
        {
            // Điều hướng đến Admin Dashboard (Có toàn quyền)
            var adminDashboardVM = App.Current.Services.GetService<AdminDashboardViewModel>();
            mainViewModel.CurrentViewModel = adminDashboardVM;
        }
    }
}
