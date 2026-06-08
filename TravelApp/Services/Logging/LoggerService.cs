using System;
using System.IO;

namespace TravelApp.Services.Logging
{
    public static class LoggerService
    {
        private static readonly string LogFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TravelApp_ErrorLog.txt");

        private static void WriteLog(string logType, string context, string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logType}] [{context}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Fallback nếu không thể ghi file
                Console.WriteLine("Lỗi không thể ghi log hệ thống.");
            }
        }

        // 1. Log khi đăng nhập thất bại
        public static void LogLoginFailure(string identifier, string reason)
        {
            WriteLog("LOGIN_FAILURE", identifier, reason);
        }

        // 2. Log khi có lỗi từ API (Timeout, 500, 404, v.v.)
        public static void LogApiError(string endpoint, string errorMessage)
        {
            WriteLog("API_ERROR", endpoint, errorMessage);
        }

        // 3. Log khi xử lý Booking (hoặc thanh toán) thất bại
        public static void LogBookingFailure(string userId, string reason)
        {
            WriteLog("BOOKING_FAILURE", userId, reason);
        }

        public static void LogDatabaseConnectionFailure(string reason)
        {
            WriteLog("DATABASE_CONNECTION", "ApplicationStartup", reason);
        }

        public static void LogDatabaseError(
            string errorId,
            string operation,
            string diagnosticDetails)
        {
            WriteLog(
                "DATABASE_ERROR",
                errorId + "/" + operation,
                diagnosticDetails);
        }
    }
}
