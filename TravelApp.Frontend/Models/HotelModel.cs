namespace TravelApp.Frontend.Models
{
    public class HotelModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public decimal PricePerNight { get; set; }
        public double Rating { get; set; }
        public bool IsApproved { get; set; }
    }
}
