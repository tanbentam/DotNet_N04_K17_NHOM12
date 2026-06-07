using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<UserModel>> GetAllAsync();
        Task<UserModel> FindByIdentifierAsync(string emailOrPhone);
        Task<bool> CreateAsync(UserModel user);
        Task<bool> UpdateAsync(UserModel user, string passwordHash);
        Task<bool> DeleteAsync(int userId);
    }
}
