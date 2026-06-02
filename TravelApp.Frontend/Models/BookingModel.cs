using TravelApp.Frontend.Models.Enums;

namespace TravelApp.Frontend.Models
{
    public class BookingModel
    {
        public string BookingId { get; set; }
        public string DestinationName { get; set; }
        public string UserName { get; set; }
        public string HotelName { get; set; }

        // Bổ sung quản lý trạng thái đơn hàng
        public BookingStatus Status { get; set; }
    }
}
