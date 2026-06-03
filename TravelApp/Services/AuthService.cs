using System;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.Contracts;

namespace TravelApp.Frontend.Services
{
    public class AuthService : IAuthService
    {
        public UserModel Authenticate(string emailOrPhone, string password)
        {
            // TODO: implement real authentication (hashing, DB lookup)
            if (emailOrPhone == "admin" && password == "admin")
            {
                return new UserModel { Id = 1, Email = "admin", FullName = "Administrator" };
            }
            return null;
        }

        public async Task<UserModel> LoginAsync(string emailOrPhone, string password)
        {
            return await Task.Run(() => Authenticate(emailOrPhone, password));
        }

        public async Task<bool> RegisterAsync(UserModel user, string password)
        {
            // TODO: implement real registration with DB
            return await Task.Run(() =>
            {
                // Placeholder for registration logic
                return true;
            });
        }
    }
}
