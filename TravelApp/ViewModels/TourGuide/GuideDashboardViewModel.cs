using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        public ScheduleManagementViewModel ScheduleVM { get; }
        public BookingRequestsViewModel BookingRequestsVM { get; }
        public DestinationManagementViewModel DestinationManagementVM { get; }
        public HotelManagementViewModel HotelManagementVM { get; }

        public GuideDashboardViewModel(
            ScheduleManagementViewModel scheduleVM,
            BookingRequestsViewModel bookingRequestsVM,
            DestinationManagementViewModel destinationManagementVM,
            HotelManagementViewModel hotelManagementVM)
        {
            ScheduleVM = scheduleVM;
            BookingRequestsVM = bookingRequestsVM;
            DestinationManagementVM = destinationManagementVM;
            HotelManagementVM = hotelManagementVM;
        }
    }
}
