using System.Threading.Tasks;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.Contracts
{
    public interface IAuthService
    {
        // [BACKEND DEVELOPER NOTE] 
        // Payload mong đợi cho Login: { "identifier": "email hoặc phone", "password": "..." }
        // Phản hồi mong đợi: UserModel chứa thông tin và JWT Token.
        Task<UserModel> LoginAsync(string identifier, string password);

        // [BACKEND DEVELOPER NOTE]
        // Payload mong đợi cho Register: { "email", "password", "phoneNumber", "province" }
        Task<bool> RegisterAsync(UserModel user, string password);
    }
}
