using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        // View này sẽ được bind với một TabControl trong XAML để quản lý các tab:
      /*  [cite_start]// 1. Destination Management [cite: 84-87]
        [cite_start]// 2. Hotel Management 
        [cite_start]// 3. Schedule Management 
        [cite_start]// 4. Booking Requests 
      */
        public GuideDashboardViewModel()
        {
            // Khởi tạo các ViewModel con nếu cần thiết lập DI chuyên sâu
        }
    }
}