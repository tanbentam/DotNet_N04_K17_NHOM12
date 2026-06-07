using System.Collections.Generic;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class DestinationModel
    {
        public DestinationModel()
        {
            Hotels = new HashSet<HotelModel>();
            Bookings = new HashSet<BookingModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int? CreatedByGuideId { get; set; }
        public ContentApprovalStatus ApprovalStatus { get; set; }

        public virtual UserModel CreatedByGuide { get; set; }
        public virtual ICollection<HotelModel> Hotels { get; set; }
        public virtual ICollection<BookingModel> Bookings { get; set; }
    }
}
