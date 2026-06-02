namespace TravelApp.Frontend.Models
{
    public class GuideAvailabilityModel
    {
        public string GuideEmail { get; set; }
        public string DayOfWeek { get; set; }
        public string TimeSlot { get; set; }
        public bool IsAvailable { get; set; }
    }
}
