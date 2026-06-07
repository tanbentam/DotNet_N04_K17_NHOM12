using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Utils;

namespace TravelApp.ViewModels.Authentication
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
                    Role = RoleType.User
                };

                var success = await _authService.RegisterAsync(newUser, Password);
                if (success)
                {
                    ErrorMessage = "Đăng ký thành công.";
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
