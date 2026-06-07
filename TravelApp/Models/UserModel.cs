using System.Collections.Generic;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Bookings = new HashSet<BookingModel>();
            GuidedBookings = new HashSet<BookingModel>();
            CreatedDestinations = new HashSet<DestinationModel>();
            CreatedHotels = new HashSet<HotelModel>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public RoleType Role { get; set; }

        public virtual ICollection<BookingModel> Bookings { get; set; }
        public virtual ICollection<BookingModel> GuidedBookings { get; set; }
        public virtual ICollection<DestinationModel> CreatedDestinations { get; set; }
        public virtual ICollection<HotelModel> CreatedHotels { get; set; }
    }
}
