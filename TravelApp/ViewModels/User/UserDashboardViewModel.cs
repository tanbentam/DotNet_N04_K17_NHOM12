using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.User
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        // Container cho các ViewModels con:
        [cite_start]// - AdvancedSearchViewModel 
        [cite_start]// - TourBookingViewModel [cite: 109-115]
        [cite_start]// - FavoriteListViewModel 
        [cite_start]// - UserProfileViewModel 

        public UserDashboardViewModel()
        {
            // Inject các ViewModels thông qua Dependency Injection (DI)
        }
    }
}