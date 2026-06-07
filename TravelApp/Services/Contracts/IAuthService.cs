using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Services.Contracts
{
    public interface IAuthService
    {
        Task<UserModel> LoginAsync(string emailOrPhone, string password);
        Task<bool> RegisterAsync(UserModel user, string password);
    }
}
