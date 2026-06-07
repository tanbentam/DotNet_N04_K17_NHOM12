using System.Collections.Generic;

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

        public virtual ICollection<HotelModel> Hotels { get; set; }
        public virtual ICollection<BookingModel> Bookings { get; set; }
    }
}
