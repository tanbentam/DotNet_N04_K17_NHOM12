using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Booking;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.User
{
    public partial class TourBookingViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;
        private readonly IBookingService _bookingService;

        [ObservableProperty] private DestinationModel _selectedDestination;
        [ObservableProperty] private UserModel _selectedGuide;
        [ObservableProperty] private HotelModel _selectedHotel;
        [ObservableProperty] private int _tripDurationDays = 1;
        [ObservableProperty] private DateTime _startDate = DateTime.Today;
        [ObservableProperty] private ObservableCollection<BookingModel> _bookings;
        [ObservableProperty] private BookingModel _selectedBooking;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _bookingMessage;
        [ObservableProperty] private string _refundReason;
        [ObservableProperty] private decimal _estimatedPrice;
        [ObservableProperty] private string _pricingSummary;

        public TourBookingViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            NotificationManager notificationManager,
            IBookingService bookingService)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            _bookingService = bookingService;
            Bookings = new ObservableCollection<BookingModel>();
            UpdatePriceEstimate();
            _ = LoadBookingsAsync();
        }

        partial void OnSelectedHotelChanged(HotelModel value)
        {
            UpdatePriceEstimate();
        }

        partial void OnTripDurationDaysChanged(int value)
        {
            UpdatePriceEstimate();
        }

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            BookingMessage = string.Empty;
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                BookingMessage = "Phiên đăng nhập người dùng không hợp lệ.";
                return;
            }

            if (SelectedDestination == null || SelectedGuide == null)
            {
                BookingMessage = "Hãy chọn điểm đến và hướng dẫn viên.";
                return;
            }

            if (TripDurationDays <= 0 ||
                TripDurationDays > BookingService.MaximumTripDays ||
                StartDate.Date < DateTime.Today ||
                StartDate.Date > DateTime.Today.AddDays(
                    BookingService.MaximumAdvanceBookingDays))
            {
                BookingMessage =
                    "Ngày đặt phải trong 365 ngày tới và chuyến đi từ 1 đến 30 ngày.";
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
                Price = 0,
                Status = BookingStatus.Pending,
                BookingId = CreateBookingCode(),
                DestinationName = SelectedDestination.Name,
                UserName = user.FullName
            };

            IsBusy = true;
            try
            {
                var result = await _bookingService.CreateBookingAsync(booking);
                if (!result.Succeeded)
                {
                    LoggerService.LogBookingFailure(
                        user.Id.ToString(),
                        "Create booking was rejected. BookingCode=" +
                            booking.BookingId +
                            "; Reason=" + result.Message);
                    BookingMessage = result.Message;
                    return;
                }

                await LoadBookingsAsync();
                BookingMessage =
                    "Đã tạo đặt tour " + booking.BookingId + " thành công.";
                _notificationManager.ShowNotification(
                    "Thành công",
                    "Yêu cầu đặt tour đã được gửi tới hướng dẫn viên.",
                    false);
            }
            catch (Exception ex)
            {
                var errorId = LoggerService.LogException(
                    "Create user booking",
                    ex,
                    "UserId=" + user.Id +
                    "; BookingCode=" + booking.BookingId);
                BookingMessage = "Không thể tạo đặt tour: " +
                    ex.GetBaseException().Message +
                    " [" + errorId + "]";
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
                BookingMessage = "Hãy chọn đặt tour cần hủy.";
                return;
            }

            IsBusy = true;
            try
            {
                var result =
                    await _bookingService.CancelByUserAsync(
                        booking.Id,
                        user.Id,
                        RefundReason);
                if (!result.Succeeded)
                {
                    LoggerService.LogBookingFailure(
                        user.Id.ToString(),
                        "Cancel booking was rejected. BookingId=" +
                            booking.Id);
                    BookingMessage = result.Message;
                    return;
                }

                await LoadBookingsAsync();
                var requestedRefund = booking.Status == BookingStatus.Paid;
                BookingMessage = requestedRefund
                    ? "Đã gửi yêu cầu hoàn tiền cho đặt tour " +
                        booking.BookingId + "."
                    : "Đã hủy đặt tour " + booking.BookingId + ".";
                RefundReason = string.Empty;
                _notificationManager.ShowNotification(
                    requestedRefund ? "Đã gửi yêu cầu" : "Đã hủy",
                    requestedRefund
                        ? "Quản trị viên sẽ xem xét yêu cầu hoàn tiền của bạn."
                        : "Đã hủy tour " + booking.BookingId + ".",
                    !requestedRefund);
            }
            catch (Exception ex)
            {
                var errorId = LoggerService.LogException(
                    "Cancel user booking",
                    ex,
                    "UserId=" + user.Id +
                    "; BookingId=" + booking.Id);
                BookingMessage = "Không thể hủy đặt tour: " +
                    ex.GetBaseException().Message +
                    " [" + errorId + "]";
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
                var errorId = LoggerService.LogException(
                    "Load user bookings",
                    ex,
                    "UserId=" + user.Id);
                BookingMessage = "Không thể tải danh sách đặt tour: " +
                    ex.GetBaseException().Message +
                    " [" + errorId + "]";
            }
        }

        private static string CreateBookingCode()
        {
            return "BK" +
                DateTime.Now.ToString("yyyyMMddHHmmss") +
                Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
        }

        private void UpdatePriceEstimate()
        {
            if (TripDurationDays <= 0 ||
                TripDurationDays > BookingService.MaximumTripDays)
            {
                EstimatedPrice = 0;
                PricingSummary = "Số ngày phải từ 1 đến 30.";
                return;
            }

            var quote = _bookingService.CalculatePrice(
                SelectedHotel?.PricePerNight ?? 0,
                TripDurationDays);
            EstimatedPrice = quote.Total;
            PricingSummary = string.Format(
                "Hướng dẫn viên: {0:N0} | Phòng: {1:N0} | Giảm: {2:N0} | Phí dịch vụ: {3:N0}",
                quote.GuideFee,
                quote.HotelFee,
                quote.Discount,
                quote.ServiceFee);
        }
    }
}
