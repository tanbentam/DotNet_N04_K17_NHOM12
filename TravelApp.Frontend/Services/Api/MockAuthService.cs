using System;
using System.Threading.Tasks;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.Services.Api
{
    public class MockAuthService : IAuthService
    {
        public Task<UserModel> LoginAsync(string identifier, string password)
        {
            // API INTEGRATION POINT:
            // Replace this mock with POST /api/auth/login.
            // Request: { identifier, password }
            // Response: { email, phoneNumber, province, role, token }
            if (string.Equals(identifier, "admin", StringComparison.OrdinalIgnoreCase) && password == "admin")
            {
                return Task.FromResult(new UserModel
                {
                    Email = "admin@travelapp.local",
                    PhoneNumber = "0000000000",
                    Province = "System",
                    Role = "Admin",
                    Token = "mock-admin-token"
                });
            }

            if (string.Equals(identifier, "guide@travelapp.local", StringComparison.OrdinalIgnoreCase) && password == "guide")
            {
                return Task.FromResult(new UserModel
                {
                    Email = "guide@travelapp.local",
                    PhoneNumber = "0900000001",
                    Province = "Da Nang",
                    Role = "Guide",
                    Token = "mock-guide-token"
                });
            }

            if (string.Equals(identifier, "user@travelapp.local", StringComparison.OrdinalIgnoreCase) && password == "user")
            {
                return Task.FromResult(new UserModel
                {
                    Email = "user@travelapp.local",
                    PhoneNumber = "0900000002",
                    Province = "Ho Chi Minh City",
                    Role = "User",
                    Token = "mock-user-token"
                });
            }

            return Task.FromResult<UserModel>(null);
        }

        public Task<bool> RegisterAsync(UserModel user, string password)
        {
            // API INTEGRATION POINT:
            // Replace this mock with POST /api/auth/register.
            // Request: { email, password, phoneNumber, province }
            // Response: { success, message }
            return Task.FromResult(user != null && !string.IsNullOrWhiteSpace(password));
        }
    }
}
