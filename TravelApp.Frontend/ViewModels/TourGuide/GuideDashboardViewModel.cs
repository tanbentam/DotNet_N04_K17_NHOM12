using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TravelApp.Frontend.Contracts;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public ScheduleManagementViewModel ScheduleManagementViewModel { get; }
        public BookingRequestsViewModel BookingRequestsViewModel { get; }

        public GuideDashboardViewModel(
            INavigationService navigationService,
            ScheduleManagementViewModel scheduleManagementViewModel,
            BookingRequestsViewModel bookingRequestsViewModel)
        {
            _navigationService = navigationService;
            ScheduleManagementViewModel = scheduleManagementViewModel;
            BookingRequestsViewModel = bookingRequestsViewModel;
        }

        [RelayCommand]
        private void Logout()
        {
            _navigationService.NavigateToLogin();
        }
    }
}
