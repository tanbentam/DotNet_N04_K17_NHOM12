using Microsoft.Extensions.DependencyInjection;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels;
using TravelApp.Frontend.ViewModels.TourGuide;

namespace TravelApp.Frontend.Services.AuthenticationStrategy
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