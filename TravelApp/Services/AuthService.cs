using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Services.Contracts;
using TravelApp.Utils;

namespace TravelApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> LoginAsync(string emailOrPhone, string password)
        {
            if (string.IsNullOrWhiteSpace(emailOrPhone) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await _userRepository.FindByIdentifierAsync(emailOrPhone);
            if (user == null ||
                !PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public Task<bool> RegisterAsync(UserModel user, string password)
        {
            // Persisting registrations is implemented in the next checklist item.
            return Task.FromResult(true);
        }
    }
}
