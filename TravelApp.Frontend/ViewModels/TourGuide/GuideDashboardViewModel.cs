using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public partial class GuideDashboardViewModel : ObservableObject
    {
        // View này sẽ được bind với một TabControl trong XAML để quản lý các tab:
        // 1. Destination Management
        // 2. Hotel Management 
        // 3. Schedule Management 
        // 4. Booking Requests 

        public GuideDashboardViewModel()
        {
            // Khởi tạo các ViewModel con nếu cần thiết lập DI chuyên sâu
        }
    }
}
