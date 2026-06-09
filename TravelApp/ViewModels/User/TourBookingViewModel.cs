using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.User
{
    public partial class TourBookingViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private DestinationModel _selectedDestination;
        [ObservableProperty] private UserModel _selectedGuide;
        [ObservableProperty] private HotelModel _selectedHotel;
        [ObservableProperty] private int _tripDurationDays = 1;
        [ObservableProperty] private DateTime _startDate = DateTime.Today;
        [ObservableProperty] private ObservableCollection<BookingModel> _bookings;
        [ObservableProperty] private BookingModel _selectedBooking;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _bookingMessage;

        public TourBookingViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            Bookings = new ObservableCollection<BookingModel>();
            _ = LoadBookingsAsync();
        }

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            BookingMessage = string.Empty;
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                BookingMessage = "Phiên đăng nhập User không hợp lệ.";
                return;
            }

            if (SelectedDestination == null || SelectedGuide == null)
            {
                BookingMessage = "Hãy chọn điểm đến và Guide.";
                return;
            }

            if (TripDurationDays <= 0 || StartDate.Date < DateTime.Today)
            {
                BookingMessage =
                    "Ngày bắt đầu không được ở quá khứ và số ngày phải lớn hơn 0.";
                return;
            }

            if (SelectedHotel != null &&
                SelectedHotel.DestinationId != SelectedDestination.Id)
            {
                BookingMessage =
                    "Khách sạn đã chọn không thuộc điểm đến này.";
                return;
            }

            var booking = new BookingModel
            {
                UserId = user.Id,
                GuideId = SelectedGuide.Id,
                HotelId = SelectedHotel?.Id,
                DestinationId = SelectedDestination.Id,
                StartDate = StartDate.Date,
                Nights = TripDurationDays,
                Price = SelectedHotel == null
                    ? 0
                    : SelectedHotel.PricePerNight * TripDurationDays,
                Status = BookingStatus.Pending,
                BookingId = CreateBookingCode(),
                DestinationName = SelectedDestination.Name,
                UserName = user.FullName
            };

            IsBusy = true;
            try
            {
                if (!await _contentRepository.CreateBookingAsync(booking))
                {
                    BookingMessage =
                        "Không thể tạo booking. Hãy kiểm tra lại lựa chọn.";
                    return;
                }

                await LoadBookingsAsync();
                BookingMessage =
                    "Đã tạo booking " + booking.BookingId + " thành công.";
                _notificationManager.ShowNotification(
                    "Thành công",
                    "Yêu cầu đặt tour đã được gửi tới Guide.",
                    false);
            }
            catch (Exception ex)
            {
                BookingMessage = "Không thể tạo booking: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelBookingAsync(BookingModel booking)
        {
            BookingMessage = string.Empty;
            var user = _sessionService.CurrentUser;
            if (booking == null || user == null || user.Role != RoleType.User)
            {
                BookingMessage = "Hãy chọn booking cần hủy.";
                return;
            }

            IsBusy = true;
            try
            {
                var cancelled =
                    await _contentRepository.CancelBookingByUserAsync(
                        booking.Id,
                        user.Id);
                if (!cancelled)
                {
                    BookingMessage =
                        "Chỉ có thể hủy booking đang chờ hoặc đã được chấp nhận.";
                    return;
                }

                await LoadBookingsAsync();
                BookingMessage = "Đã hủy booking " + booking.BookingId + ".";
                _notificationManager.ShowNotification(
                    "Đã hủy",
                    "Đã hủy tour " + booking.BookingId + ".",
                    true);
            }
            catch (Exception ex)
            {
                BookingMessage = "Không thể hủy booking: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                Bookings.Clear();
                return;
            }

            try
            {
                var bookings =
                    await _contentRepository.GetBookingsByUserAsync(user.Id);
                Bookings = new ObservableCollection<BookingModel>(bookings);
            }
            catch (Exception ex)
            {
                BookingMessage = "Không thể tải booking: " +
                    ex.GetBaseException().Message;
            }
        }

        private static string CreateBookingCode()
        {
            return "BK" +
                DateTime.Now.ToString("yyyyMMddHHmmss") +
                Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
        }
    }
}
