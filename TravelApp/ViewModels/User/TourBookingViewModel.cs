using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.User
{
    public partial class TourBookingViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private DestinationModel _selectedDestination; // [cite: 111]
        [ObservableProperty] private UserModel _selectedGuide;              // [cite: 112]
        [ObservableProperty] private HotelModel _selectedHotel;             // [cite: 113]

        [ObservableProperty] private int _tripDurationDays = 1;

        public TourBookingViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        partial void OnTripDurationDaysChanged(int value)
        {
            [cite_start]// Business Logic Rule 
            [cite_start]// Chọn khách sạn nếu chuyến đi từ 2 ngày trở lên [cite: 113]
            if (value >= 2)
            {
                _notificationManager.ShowNotification("Gợi ý", "Chuyến đi từ 2 ngày trở lên, vui lòng chọn thêm Khách sạn.", false);
            }
        }

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            // [BACKEND DEVELOPER NOTE] 
            // Endpoint: Constants.User_BookTour_Endpoint
            // Payload phải bao gồm DestinationId, GuideId, HotelId (nếu có), PaymentStatus.
            [cite_start]// Nếu thời gian User chọn khớp với lịch Guide đăng ký, hệ thống phải trả về thông báo[cite: 96].

            await Task.Delay(500); // Bất đồng bộ [cite: 156-157]

            [cite_start]// Hiển thị Popup ở góc trên [cite: 118, 126-127]
            _notificationManager.ShowNotification("Thành công", "Yêu cầu đặt tour đã được gửi tới Guide.", false);
        }

        [RelayCommand]
        private async Task CancelBookingAsync(string bookingId)
        {
            [cite_start]// Logic Hủy tour đã đặt [cite: 119-120]
            await Task.Delay(500);
            _notificationManager.ShowNotification("Đã hủy", $"Đã hủy tour {bookingId}.", true);
        }
    }
}