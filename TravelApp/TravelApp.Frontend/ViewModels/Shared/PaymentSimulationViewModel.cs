using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace TravelApp.Frontend.ViewModels.Shared
{
    public partial class PaymentSimulationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _paymentMethod; // "Fake QR" hoặc "Bank Transfer"

        [ObservableProperty]
        private bool _isPaymentSuccess;

        [RelayCommand]
        private async Task ProcessFakeQRPaymentAsync()
        {
            PaymentMethod = "Fake QR"; // [cite: 160]
            await SimulatePaymentProcessing();
        }

        [RelayCommand]
        private async Task ProcessBankTransferAsync()
        {
            PaymentMethod = "Simulated Bank Transfer"; // [cite: 161]
            await SimulatePaymentProcessing();
        }

        private async Task SimulatePaymentProcessing()
        {
            // Giả lập xử lý thanh toán
            await Task.Delay(2000); // Async Processing [cite: 156-157]

            [cite_start]// Random mô phỏng thành công hoặc thất bại 
            IsPaymentSuccess = new System.Random().Next(0, 2) == 1;

            if (IsPaymentSuccess)
            {
                [cite_start]// Gọi tới TourBookingViewModel để xác nhận hoàn tất thanh toán [cite: 114]
            }
            else
            {
                [cite_start]// [BACKEND DEVELOPER NOTE] Ghi log lỗi Booking/Payment Failures [cite: 168-171]
            }
        }
    }
}