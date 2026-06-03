using Microsoft.Extensions.DependencyInjection;
using TravelApp.Models;
using TravelApp.ViewModels;
using TravelApp.ViewModels.Admin;

namespace TravelApp.Services.AuthenticationStrategy
{
    public class AdminAuthStrategy : IAuthStrategy
    {
        public void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser)
        {
            [cite_start]// Điều hướng đến Admin Dashboard (Có toàn quyền) [cite: 24-30]
            var adminDashboardVM = App.Current.Services.GetService<AdminDashboardViewModel>();
            mainViewModel.CurrentViewModel = adminDashboardVM;
        }
    }
}