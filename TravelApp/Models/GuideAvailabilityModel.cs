using System;

namespace TravelApp.Models
{
    public class GuideAvailabilityModel
    {
        public int Id { get; set; }
        public int GuideId { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; }
        public bool IsAvailable { get; set; }
        public string TimeSlot { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual UserModel Guide { get; set; }
    }
}
