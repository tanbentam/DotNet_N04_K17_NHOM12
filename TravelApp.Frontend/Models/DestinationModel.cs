namespace TravelApp.Frontend.Models
{
    public class DestinationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Rating { get; set; }
        public decimal GuidePriceFrom { get; set; }
    }
}
