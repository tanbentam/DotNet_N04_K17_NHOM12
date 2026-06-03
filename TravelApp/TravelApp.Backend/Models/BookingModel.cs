using System;

namespace TravelApp.Backend.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GuideId { get; set; }
        public int HotelId { get; set; }
        public DateTime StartDate { get; set; }
        public int Nights { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}