using Microsoft.Extensions.DependencyInjection;
using TravelApp.Models;
using TravelApp.ViewModels;
using TravelApp.ViewModels.User;

namespace TravelApp.Services.AuthenticationStrategy
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