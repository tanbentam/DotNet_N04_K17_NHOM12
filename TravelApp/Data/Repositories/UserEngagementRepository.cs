using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Logging;

namespace TravelApp.Data.Repositories
{
    public sealed class UserEngagementRepository :
        IUserEngagementRepository
    {
        public async Task<IReadOnlyList<FavoriteModel>> GetFavoritesAsync(
            int userId)
        {
            if (userId <= 0)
            {
                return new List<FavoriteModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Favorites
                    .AsNoTracking()
                    .Include(item => item.Hotel)
                    .Include(item => item.Guide)
                    .Where(item => item.UserId == userId)
                    .OrderByDescending(item => item.CreatedAt)
                    .ToListAsync();
            }
        }

        public Task<bool> AddHotelFavoriteAsync(int userId, int hotelId)
        {
            return AddFavoriteAsync(userId, hotelId, null);
        }

        public Task<bool> AddGuideFavoriteAsync(int userId, int guideId)
        {
            return AddFavoriteAsync(userId, null, guideId);
        }

        public async Task<bool> RemoveFavoriteAsync(
            int favoriteId,
            int userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var favorite = await context.Favorites.FirstOrDefaultAsync(
                    item => item.Id == favoriteId && item.UserId == userId);
                if (favorite == null)
                {
                    return false;
                }

                context.Favorites.Remove(favorite);
                return await SaveChangesAsync(context);
            }
        }

        public async Task<IReadOnlyList<ReviewModel>> GetReviewsAsync(
            int userId)
        {
            if (userId <= 0)
            {
                return new List<ReviewModel>();
            }

            using (var context = new ApplicationDbContext())
            {
                return await context.Reviews
                    .AsNoTracking()
                    .Include(item => item.Hotel)
                    .Include(item => item.Guide)
                    .Where(item => item.UserId == userId)
                    .OrderByDescending(item => item.UpdatedAt)
                    .ToListAsync();
            }
        }

        public Task<bool> SaveHotelReviewAsync(
            int userId,
            int hotelId,
            int rating,
            string comment)
        {
            return SaveReviewAsync(
                userId,
                hotelId,
                null,
                rating,
                comment);
        }

        public Task<bool> SaveGuideReviewAsync(
            int userId,
            int guideId,
            int rating,
            string comment)
        {
            return SaveReviewAsync(
                userId,
                null,
                guideId,
                rating,
                comment);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            using (var context = new ApplicationDbContext())
            {
                var review = await context.Reviews.FirstOrDefaultAsync(
                    item => item.Id == reviewId && item.UserId == userId);
                if (review == null)
                {
                    return false;
                }

                context.Reviews.Remove(review);
                return await SaveChangesAsync(context);
            }
        }

        private static async Task<bool> AddFavoriteAsync(
            int userId,
            int? hotelId,
            int? guideId)
        {
            if (userId <= 0 || hotelId.HasValue == guideId.HasValue)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                if (!await IsValidTargetAsync(
                    context,
                    userId,
                    hotelId,
                    guideId))
                {
                    return false;
                }

                var exists = await context.Favorites.AnyAsync(item =>
                    item.UserId == userId &&
                    item.HotelId == hotelId &&
                    item.GuideId == guideId);
                if (exists)
                {
                    return false;
                }

                context.Favorites.Add(new FavoriteModel
                {
                    UserId = userId,
                    HotelId = hotelId,
                    GuideId = guideId,
                    CreatedAt = DateTime.Now
                });
                return await SaveChangesAsync(context);
            }
        }

        private static async Task<bool> SaveReviewAsync(
            int userId,
            int? hotelId,
            int? guideId,
            int rating,
            string comment)
        {
            if (userId <= 0 ||
                hotelId.HasValue == guideId.HasValue ||
                rating < 1 ||
                rating > 5 ||
                (comment?.Trim().Length ?? 0) > 1000)
            {
                return false;
            }

            using (var context = new ApplicationDbContext())
            {
                if (!await IsValidTargetAsync(
                    context,
                    userId,
                    hotelId,
                    guideId))
                {
                    return false;
                }

                var review = await context.Reviews.FirstOrDefaultAsync(item =>
                    item.UserId == userId &&
                    item.HotelId == hotelId &&
                    item.GuideId == guideId);
                var now = DateTime.Now;

                if (review == null)
                {
                    context.Reviews.Add(new ReviewModel
                    {
                        UserId = userId,
                        HotelId = hotelId,
                        GuideId = guideId,
                        Rating = rating,
                        Comment = comment?.Trim(),
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
                else
                {
                    review.Rating = rating;
                    review.Comment = comment?.Trim();
                    review.UpdatedAt = now;
                }

                return await SaveChangesAsync(context);
            }
        }

        private static async Task<bool> IsValidTargetAsync(
            ApplicationDbContext context,
            int userId,
            int? hotelId,
            int? guideId)
        {
            var userExists = await context.Users.AnyAsync(user =>
                user.Id == userId && user.Role == RoleType.User);
            if (!userExists)
            {
                return false;
            }

            if (hotelId.HasValue)
            {
                return await context.Hotels.AnyAsync(hotel =>
                    hotel.Id == hotelId.Value &&
                    hotel.ApprovalStatus == ContentApprovalStatus.Approved);
            }

            return await context.Users.AnyAsync(guide =>
                guide.Id == guideId.Value &&
                guide.Role == RoleType.TourGuide);
        }

        private static async Task<bool> SaveChangesAsync(
            ApplicationDbContext context)
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                LoggerService.LogException(
                    "Save user engagement repository",
                    ex);
                return false;
            }
        }
    }
}
