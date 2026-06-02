using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Utils;

namespace TravelApp.Frontend.ViewModels.Authentication
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

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

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            // API INTEGRATION POINT: keep these client-side checks before POST /api/auth/register.
            if (!ValidationHelper.IsValidEmail(Email))
            {
                ErrorMessage = "Email is invalid.";
                return;
            }

            if (!ValidationHelper.IsValidPhoneNumber(PhoneNumber))
            {
                ErrorMessage = "Phone number must contain exactly 10 digits.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Province))
            {
                ErrorMessage = "Please enter password and current province/city.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var newUser = new UserModel
                {
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Province = Province,
                    Role = "User"
                };

                var success = await _authService.RegisterAsync(newUser, Password);
                if (success)
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                ErrorMessage = "Registration failed. Email or phone number may already exist.";
            }
            catch (Exception)
            {
                ErrorMessage = "Unable to connect to the server.";
                // BACKEND DEVELOPER NOTE: log API errors after Auth API integration.
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void GoToLogin()
        {
            _navigationService.NavigateToLogin();
        }
    }
}
