using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        public ScheduleManagementViewModel ScheduleVM { get; }
        public BookingRequestsViewModel BookingRequestsVM { get; }
        public DestinationManagementViewModel DestinationManagementVM { get; }

        public GuideDashboardViewModel(
            ScheduleManagementViewModel scheduleVM,
            BookingRequestsViewModel bookingRequestsVM,
            DestinationManagementViewModel destinationManagementVM)
        {
            ScheduleVM = scheduleVM;
            BookingRequestsVM = bookingRequestsVM;
            DestinationManagementVM = destinationManagementVM;
        }
    }
}
