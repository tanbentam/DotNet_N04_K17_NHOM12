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
            _authStrategy = role switch
            {
                "Admin" => new AdminAuthStrategy(),
                "Guide" => new GuideAuthStrategy(),
                "User" => new UserAuthStrategy(),
                _ => throw new ArgumentException("Vai trò không hợp lệ.")
            };
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