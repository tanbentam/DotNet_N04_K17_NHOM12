using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.TourGuide
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
            PendingBookings.Add(new BookingModel
            {
                BookingId = "BK001",
                DestinationName = "Đà Nẵng",
                UserName = "Nguyễn Văn A",
                StartDate = DateTime.Today.AddDays(7),
                Nights = 3,
                Price = 4500000,
                Status = BookingStatus.Pending
            });
        }

        [RelayCommand]
        private async Task AcceptBookingAsync(BookingModel booking)
        {
            if (booking == null)
                return;

            await Task.Delay(500);

            booking.Status = BookingStatus.Accepted;
            PendingBookings.Remove(booking);

            _notificationManager.ShowNotification("Thành công", $"Đã chấp nhận đơn đặt tour {booking.BookingId}.", false);
        }

        [RelayCommand]
        private async Task RejectBookingAsync(BookingModel booking)
        {
            if (booking == null)
                return;

            await Task.Delay(500);

            booking.Status = BookingStatus.Rejected;
            PendingBookings.Remove(booking);

            _notificationManager.ShowNotification("Đã hủy", $"Đã từ chối đơn đặt tour {booking.BookingId}.", true);
        }
    }
}
