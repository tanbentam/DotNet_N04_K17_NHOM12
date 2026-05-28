using Microsoft.Extensions.DependencyInjection;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels;
using TravelApp.Frontend.ViewModels.User;

namespace TravelApp.Frontend.Services.AuthenticationStrategy
{
    public class UserAuthStrategy : IAuthStrategy
    {
        public void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser)
        {
            [cite_start]// Điều hướng đến User Dashboard [cite: 104-120]
            var userDashboardVM = App.Current.Services.GetService<UserDashboardViewModel>();
            mainViewModel.CurrentViewModel = userDashboardVM;
        }
    }
}