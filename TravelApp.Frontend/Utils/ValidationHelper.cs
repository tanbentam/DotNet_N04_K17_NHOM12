using System.Text.RegularExpressions;

namespace TravelApp.Frontend.Utils
{
    public static class ValidationHelper
    {
        // Yêu cầu số điện thoại phải đúng 10 chữ số 
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
            return Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
