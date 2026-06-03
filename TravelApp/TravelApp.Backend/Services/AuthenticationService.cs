using System;
using TravelApp.Common.Contracts;
using TravelApp.Common.Models;

namespace TravelApp.Backend.Services
{
    public class AuthenticationService : IAuthService
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
    }
}