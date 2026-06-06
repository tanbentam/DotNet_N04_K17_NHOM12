using System;
using System.Data;
using System.Threading.Tasks;
using TravelApp.Data;

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
            return new DatabaseConnectionResult(true, "Database connected");
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
                    var connection = context.Database.Connection;
                    await connection.OpenAsync();

                    if (connection.State != ConnectionState.Open)
                    {
                        return DatabaseConnectionResult.Failure("Database unavailable");
                    }

                    connection.Close();
                    return DatabaseConnectionResult.Success();
                }
            }
            catch (Exception ex)
            {
                return DatabaseConnectionResult.Failure(ex.GetBaseException().Message);
            }
        }
    }
}
