namespace TravelApp.Frontend.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; } // Exactly 10 digits 
        public string Province { get; set; }
        public string Role { get; set; } // "Admin", "Guide", hoặc "User"
        public string Token { get; set; } // JWT Token từ Backend
    }
}