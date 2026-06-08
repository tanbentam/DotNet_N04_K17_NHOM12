using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        public ScheduleManagementViewModel ScheduleVM { get; }
        public BookingRequestsViewModel BookingRequestsVM { get; }

        public GuideDashboardViewModel(
            ScheduleManagementViewModel scheduleVM,
            BookingRequestsViewModel bookingRequestsVM)
        {
            ScheduleVM = scheduleVM;
            BookingRequestsVM = bookingRequestsVM;
        }
    }
}
