using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Data;
using TravelApp.Services.Logging;

namespace TravelApp.Services
{
    public sealed class DatabaseConnectionResult
    {
        private DatabaseConnectionResult(bool isConnected, string message)
        {
            IsConnected = isConnected;
            Message = message;
        }

        public bool IsConnected { get; }
        public string Message { get; }

        public static DatabaseConnectionResult Success()
        {
            return new DatabaseConnectionResult(
                true,
                "Cơ sở dữ liệu sẵn sàng");
        }

        public static DatabaseConnectionResult Failure(string message)
        {
            return new DatabaseConnectionResult(false, message);
        }
    }

    public class DatabaseConnectionService
    {
        public async Task<DatabaseConnectionResult> CheckConnectionAsync()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    // A real query verifies connection, EF migrations and the current schema.
                    await context.Users
                        .AsNoTracking()
                        .Select(user => user.Id)
                        .FirstOrDefaultAsync();
                    return DatabaseConnectionResult.Success();
                }
            }
            catch (Exception ex)
            {
                return DatabaseConnectionResult.Failure(
                    DatabaseErrorDiagnostics.Report("Startup health check", ex));
            }
        }
    }
}
