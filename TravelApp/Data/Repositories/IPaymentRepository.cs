using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;

namespace TravelApp.Data.Repositories
{
    public sealed class PaymentProcessResult
    {
        private PaymentProcessResult(
            bool wasSaved,
            bool wasSuccessful,
            string message)
        {
            WasSaved = wasSaved;
            WasSuccessful = wasSuccessful;
            Message = message;
        }

        public bool WasSaved { get; }
        public bool WasSuccessful { get; }
        public string Message { get; }

        public static PaymentProcessResult Success()
        {
            return new PaymentProcessResult(
                true,
                true,
                "Thanh toán thành công.");
        }

        public static PaymentProcessResult FailedSimulation()
        {
            return new PaymentProcessResult(
                true,
                false,
                "Giao dịch mô phỏng thất bại.");
        }

        public static PaymentProcessResult Rejected(string message)
        {
            return new PaymentProcessResult(false, false, message);
        }
    }

    public interface IPaymentRepository
    {
        Task<IReadOnlyList<BookingModel>> GetPayableBookingsAsync(int userId);
        Task<IReadOnlyList<PaymentModel>> GetPaymentHistoryAsync(int userId);
        Task<PaymentProcessResult> ProcessPaymentAsync(
            int userId,
            int bookingId,
            PaymentMethod method,
            string transactionCode,
            string referenceCode,
            bool simulateSuccess);
    }
}
