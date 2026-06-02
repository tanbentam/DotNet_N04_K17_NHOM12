using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public partial class AvailableDay : ObservableObject
    {
        public string DayOfWeek { get; set; }

        [ObservableProperty] private bool _isAvailable;
        [ObservableProperty] private string _timeSlot;
    }

    public partial class ScheduleManagementViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        [ObservableProperty]
        private ObservableCollection<AvailableDay> _weeklySchedule;

        public ScheduleManagementViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            WeeklySchedule = new ObservableCollection<AvailableDay>
            {
                new AvailableDay { DayOfWeek = "Monday", IsAvailable = false, TimeSlot = "08:00 - 12:00" },
                new AvailableDay { DayOfWeek = "Tuesday", IsAvailable = false, TimeSlot = "08:00 - 12:00" },
                new AvailableDay { DayOfWeek = "Wednesday", IsAvailable = false, TimeSlot = "13:00 - 17:00" },
                new AvailableDay { DayOfWeek = "Thursday", IsAvailable = false, TimeSlot = "13:00 - 17:00" },
                new AvailableDay { DayOfWeek = "Friday", IsAvailable = false, TimeSlot = "08:00 - 17:00" },
                new AvailableDay { DayOfWeek = "Saturday", IsAvailable = true, TimeSlot = "08:00 - 12:00" },
                new AvailableDay { DayOfWeek = "Sunday", IsAvailable = true, TimeSlot = "13:00 - 18:00" }
            };
        }

        [RelayCommand]
        private async Task SaveScheduleAsync()
        {
            // API INTEGRATION POINT:
            // PUT /api/guide/availability with weeklySchedule[] containing dayOfWeek, isAvailable, and timeSlot.
            // Backend must notify users when booking date/time matches guide availability.
            await Task.Delay(500);

            _notificationManager.ShowNotification("Schedule saved", "Weekly availability was updated.", false);
        }
    }
}
