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
            // API INTEGRATION POINT:
            // GET /api/guide/bookings?status=Pending.
            PendingBookings.Add(new BookingModel { BookingId = "BK001", DestinationName = "Da Nang Beach", UserName = "Nguyen Van A", HotelName = "Seaside Hotel" });
            PendingBookings.Add(new BookingModel { BookingId = "BK003", DestinationName = "Hoi An Lantern Walk", UserName = "Tran Thi B", HotelName = "Lantern Stay" });
        }

        [RelayCommand]
        private async Task AcceptBookingAsync(BookingModel booking)
        {
            if (booking == null)
            {
                return;
            }

            // API INTEGRATION POINT: PATCH /api/guide/bookings/{bookingId}/accept.
            await Task.Delay(500);

            PendingBookings.Remove(booking);
            _notificationManager.ShowNotification("Booking accepted", $"Accepted booking {booking.BookingId}.", false);
        }

        [RelayCommand]
        private async Task RejectBookingAsync(BookingModel booking)
        {
            if (booking == null)
            {
                return;
            }

            // API INTEGRATION POINT: PATCH /api/guide/bookings/{bookingId}/reject.
            await Task.Delay(500);

            PendingBookings.Remove(booking);
            _notificationManager.ShowNotification("Booking rejected", $"Rejected booking {booking.BookingId}.", true);
        }
    }
}
