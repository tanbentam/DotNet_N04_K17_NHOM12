using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<UserModel>> GetAllAsync();
        Task<bool> DeleteAsync(int userId);
    }
}
