using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.User
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        // Container cho các ViewModels con:
        // - AdvancedSearchViewModel
        // - TourBookingViewModel
        // - FavoriteListViewModel
        // - UserProfileViewModel

        public UserDashboardViewModel()
        {
            // Inject các ViewModels thông qua Dependency Injection (DI)
        }
    }
}