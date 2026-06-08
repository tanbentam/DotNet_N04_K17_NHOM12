using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public interface IGuideAvailabilityRepository
    {
        Task<IReadOnlyList<GuideAvailabilityModel>> GetByGuideIdAsync(int guideId);
        Task<bool> SaveWeeklyScheduleAsync(
            int guideId,
            IEnumerable<GuideAvailabilityModel> weeklySchedule);
    }
}
