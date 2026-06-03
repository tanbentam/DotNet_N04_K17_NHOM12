using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.Contracts;
using TravelApp.Frontend.Utils;

namespace TravelApp.Frontend.ViewModels.Authentication
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _phoneNumber;

        [ObservableProperty]
        private string _province;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            // Validation
            if (!ValidationHelper.IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ."; 
                return;
            }
            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                ErrorMessage = "Số điện thoại phải bao gồm chính xác 10 chữ số.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Province))
            {
                ErrorMessage = "Vui lòng điền đầy đủ mật khẩu và tỉnh/thành phố.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var newUser = new UserModel
                {
                    Email = Email,
                    Phone = PhoneNumber,
                    FullName = Province,
                };

                // TODO: Implement RegisterAsync in IAuthService
                // var success = await _authService.RegisterAsync(newUser, Password);
                var success = true;
                if (success)
                {
                    // Chuyển hướng sang trang Đăng nhập sau khi đăng ký thành công
                    // Logic Navigation sẽ được xử lý ở MainViewModel
                }
                else
                {
                    ErrorMessage = "Đăng ký thất bại. Email hoặc Số điện thoại có thể đã tồn tại.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối đến máy chủ.";
                // [BACKEND DEVELOPER NOTE] Ghi log lỗi API
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
