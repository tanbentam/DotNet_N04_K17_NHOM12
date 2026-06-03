using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Services.Contracts;

namespace TravelApp.ViewModels.Authentication
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _identifier;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập Email/Số điện thoại và Mật khẩu.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.LoginAsync(Identifier, Password);
                if (user != null)
                {
                    // Logic điều hướng (Navigation) sẽ được triển khai thông qua Strategy Pattern sau
                    // Điều hướng sang Dashboard tương ứng dựa trên user.Role
                }
                else
                {
                    ErrorMessage = "Thông tin đăng nhập không chính xác.";
                    // [BACKEND DEVELOPER NOTE] Ghi log lỗi đăng nhập (Login failures) vào file cục bộ 
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Không thể kết nối đến máy chủ.";
                // [BACKEND DEVELOPER NOTE] Ghi log API errors
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
