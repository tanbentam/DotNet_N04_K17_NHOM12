using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public sealed class GuideAvailabilityRepository : IGuideAvailabilityRepository
    {
        public async Task<IReadOnlyList<GuideAvailabilityModel>> GetByGuideIdAsync(
            int guideId)
        {
            using (var context = new ApplicationDbContext())
            {
                return await context.GuideAvailabilities
                    .AsNoTracking()
                    .Where(item => item.GuideId == guideId)
                    .OrderBy(item => item.DayOfWeek)
                    .ToListAsync();
            }
        }

        public async Task<bool> SaveWeeklyScheduleAsync(
            int guideId,
            IEnumerable<GuideAvailabilityModel> weeklySchedule)
        {
            if (guideId <= 0 || weeklySchedule == null)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                var guideExists = await context.Users.AnyAsync(user => user.Id == guideId);
                if (!guideExists)
                {
                    return false;
                }

                var existing = await context.GuideAvailabilities
                    .Where(item => item.GuideId == guideId)
                    .ToListAsync();
                context.GuideAvailabilities.RemoveRange(existing);

                var now = DateTime.UtcNow;
                foreach (var item in weeklySchedule)
                {
                    context.GuideAvailabilities.Add(new GuideAvailabilityModel
                    {
                        GuideId = guideId,
                        DayOfWeek = item.DayOfWeek,
                        DayName = item.DayName,
                        IsAvailable = item.IsAvailable,
                        TimeSlot = item.IsAvailable ? item.TimeSlot : null,
                        UpdatedAt = now
                    });
                }

                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
                    return false;
                }
            }
        }
    }
}
