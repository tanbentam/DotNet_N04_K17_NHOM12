using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentAdminContent;

        private readonly AccountManagementViewModel _accountManagementVM;
        private readonly ContentManagementViewModel _contentManagementVM;

        public AdminDashboardViewModel(
            AccountManagementViewModel accountManagementVM,
            ContentManagementViewModel contentManagementVM)
        {
            _accountManagementVM = accountManagementVM;
            _contentManagementVM = contentManagementVM;

            // Mặc định hiển thị màn hình Quản lý tài khoản
            CurrentAdminContent = _accountManagementVM;
        }

        [RelayCommand]
        private void NavigateToAccounts()
        {
            // Điều hướng sang màn hình quản lý Guide/User 
            CurrentAdminContent = _accountManagementVM;
        }

        [RelayCommand]
        private void NavigateToContent()
        {
            // Điều hướng sang màn hình quản lý Destination/Hotel/Booking 
            CurrentAdminContent = _contentManagementVM;
        }

        [RelayCommand]
        private void Logout()
        {
            // Logic đăng xuất và quay về LoginView
        }
    }
}
