using System;
using TravelApp.Backend.Models;

namespace TravelApp.Backend.Contracts
{
    public interface IAuthService
    {
        UserModel Authenticate(string emailOrPhone, string password);
    }
}