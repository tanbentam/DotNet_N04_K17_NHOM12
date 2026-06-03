using Microsoft.Extensions.DependencyInjection;
using TravelApp.Common.Models;
using TravelApp.ViewModels;
using TravelApp.ViewModels.TourGuide;

namespace TravelApp.Services.AuthenticationStrategy
{
    public class GuideAuthStrategy : IAuthStrategy
    {
        public void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser)
        {
            [cite_start]// Điều hướng đến Guide Dashboard [cite: 83-103]
            var guideDashboardVM = App.Current.Services.GetService<GuideDashboardViewModel>();
            mainViewModel.CurrentViewModel = guideDashboardVM;
        }
    }
}