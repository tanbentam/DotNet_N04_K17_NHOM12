using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TravelApp.Common.Models;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class ContentManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<DestinationModel> _destinations;

        [ObservableProperty]
        private ObservableCollection<HotelModel> _hotels;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _bookings;

        public ContentManagementViewModel()
        {
            Destinations = new ObservableCollection<DestinationModel>();
            Hotels = new ObservableCollection<HotelModel>();
            Bookings = new ObservableCollection<BookingModel>();

            LoadContentData();
        }

        private void LoadContentData()
        {
            // [BACKEND DEVELOPER NOTE] 
            // Cần tạo các API GET endpoints để Admin có thể lấy:
            [cite_start]// 1. Toàn bộ danh sách điểm đến 
            [cite_start]// 2. Toàn bộ thông tin khách sạn cần phê duyệt/quản lý 
            [cite_start]// 3. Toàn bộ đơn booking của hệ thống 
            [cite_start]// 4. Lịch trình khả dụng của các Guide (Thứ 2 - Chủ Nhật) 
        }
    }
}