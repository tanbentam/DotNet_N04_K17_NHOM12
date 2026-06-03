namespace TravelApp.Utils
{
    public static class Constants
    {
        // ---------------------------------------------------------
        // [BACKEND DEVELOPER NOTE] 
        // Thay đổi Base_API_Url này thành URL thực tế khi deploy Backend.
        // Frontend đang giả lập gọi đến localhost port 5001.
        // ---------------------------------------------------------
        public const string Base_API_Url = "https://localhost:5001/api/";

        // Các endpoint kết nối API (API Integration Points)
        public const string Auth_Login_Endpoint = Base_API_Url + "auth/login";
        public const string Auth_Register_Endpoint = Base_API_Url + "auth/register";

        public const string Admin_ManageAccounts_Endpoint = Base_API_Url + "admin/accounts";
        public const string Guide_ManageSchedule_Endpoint = Base_API_Url + "guide/schedule";
        public const string User_SearchHotels_Endpoint = Base_API_Url + "hotels/search";
        public const string User_BookTour_Endpoint = Base_API_Url + "bookings/create";

        // Hằng số Validation
        public const int PhoneNumberLength = 10;
    }
}