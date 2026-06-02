using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Frontend.Contracts;

namespace TravelApp.Frontend.ViewModels.Authentication
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _identifier;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your email/phone and password.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.LoginAsync(Identifier, Password);
                if (user != null)
                {
                    _navigationService.NavigateToRoleDashboard(user);
                    return;
                }

                ErrorMessage = "The login information is incorrect.";
                // BACKEND DEVELOPER NOTE: log failed login attempts after Auth API integration.
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
        private void GoToRegister()
        {
            _navigationService.NavigateToRegister();
        }
    }
}
