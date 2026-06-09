using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TravelApp.Models;
using TravelApp.Services.Logging;

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
                catch (DbUpdateException ex) when (IsDuplicateKey(ex))
                {
                    // Unique indexes remain the final guard against concurrent registration.
                    LoggerService.LogWarning(
                        "Create user",
                        "Database rejected a duplicate email or phone.",
                        "Role=" + user.Role);
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
                catch (DbUpdateException ex)
                {
                    LoggerService.LogException(
                        "Update user repository",
                        ex,
                        "UserId=" + user.Id);
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
                catch (DbUpdateException ex)
                {
                    LoggerService.LogException(
                        "Delete user repository",
                        ex,
                        "UserId=" + userId);
                    return false;
                }
            }
        }

        private static bool IsDuplicateKey(DbUpdateException exception)
        {
            for (var current = exception as System.Exception;
                 current != null;
                 current = current.InnerException)
            {
                var mysqlException = current as MySqlException;
                if (mysqlException?.Number == 1062)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
