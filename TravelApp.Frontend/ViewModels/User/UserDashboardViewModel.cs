using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.ViewModels.Shared;

namespace TravelApp.Frontend.ViewModels.User
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public AdvancedSearchViewModel AdvancedSearchViewModel { get; }
        public TourBookingViewModel TourBookingViewModel { get; }
        public PaymentSimulationViewModel PaymentSimulationViewModel { get; }

        public UserDashboardViewModel(
            INavigationService navigationService,
            AdvancedSearchViewModel advancedSearchViewModel,
            TourBookingViewModel tourBookingViewModel,
            PaymentSimulationViewModel paymentSimulationViewModel)
        {
            _navigationService = navigationService;
            AdvancedSearchViewModel = advancedSearchViewModel;
            TourBookingViewModel = tourBookingViewModel;
            PaymentSimulationViewModel = paymentSimulationViewModel;
        }

        [RelayCommand]
        private void Logout()
        {
            _navigationService.NavigateToLogin();
        }
    }
}
