using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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

        public async Task<UserModel> FindByIdentifierAsync(string emailOrPhone)
        {
            if (string.IsNullOrWhiteSpace(emailOrPhone))
            {
                return null;
            }

            var identifier = emailOrPhone.Trim();

            using (var context = new ApplicationDbContext())
            {
                return await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(user =>
                        user.Email == identifier ||
                        user.Phone == identifier);
            }
        }

        public async Task<bool> CreateAsync(UserModel user)
        {
            if (user == null)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var exists = await context.Users.AnyAsync(existingUser =>
                    existingUser.Email == user.Email ||
                    existingUser.Phone == user.Phone);
                if (exists)
                {
                    return false;
                }

                context.Users.Add(user);

                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
                    // Unique indexes remain the final guard against concurrent registration.
                    return false;
                }
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
