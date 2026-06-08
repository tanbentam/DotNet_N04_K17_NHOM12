using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Services.Logging;
using TravelApp.Services.Contracts;

namespace TravelApp.ViewModels.Authentication
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IRoleNavigationService _navigationService;
        private readonly IUserSessionService _sessionService;

        [ObservableProperty]
        private string _identifier;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(
            IAuthService authService,
            IRoleNavigationService navigationService,
            IUserSessionService sessionService)
        {
            _authService = authService;
            _navigationService = navigationService;
            _sessionService = sessionService;
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
                    Password = string.Empty;
                    _sessionService.SignIn(user);
                    _navigationService.NavigateToDashboard(user);
                }
                else
                {
                    ErrorMessage = "Thông tin đăng nhập không chính xác.";
                    // [BACKEND DEVELOPER NOTE] Ghi log lỗi đăng nhập (Login failures) vào file cục bộ 
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report("Login", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
