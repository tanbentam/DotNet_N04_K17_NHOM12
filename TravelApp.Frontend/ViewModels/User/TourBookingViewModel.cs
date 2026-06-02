using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.User
{
    public partial class TourBookingViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private DestinationModel _selectedDestination; //
        [ObservableProperty] private UserModel _selectedGuide;              //
        [ObservableProperty] private HotelModel _selectedHotel;             //

        [ObservableProperty] private int _tripDurationDays = 1;

        public TourBookingViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            if (TripDurationDays >= 2 && SelectedHotel == null)
            {
                _notificationManager.ShowNotification("Hotel required", "Trips of 2 days or longer require a hotel selection.", true);
                return;
            }

            // [BACKEND DEVELOPER NOTE] 
            // Endpoint: Constants.User_BookTour_Endpoint
            // Payload phải bao gồm DestinationId, GuideId, HotelId (nếu có), PaymentStatus.
            // Nếu thời gian User chọn khớp với lịch Guide đăng ký, hệ thống phải trả về thông báo.

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
