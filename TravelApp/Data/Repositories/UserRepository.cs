using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        public async Task<IReadOnlyList<UserModel>> GetAllAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.Users
                    .AsNoTracking()
                    .OrderBy(user => user.FullName)
                    .ToListAsync();
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
