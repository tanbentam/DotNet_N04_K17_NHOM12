using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace TravelApp.Frontend.ViewModels.Shared
{
    public partial class PaymentSimulationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _paymentMethod;

        [ObservableProperty]
        private bool _isPaymentSuccess;

        [RelayCommand]
        private async Task ProcessFakeQRPaymentAsync()
        {
            PaymentMethod = "Fake QR";
            await SimulatePaymentProcessing();
        }

        [RelayCommand]
        private async Task ProcessBankTransferAsync()
        {
            PaymentMethod = "Simulated Bank Transfer";
            await SimulatePaymentProcessing();
        }

        private async Task SimulatePaymentProcessing()
        {
            // Giả lập xử lý thanh toán
            await Task.Delay(2000);

            // Random mô phỏng thành công hoặc thất bại 
            IsPaymentSuccess = new Random().Next(0, 2) == 1;

            if (IsPaymentSuccess)
            {
                // Gọi tới TourBookingViewModel để xác nhận hoàn tất thanh toán
            }
            else
            {
                // [BACKEND DEVELOPER NOTE] Ghi log lỗi Booking/Payment Failures
            }
        }
    }
}
