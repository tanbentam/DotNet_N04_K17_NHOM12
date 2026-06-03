using TravelApp.Common.Models;

namespace TravelApp.Common.Contracts
{
    public interface IAuthService
    {
        UserModel Authenticate(string emailOrPhone, string password);
    }
}