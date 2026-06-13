using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class BookingModel
    {
        public BookingModel()
        {
            Payments = new HashSet<PaymentModel>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int GuideId { get; set; }
        public int? HotelId { get; set; }
        public int DestinationId { get; set; }
        public DateTime StartDate { get; set; }
        public int Nights { get; set; }
        public decimal Price { get; set; }
        public BookingStatus Status { get; set; }
        public string BookingId { get; set; }
        public string DestinationName { get; set; }
        public string UserName { get; set; }
        public DateTime? GuideCancellationRequestedAt { get; set; }
        public string GuideCancellationReason { get; set; }
        public DateTime? GuideCancellationResolvedAt { get; set; }
        public bool? GuideCancellationApproved { get; set; }

        [NotMapped]
        public DateTime CompletionDate =>
            StartDate.Date.AddDays(Math.Max(1, Nights) - 1);

        [NotMapped]
        public bool HasPendingGuideCancellation =>
            GuideCancellationRequestedAt.HasValue &&
            !GuideCancellationResolvedAt.HasValue;

        [NotMapped]
        public bool CanGuideRequestCancellation =>
            Status == BookingStatus.Accepted &&
            StartDate.Date > DateTime.Today &&
            !HasPendingGuideCancellation;

        [NotMapped]
        public string WorkScheduleStatus
        {
            get
            {
                if (HasPendingGuideCancellation)
                {
                    return "Đang chờ duyệt hủy";
                }

                if (DateTime.Today < StartDate.Date)
                {
                    return "Sắp diễn ra";
                }

                if (DateTime.Today <= CompletionDate)
                {
                    return "Đang diễn ra";
                }

                return "Đã kết thúc";
            }
        }

        public virtual UserModel User { get; set; }
        public virtual UserModel Guide { get; set; }
        public virtual HotelModel Hotel { get; set; }
        public virtual DestinationModel Destination { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
