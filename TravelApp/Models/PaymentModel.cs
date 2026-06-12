using System;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionCode { get; set; }
        public string ReferenceCode { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual BookingModel Booking { get; set; }
        public virtual UserModel User { get; set; }
    }
}
