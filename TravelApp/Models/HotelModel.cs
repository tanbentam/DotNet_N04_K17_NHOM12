namespace TravelApp.Models
{
    public class HotelModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int Rating { get; set; }
        public string ImageUrl { get; set; }
    }
}
