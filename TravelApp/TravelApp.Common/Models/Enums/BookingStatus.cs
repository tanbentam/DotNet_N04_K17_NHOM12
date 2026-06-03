namespace TravelApp.Common.Models.Enums
{
    // Quản lý trạng thái Booking
    public enum BookingStatus
    {
        Pending,    // Đang chờ Guide xác nhận
        Accepted,   // Guide đã chấp nhận
        Rejected,   // Guide từ chối
        Paid,       // Đã thanh toán thành công
        Cancelled,  // User hủy đơn
        Completed   // Đã hoàn thành chuyến đi
    }
}
