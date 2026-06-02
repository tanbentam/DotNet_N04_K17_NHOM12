using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TravelApp.Frontend.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _windowTitle = "Digital Travel Application";

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel()
        {
            // Khởi tạo mặc định: Sẽ điều hướng đến LoginViewModel trong các bước tới
        }

        // [BACKEND DEVELOPER NOTE]
        // Các phương thức thay đổi CurrentViewModel ở đây sẽ được gọi sau khi Auth API 
        // trả về Role của User để quyết định xem hiển thị Dashboard của Admin, Guide hay User.
    }
}