using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        public ScheduleManagementViewModel ScheduleVM { get; }
        public BookingRequestsViewModel BookingRequestsVM { get; }
        public WorkScheduleViewModel WorkScheduleVM { get; }
        public DestinationManagementViewModel DestinationManagementVM { get; }
        public HotelManagementViewModel HotelManagementVM { get; }

        public GuideDashboardViewModel(
            ScheduleManagementViewModel scheduleVM,
            BookingRequestsViewModel bookingRequestsVM,
            WorkScheduleViewModel workScheduleVM,
            DestinationManagementViewModel destinationManagementVM,
            HotelManagementViewModel hotelManagementVM)
        {
            ScheduleVM = scheduleVM;
            BookingRequestsVM = bookingRequestsVM;
            WorkScheduleVM = workScheduleVM;
            DestinationManagementVM = destinationManagementVM;
            HotelManagementVM = hotelManagementVM;

            BookingRequestsVM.BookingAccepted += async (sender, args) =>
                await WorkScheduleVM.LoadWorkScheduleAsync();
        }
    }
}
