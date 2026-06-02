using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public partial class BookingRequestsViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _pendingBookings;

        public BookingRequestsViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            PendingBookings = new ObservableCollection<BookingModel>();

            LoadPendingBookings();
        }

        private void LoadPendingBookings()
        {
            // [BACKEND DEVELOPER NOTE] Gọi API để lấy danh sách các đơn đặt tour đang chờ xác nhận
            // Giả lập dữ liệu
            PendingBookings.Add(new BookingModel { BookingId = "BK001", DestinationName = "Đà Nẵng", UserName = "Nguyễn Văn A" });
        }

        [RelayCommand]
        private async Task AcceptBookingAsync(BookingModel booking)
        {
            // [BACKEND DEVELOPER NOTE] Gọi API xác nhận đơn
            await Task.Delay(500); // Giả lập API

            PendingBookings.Remove(booking);

            // Hiển thị Popup Notification góc trên 
            _notificationManager.ShowNotification("Thành công", $"Đã CHẤP NHẬN đơn đặt tour {booking.BookingId}.", false);
        }

        [RelayCommand]
        private async Task RejectBookingAsync(BookingModel booking)
        {
            // [BACKEND DEVELOPER NOTE] Gọi API từ chối đơn
            await Task.Delay(500); // Giả lập API

            PendingBookings.Remove(booking);

            // Hiển thị Popup Notification góc trên (màu đỏ báo lỗi/từ chối) 
            _notificationManager.ShowNotification("Đã hủy", $"Đã TỪ CHỐI đơn đặt tour {booking.BookingId}.", true);
        }
    }
}
