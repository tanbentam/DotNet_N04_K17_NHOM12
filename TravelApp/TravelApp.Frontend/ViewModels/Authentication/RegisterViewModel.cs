using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using System.Threading.Tasks;
using TravelApp.Common.Contracts;
using TravelApp.Common.Models;
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
        [cite_start]
        private async Task RegisterAsync() // Xử lý bất đồng bộ [cite: 156-157]
        {
            [cite_start]// Validation chuyên nghiệp [cite: 130-131]
            if (!ValidationHelper.IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ."; 
                return;
            }
            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                ErrorMessage = "Số điện thoại phải bao gồm chính xác 10 chữ số."; // [cite: 46]
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
                    [cite_start]PhoneNumber = PhoneNumber, // [cite: 44-46]
                    [cite_start]Province = Province,       // [cite: 47]
                    [cite_start]Role = "User"              // Mặc định tự đăng ký là User 
                };

                var success = await _authService.RegisterAsync(newUser, Password);
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
                [cite_start]// [BACKEND DEVELOPER NOTE] Ghi log lỗi API [cite: 168-170]
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}