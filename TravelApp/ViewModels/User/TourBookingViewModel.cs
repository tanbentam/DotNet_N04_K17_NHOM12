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

        [ObservableProperty] private DestinationModel _selectedDestination;
        [ObservableProperty] private UserModel _selectedGuide;
        [ObservableProperty] private HotelModel _selectedHotel;

        [ObservableProperty] private int _tripDurationDays = 1;

        public TourBookingViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

       /* partial void OnTripDurationDaysChanged(int value)
        {
            if (value >= 2)
            {
                _notificationManager.ShowNotification("Gợi ý", "Chuyến đi từ 2 ngày trở lên, vui lòng chọn thêm Khách sạn.", false);
            }
        }*/

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            // [BACKEND DEVELOPER NOTE] 
            // Endpoint: Constants.User_BookTour_Endpoint
            // Payload phải bao gồm DestinationId, GuideId, HotelId (nếu có), PaymentStatus.

            await Task.Delay(500); // Bất đồng bộ

            // Hiển thị Popup ở góc trên
            _notificationManager.ShowNotification("Thành công", "Yêu cầu đặt tour đã được gửi tới Guide.", false);
        }

        [RelayCommand]
        private async Task CancelBookingAsync(string bookingId)
        {
            // Logic Hủy tour đã đặt
            await Task.Delay(500);
            _notificationManager.ShowNotification("Đã hủy", $"Đã hủy tour {bookingId}.", true);
        }
    }
}