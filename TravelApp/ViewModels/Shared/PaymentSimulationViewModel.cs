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

namespace TravelApp.ViewModels.Shared
{
    public partial class PaymentSimulationViewModel : ObservableObject
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private ObservableCollection<BookingModel> _payableBookings;
        [ObservableProperty] private ObservableCollection<PaymentModel> _paymentHistory;
        [ObservableProperty] private BookingModel _selectedBooking;
        [ObservableProperty] private PaymentMethod? _selectedPaymentMethod;
        [ObservableProperty] private string _paymentMethod;
        [ObservableProperty] private string _transactionCode;
        [ObservableProperty] private string _referenceCode;
        [ObservableProperty] private string _paymentInstructions;
        [ObservableProperty] private string _paymentMessage;
        [ObservableProperty] private bool _simulateSuccess = true;
        [ObservableProperty] private bool _isPaymentPrepared;
        [ObservableProperty] private bool _isProcessing;

        public event Action SuccessfulPaymentProcessed;

        public PaymentSimulationViewModel(
            IPaymentRepository paymentRepository,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _paymentRepository = paymentRepository;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            PayableBookings = new ObservableCollection<BookingModel>();
            PaymentHistory = new ObservableCollection<PaymentModel>();
            _ = LoadPaymentDataAsync();
        }

        partial void OnSelectedBookingChanged(BookingModel value)
        {
            ClearPreparedPayment();
        }

        [RelayCommand]
        private async Task LoadPaymentDataAsync()
        {
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                PayableBookings.Clear();
                PaymentHistory.Clear();
                PaymentMessage = "Phiên đăng nhập User không hợp lệ.";
                return;
            }

            IsProcessing = true;
            try
            {
                var bookingsTask =
                    _paymentRepository.GetPayableBookingsAsync(user.Id);
                var historyTask =
                    _paymentRepository.GetPaymentHistoryAsync(user.Id);
                await Task.WhenAll(bookingsTask, historyTask);

                PayableBookings = new ObservableCollection<BookingModel>(
                    await bookingsTask);
                PaymentHistory = new ObservableCollection<PaymentModel>(
                    await historyTask);

                if (SelectedBooking != null)
                {
                    SelectedBooking = FindBooking(SelectedBooking.Id);
                }
            }
            catch (Exception ex)
            {
                PaymentMessage = "Không thể tải dữ liệu thanh toán: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void PrepareQrPayment()
        {
            if (!ValidateSelectedBooking())
            {
                return;
            }

            SelectedPaymentMethod = Models.Enums.PaymentMethod.QrCode;
            PaymentMethod = "QR mô phỏng";
            TransactionCode = CreateTransactionCode("QR");
            ReferenceCode = SelectedBooking.BookingId;
            PaymentInstructions = string.Format(
                "Quét QR mô phỏng | Booking: {0} | Số tiền: {1:N0} | Nội dung: {2}",
                SelectedBooking.BookingId,
                SelectedBooking.Price,
                TransactionCode);
            IsPaymentPrepared = true;
            PaymentMessage =
                "QR đã sẵn sàng. Chọn kết quả mô phỏng rồi xác nhận.";
        }

        [RelayCommand]
        private void PrepareBankTransfer()
        {
            if (!ValidateSelectedBooking())
            {
                return;
            }

            SelectedPaymentMethod =
                Models.Enums.PaymentMethod.BankTransfer;
            PaymentMethod = "Chuyển khoản mô phỏng";
            TransactionCode = CreateTransactionCode("BANK");
            ReferenceCode = string.Empty;
            PaymentInstructions = string.Format(
                "Ngân hàng: Travel Bank | STK: 000012345678 | Số tiền: {0:N0} | Nội dung: {1}",
                SelectedBooking.Price,
                SelectedBooking.BookingId);
            IsPaymentPrepared = true;
            PaymentMessage =
                "Nhập mã tham chiếu giả lập rồi xác nhận chuyển khoản.";
        }

        [RelayCommand]
        private async Task ConfirmPaymentAsync()
        {
            var user = _sessionService.CurrentUser;
            if (user == null ||
                user.Role != RoleType.User ||
                SelectedBooking == null ||
                !SelectedPaymentMethod.HasValue ||
                !IsPaymentPrepared)
            {
                PaymentMessage =
                    "Hãy chọn booking và chuẩn bị phương thức thanh toán.";
                return;
            }

            if (SelectedPaymentMethod.Value ==
                    Models.Enums.PaymentMethod.BankTransfer &&
                string.IsNullOrWhiteSpace(ReferenceCode))
            {
                PaymentMessage =
                    "Vui lòng nhập mã tham chiếu chuyển khoản giả lập.";
                return;
            }

            IsProcessing = true;
            PaymentMessage = "Đang xử lý giao dịch mô phỏng...";
            try
            {
                await Task.Delay(1000);
                var result = await _paymentRepository.ProcessPaymentAsync(
                    user.Id,
                    SelectedBooking.Id,
                    SelectedPaymentMethod.Value,
                    TransactionCode,
                    ReferenceCode,
                    SimulateSuccess);

                PaymentMessage = result.Message;
                if (!result.WasSaved)
                {
                    return;
                }

                if (result.WasSuccessful)
                {
                    _notificationManager.ShowNotification(
                        "Thanh toán thành công",
                        "Booking " + SelectedBooking.BookingId +
                            " đã chuyển sang Paid.",
                        false);
                    SuccessfulPaymentProcessed?.Invoke();
                }
                else
                {
                    _notificationManager.ShowNotification(
                        "Thanh toán thất bại",
                        "Giao dịch mô phỏng đã được ghi nhận.",
                        true);
                }

                ClearPreparedPayment();
                await LoadPaymentDataAsync();
            }
            catch (Exception ex)
            {
                PaymentMessage = "Không thể xử lý thanh toán: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool ValidateSelectedBooking()
        {
            PaymentMessage = string.Empty;
            if (SelectedBooking == null)
            {
                PaymentMessage =
                    "Hãy chọn một booking đã được Guide chấp nhận.";
                return false;
            }

            if (SelectedBooking.Status != BookingStatus.Accepted)
            {
                PaymentMessage =
                    "Chỉ booking Accepted mới có thể thanh toán.";
                return false;
            }

            return true;
        }

        private BookingModel FindBooking(int bookingId)
        {
            foreach (var booking in PayableBookings)
            {
                if (booking.Id == bookingId)
                {
                    return booking;
                }
            }

            return null;
        }

        private void ClearPreparedPayment()
        {
            SelectedPaymentMethod = null;
            PaymentMethod = string.Empty;
            TransactionCode = string.Empty;
            ReferenceCode = string.Empty;
            PaymentInstructions = string.Empty;
            IsPaymentPrepared = false;
        }

        private static string CreateTransactionCode(string prefix)
        {
            return prefix + "-" +
                DateTime.Now.ToString("yyyyMMddHHmmss") + "-" +
                Guid.NewGuid().ToString("N")
                    .Substring(0, 6)
                    .ToUpperInvariant();
        }
    }
}
