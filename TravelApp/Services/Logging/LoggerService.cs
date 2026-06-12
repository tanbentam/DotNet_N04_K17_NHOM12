using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TravelApp.Services.Logging
{
    public static class LoggerService
    {
        private static readonly object SyncRoot = new object();
        private static readonly string LogFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "TravelApp_ErrorLog.txt");
        private static readonly Regex SensitiveValuePattern = new Regex(
            @"(?i)(password|pwd|token|secret|connectionstring)\s*[:=]\s*[^;\s]+",
            RegexOptions.Compiled);

        public static string LogException(
            string operation,
            Exception exception,
            string context = null)
        {
            var errorId = CreateErrorId("APP");
            WriteLog(
                "ERROR",
                errorId + "/" + Normalize(operation),
                JoinContext(context, exception?.ToString()));
            return errorId;
        }

        public static void LogWarning(
            string operation,
            string message,
            string context = null)
        {
            WriteLog(
                "WARNING",
                Normalize(operation),
                JoinContext(context, message));
        }

        public static void LogLoginFailure(string identifier, string reason)
        {
            WriteLog(
                "LOGIN_FAILURE",
                MaskIdentifier(identifier),
                Sanitize(reason));
        }

        public static void LogBookingFailure(string userId, string reason)
        {
            WriteLog(
                "BOOKING_FAILURE",
                "UserId=" + Normalize(userId),
                Sanitize(reason));
        }

        public static void LogDatabaseConnectionFailure(string reason)
        {
            WriteLog(
                "DATABASE_CONNECTION",
                "ApplicationStartup",
                Sanitize(reason));
        }

        public static void LogDatabaseError(
            string errorId,
            string operation,
            string diagnosticDetails)
        {
            WriteLog(
                "DATABASE_ERROR",
                Normalize(errorId) + "/" + Normalize(operation),
                diagnosticDetails);
        }

        private static void WriteLog(
            string logType,
            string context,
            string message)
        {
            try
            {
                var logEntry = string.Format(
                    "[{0:yyyy-MM-dd HH:mm:ss}] [{1}] [{2}] {3}{4}",
                    DateTime.Now,
                    Normalize(logType),
                    Normalize(context),
                    Sanitize(message),
                    Environment.NewLine);

                lock (SyncRoot)
                {
                    File.AppendAllText(LogFilePath, logEntry);
                }
            }
            catch
            {
                Console.WriteLine("Unable to write application log.");
            }
        }

        private static string JoinContext(string context, string message)
        {
            if (string.IsNullOrWhiteSpace(context))
            {
                return message;
            }

            return context.Trim() + Environment.NewLine + message;
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "No diagnostic details.";
            }

            return SensitiveValuePattern.Replace(value, "$1=***");
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "Unknown"
                : value.Trim().Replace(Environment.NewLine, " ");
        }

        private static string MaskIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return "Unknown";
            }

            var value = identifier.Trim();
            var atIndex = value.IndexOf('@');
            if (atIndex > 1)
            {
                return value.Substring(0, 1) + "***" +
                    value.Substring(atIndex);
            }

            if (value.Length > 4)
            {
                return "***" + value.Substring(value.Length - 4);
            }

            return "***";
        }

        private static string CreateErrorId(string prefix)
        {
            return prefix + "-" +
                DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
        }
    }
}
