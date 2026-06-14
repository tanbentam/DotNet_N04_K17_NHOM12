using System;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace TravelApp.Services.Logging
{
    public static class DatabaseErrorDiagnostics
    {
        private static readonly Regex PasswordPattern = new Regex(
            @"(?i)(password|pwd)\s*=\s*[^;]*",
            RegexOptions.Compiled);

        public static string Report(string operation, Exception exception)
        {
            var errorId = "DB-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            LoggerService.LogDatabaseError(errorId, operation, Sanitize(exception.ToString()));

            var root = exception.GetBaseException();
            return string.Join(
                Environment.NewLine,
                "Lỗi cơ sở dữ liệu [" + errorId + "]",
                GetFriendlyReason(exception, root),
                "Chi tiết: " + Sanitize(root.Message),
                "Loại lỗi: " + root.GetType().Name,
                "Xem TravelApp_ErrorLog.txt để có stack trace đầy đủ.");
        }

        private static string GetFriendlyReason(Exception exception, Exception root)
        {
            var message = (root.Message ?? string.Empty).ToLowerInvariant();

            if (message.Contains("has changed since the database was created") ||
                message.Contains("model backing the context has changed") ||
                message.Contains("__migrationhistory") ||
                message.Contains("pending changes") ||
                message.Contains("automatic migration is disabled"))
            {
                return "Mô hình EF và lịch sử migration trong cơ sở dữ liệu không cùng phiên bản.";
            }

            if (message.Contains("doesn't exist") ||
                message.Contains("does not exist") ||
                message.Contains("unknown column") ||
                message.Contains("no such table"))
            {
                return "Kết nối MySQL thành công nhưng schema đang thiếu bảng hoặc cột.";
            }

            if (message.Contains("access denied") ||
                message.Contains("authentication"))
            {
                return "MySQL từ chối tài khoản hoặc mật khẩu kết nối.";
            }

            if (message.Contains("timeout") ||
                message.Contains("unable to connect") ||
                message.Contains("connect to any of the specified"))
            {
                return "Không thể thiết lập kết nối tới máy chủ MySQL.";
            }

            if (exception is DbUpdateException)
            {
                return "Cơ sở dữ liệu từ chối thao tác ghi dữ liệu.";
            }

            return "Thao tác với cơ sở dữ liệu thất bại.";
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Không có thông tin lỗi từ provider.";
            }

            return PasswordPattern.Replace(value, "$1=***");
        }
    }
}
