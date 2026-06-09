using CommunityToolkit.Mvvm.ComponentModel;
using TravelApp.ViewModels.Shared;

namespace TravelApp.ViewModels.User
{
    public partial class UserDashboardViewModel : ObservableObject
    {
        public AdvancedSearchViewModel SearchVM { get; }
        public TourBookingViewModel BookingVM { get; }
        public PaymentSimulationViewModel PaymentVM { get; }

        public UserDashboardViewModel(
            AdvancedSearchViewModel searchVM,
            TourBookingViewModel bookingVM,
            PaymentSimulationViewModel paymentVM)
        {
            SearchVM = searchVM;
            BookingVM = bookingVM;
            PaymentVM = paymentVM;
        }
    }
}
