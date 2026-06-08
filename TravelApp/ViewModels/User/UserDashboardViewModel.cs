using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TravelApp.ViewModels.Shared;

namespace TravelApp.ViewModels.User
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        public AdvancedSearchViewModel AdvancedSearchVM { get; }
        public TourBookingViewModel TourBookingVM { get; }
        public PaymentSimulationViewModel PaymentSimulationVM { get; }

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public UserDashboardViewModel(
            AdvancedSearchViewModel advancedSearchVM,
            TourBookingViewModel tourBookingVM,
            PaymentSimulationViewModel paymentSimulationVM)
        {
            AdvancedSearchVM = advancedSearchVM;
            TourBookingVM = tourBookingVM;
            PaymentSimulationVM = paymentSimulationVM;

            CurrentViewModel = AdvancedSearchVM;
        }

        [RelayCommand]
        private void Navigate(string viewName)
        {
            switch (viewName)
            {
                case "Search": CurrentViewModel = AdvancedSearchVM; break;
                case "Booking": CurrentViewModel = TourBookingVM; break;
                case "Payment": CurrentViewModel = PaymentSimulationVM; break;
            }
        }
    }
}