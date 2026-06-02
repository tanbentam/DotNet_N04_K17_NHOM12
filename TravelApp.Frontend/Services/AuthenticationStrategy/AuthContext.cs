using System;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels;

namespace TravelApp.Frontend.Services.AuthenticationStrategy
{
    public class AuthContext
    {
        private IAuthStrategy _authStrategy;

        public void SetStrategy(string role)
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                _authStrategy = new AdminAuthStrategy();
                return;
            }

            if (string.Equals(role, "Guide", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "TourGuide", StringComparison.OrdinalIgnoreCase))
            {
                _authStrategy = new GuideAuthStrategy();
                return;
            }

            if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
            {
                _authStrategy = new UserAuthStrategy();
                return;
            }

            throw new ArgumentException("Invalid role.");
        }

        public void ExecuteNavigation(MainViewModel mainViewModel, UserModel currentUser)
        {
            if (_authStrategy == null)
            {
                throw new InvalidOperationException("Strategy chưa được thiết lập.");
            }

            _authStrategy.NavigateToDashboard(mainViewModel, currentUser);
        }
    }
}
