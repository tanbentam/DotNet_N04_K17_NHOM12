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

        public async Task<bool> UpdateAsync(
            UserModel user,
            string passwordHash)
        {
            if (user == null || user.Id <= 0)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var duplicateExists = await context.Users.AnyAsync(existingUser =>
                    existingUser.Id != user.Id &&
                    (existingUser.Email == user.Email ||
                     existingUser.Phone == user.Phone));
                if (duplicateExists)
                {
                    return false;
                }

                var existing = await context.Users.FindAsync(user.Id);
                if (existing == null)
                {
                    return false;
                }

                existing.Email = user.Email;
                existing.Phone = user.Phone;
                existing.FullName = user.FullName;
                existing.Role = user.Role;

                if (!string.IsNullOrWhiteSpace(passwordHash))
                {
                    existing.PasswordHash = passwordHash;
                }

                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
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

                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
                    return false;
                }
            }
        }
    }
}
