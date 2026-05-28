using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public class AvailableDay : ObservableObject
    {
        public string DayOfWeek { get; set; } // Thứ 2 đến Chủ Nhật [cite: 94-95]

        [ObservableProperty]
        private bool _isAvailable;

        [ObservableProperty]
        private string _timeSlot; // Ví dụ: "08:00 - 17:00"
    }

    public partial class ScheduleManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<AvailableDay> _weeklySchedule;

        public ScheduleManagementViewModel()
        {
            WeeklySchedule = new ObservableCollection<AvailableDay>
            {
                new AvailableDay { DayOfWeek = "Thứ 2", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Thứ 3", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Thứ 4", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Thứ 5", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Thứ 6", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Thứ 7", IsAvailable = false },
                new AvailableDay { DayOfWeek = "Chủ Nhật", IsAvailable = false }
            };
        }

        [RelayCommand]
        private async Task SaveScheduleAsync()
        {
            // [BACKEND DEVELOPER NOTE]
            // Payload sẽ gửi mảng WeeklySchedule chứa các ngày IsAvailable = true.
            // Endpoint gợi ý: Constants.Guide_ManageSchedule_Endpoint
            [cite_start]// Logic Backend: Khi User chọn thời gian khớp với lịch rảnh này, hệ thống phải thông báo cho User[cite: 96].

            await Task.Delay(500); // Giả lập API call

            // Có thể gọi NotificationManager ở đây để báo lưu thành công
        }
    }
}