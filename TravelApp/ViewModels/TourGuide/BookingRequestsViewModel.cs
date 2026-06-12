using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class BookingRequestsViewModel : ObservableObject
    {
        public event EventHandler BookingAccepted;

        private readonly NotificationManager _notificationManager;
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly IBookingService _bookingService;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _pendingBookings;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage;

        public BookingRequestsViewModel(
            NotificationManager notificationManager,
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            IBookingService bookingService)
        {
            _notificationManager = notificationManager;
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _bookingService = bookingService;
            PendingBookings = new ObservableCollection<BookingModel>();
            _ = LoadPendingBookingsAsync();
        }

        [RelayCommand]
        private async Task LoadPendingBookingsAsync()
        {
            ErrorMessage = string.Empty;
            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            IsBusy = true;
            try
            {
                PendingBookings = new ObservableCollection<BookingModel>(
                    await _contentRepository.GetPendingBookingsByGuideAsync(
                        guide.Id));
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load guide booking requests",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private Task AcceptBookingAsync(BookingModel booking)
        {
            return SetBookingStatusAsync(booking, BookingStatus.Accepted);
        }

        [RelayCommand]
        private Task RejectBookingAsync(BookingModel booking)
        {
            return SetBookingStatusAsync(booking, BookingStatus.Rejected);
        }

        private async Task SetBookingStatusAsync(
            BookingModel booking,
            BookingStatus status)
        {
            if (booking == null || IsBusy)
            {
                return;
            }

            ErrorMessage = string.Empty;
            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            IsBusy = true;
            try
            {
                var result = await _bookingService.UpdateByGuideAsync(
                        booking.Id,
                        guide.Id,
                        status);
                if (!result.Succeeded)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                PendingBookings.Remove(booking);
                var accepted = status == BookingStatus.Accepted;
                if (accepted)
                {
                    BookingAccepted?.Invoke(this, EventArgs.Empty);
                }

                _notificationManager.ShowNotification(
                    accepted ? "Thành công" : "Đã từ chối",
                    (accepted
                        ? "Đã chấp nhận booking "
                        : "Đã từ chối booking ") +
                    booking.BookingId + ".",
                    !accepted);
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    status == BookingStatus.Accepted
                        ? "Accept guide booking"
                        : "Reject guide booking",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
