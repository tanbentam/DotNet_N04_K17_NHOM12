namespace TravelApp.Services.Booking
{
    public sealed class BookingPriceQuote
    {
        public decimal GuideFee { get; set; }
        public decimal HotelFee { get; set; }
        public decimal Discount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Total { get; set; }
    }
}
