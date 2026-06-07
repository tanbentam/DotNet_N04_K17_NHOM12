using System;
using TravelApp.Models.Enums;

namespace TravelApp.Models
{
    public class BookingModel
    {
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

        public virtual UserModel User { get; set; }
        public virtual UserModel Guide { get; set; }
        public virtual HotelModel Hotel { get; set; }
        public virtual DestinationModel Destination { get; set; }
    }
}
