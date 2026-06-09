using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.Data.Repositories
{
    public interface IUserEngagementRepository
    {
        Task<IReadOnlyList<FavoriteModel>> GetFavoritesAsync(int userId);
        Task<bool> AddHotelFavoriteAsync(int userId, int hotelId);
        Task<bool> AddGuideFavoriteAsync(int userId, int guideId);
        Task<bool> RemoveFavoriteAsync(int favoriteId, int userId);
        Task<IReadOnlyList<ReviewModel>> GetReviewsAsync(int userId);
        Task<bool> SaveHotelReviewAsync(
            int userId,
            int hotelId,
            int rating,
            string comment);
        Task<bool> SaveGuideReviewAsync(
            int userId,
            int guideId,
            int rating,
            string comment);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
    }
}
