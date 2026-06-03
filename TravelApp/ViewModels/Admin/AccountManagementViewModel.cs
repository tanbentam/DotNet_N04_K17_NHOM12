using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class AccountManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserModel> _usersList;

        [ObservableProperty]
        private bool _isLoading;

        public AccountManagementViewModel()
        {
            UsersList = new ObservableCollection<UserModel>();
            LoadAccountsAsync();
        }

        private async void LoadAccountsAsync()
        {
            IsLoading = true;
            // [BACKEND DEVELOPER NOTE] 
            // Gọi API GET để lấy danh sách toàn bộ Users
            // Giả lập dữ liệu:
            await Task.Delay(1000);
            UsersList.Add(new UserModel { Email = "guide1@travel.com", FullName = "Guide", Phone = "0123456789" });
            UsersList.Add(new UserModel { Email = "user1@travel.com", FullName = "User", Phone = "0987654321" });

            IsLoading = false;
        }

        [RelayCommand]
        private void CreateGuideAccount()
        {
            // Mở Dialog/Popup form để tạo tài khoản Tour Guide
            // Chỉ Admin mới được quyền tạo tài khoản này 
        }

        [RelayCommand]
        private void CreateUserAccount()
        {
            // Mở Dialog/Popup form để tạo tài khoản User
        }

        [RelayCommand]
        private void DeleteAccount(UserModel user)
        {
            // [BACKEND DEVELOPER NOTE] Gọi API DELETE để xóa tài khoản
            if (user != null)
            {
                UsersList.Remove(user);
            }
        }
    }
}
