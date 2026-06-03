using System.Threading.Tasks;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.Services.Contracts
{
    public interface IAuthService
    {
        UserModel Authenticate(string emailOrPhone, string password);
        Task<UserModel> LoginAsync(string emailOrPhone, string password);
        Task<bool> RegisterAsync(UserModel user, string password);
    }
}
