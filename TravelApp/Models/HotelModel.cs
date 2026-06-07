using System.Collections.Generic;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class HotelModel
    {
        public HotelModel()
        {
            Bookings = new HashSet<BookingModel>();
        }

        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int Rating { get; set; }
        public string ImageUrl { get; set; }
        public int? CreatedByGuideId { get; set; }
        public ContentApprovalStatus ApprovalStatus { get; set; }

        public virtual DestinationModel Destination { get; set; }
        public virtual UserModel CreatedByGuide { get; set; }
        public virtual ICollection<BookingModel> Bookings { get; set; }
    }
}
