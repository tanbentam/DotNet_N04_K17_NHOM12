using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.User
{
    public partial class UserEngagementViewModel : ObservableObject
    {
        private readonly IUserEngagementRepository _engagementRepository;
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private ObservableCollection<HotelModel> _hotels;
        [ObservableProperty] private ObservableCollection<UserModel> _guides;
        [ObservableProperty] private ObservableCollection<FavoriteModel> _favorites;
        [ObservableProperty] private ObservableCollection<ReviewModel> _reviews;
        [ObservableProperty] private HotelModel _selectedHotel;
        [ObservableProperty] private UserModel _selectedGuide;
        [ObservableProperty] private int _hotelRating = 5;
        [ObservableProperty] private int _guideRating = 5;
        [ObservableProperty] private string _hotelComment;
        [ObservableProperty] private string _guideComment;
        [ObservableProperty] private string _engagementMessage;
        [ObservableProperty] private bool _isBusy;

        public UserEngagementViewModel(
            IUserEngagementRepository engagementRepository,
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _engagementRepository = engagementRepository;
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            Hotels = new ObservableCollection<HotelModel>();
            Guides = new ObservableCollection<UserModel>();
            Favorites = new ObservableCollection<FavoriteModel>();
            Reviews = new ObservableCollection<ReviewModel>();
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return;
            }

            IsBusy = true;
            EngagementMessage = string.Empty;
            try
            {
                var hotelsTask =
                    _contentRepository.SearchApprovedHotelsAsync(
                        null,
                        0,
                        0,
                        null);
                var guidesTask = _contentRepository.SearchGuidesAsync(
                    null,
                    null);
                var favoritesTask =
                    _engagementRepository.GetFavoritesAsync(user.Id);
                var reviewsTask =
                    _engagementRepository.GetReviewsAsync(user.Id);

                await Task.WhenAll(
                    hotelsTask,
                    guidesTask,
                    favoritesTask,
                    reviewsTask);

                Hotels = new ObservableCollection<HotelModel>(
                    await hotelsTask);
                Guides = new ObservableCollection<UserModel>(
                    await guidesTask);
                Favorites = new ObservableCollection<FavoriteModel>(
                    await favoritesTask);
                Reviews = new ObservableCollection<ReviewModel>(
                    await reviewsTask);
            }
            catch (Exception ex)
            {
                EngagementMessage = "Không thể tải dữ liệu: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private Task AddHotelFavoriteAsync()
        {
            if (SelectedHotel == null)
            {
                EngagementMessage = "Hãy chọn khách sạn cần yêu thích.";
                return Task.CompletedTask;
            }

            return AddFavoriteAsync(
                () => _engagementRepository.AddHotelFavoriteAsync(
                    _sessionService.CurrentUser.Id,
                    SelectedHotel.Id));
        }

        [RelayCommand]
        private Task AddGuideFavoriteAsync()
        {
            if (SelectedGuide == null)
            {
                EngagementMessage = "Hãy chọn Guide cần yêu thích.";
                return Task.CompletedTask;
            }

            return AddFavoriteAsync(
                () => _engagementRepository.AddGuideFavoriteAsync(
                    _sessionService.CurrentUser.Id,
                    SelectedGuide.Id));
        }

        [RelayCommand]
        private async Task RemoveFavoriteAsync(FavoriteModel favorite)
        {
            var user = GetCurrentUser();
            if (user == null || favorite == null)
            {
                return;
            }

            if (!await _engagementRepository.RemoveFavoriteAsync(
                favorite.Id,
                user.Id))
            {
                EngagementMessage = "Không thể bỏ mục yêu thích.";
                return;
            }

            await RefreshUserDataAsync(user.Id);
            EngagementMessage = "Đã bỏ khỏi danh sách yêu thích.";
        }

        [RelayCommand]
        private Task SaveHotelReviewAsync()
        {
            if (SelectedHotel == null)
            {
                EngagementMessage = "Hãy chọn khách sạn cần đánh giá.";
                return Task.CompletedTask;
            }

            return SaveReviewAsync(
                HotelRating,
                HotelComment,
                () => _engagementRepository.SaveHotelReviewAsync(
                    _sessionService.CurrentUser.Id,
                    SelectedHotel.Id,
                    HotelRating,
                    HotelComment));
        }

        [RelayCommand]
        private Task SaveGuideReviewAsync()
        {
            if (SelectedGuide == null)
            {
                EngagementMessage = "Hãy chọn Guide cần đánh giá.";
                return Task.CompletedTask;
            }

            return SaveReviewAsync(
                GuideRating,
                GuideComment,
                () => _engagementRepository.SaveGuideReviewAsync(
                    _sessionService.CurrentUser.Id,
                    SelectedGuide.Id,
                    GuideRating,
                    GuideComment));
        }

        [RelayCommand]
        private async Task DeleteReviewAsync(ReviewModel review)
        {
            var user = GetCurrentUser();
            if (user == null || review == null)
            {
                return;
            }

            if (!await _engagementRepository.DeleteReviewAsync(
                review.Id,
                user.Id))
            {
                EngagementMessage = "Không thể xóa đánh giá.";
                return;
            }

            await RefreshUserDataAsync(user.Id);
            EngagementMessage = "Đã xóa đánh giá.";
        }

        private async Task AddFavoriteAsync(Func<Task<bool>> addAction)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return;
            }

            IsBusy = true;
            try
            {
                if (!await addAction())
                {
                    EngagementMessage =
                        "Không thể thêm yêu thích hoặc mục này đã tồn tại.";
                    return;
                }

                await RefreshUserDataAsync(user.Id);
                EngagementMessage = "Đã thêm vào danh sách yêu thích.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveReviewAsync(
            int rating,
            string comment,
            Func<Task<bool>> saveAction)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return;
            }

            if (rating < 1 || rating > 5)
            {
                EngagementMessage = "Điểm đánh giá phải từ 1 đến 5.";
                return;
            }

            if ((comment?.Trim().Length ?? 0) > 1000)
            {
                EngagementMessage =
                    "Nội dung đánh giá không được vượt quá 1000 ký tự.";
                return;
            }

            IsBusy = true;
            try
            {
                if (!await saveAction())
                {
                    EngagementMessage = "Không thể lưu đánh giá.";
                    return;
                }

                await RefreshUserDataAsync(user.Id);
                EngagementMessage = "Đã lưu đánh giá.";
                _notificationManager.ShowNotification(
                    "Thành công",
                    "Đánh giá của bạn đã được lưu.",
                    false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshUserDataAsync(int userId)
        {
            var favoritesTask =
                _engagementRepository.GetFavoritesAsync(userId);
            var reviewsTask = _engagementRepository.GetReviewsAsync(userId);
            await Task.WhenAll(favoritesTask, reviewsTask);

            Favorites = new ObservableCollection<FavoriteModel>(
                await favoritesTask);
            Reviews = new ObservableCollection<ReviewModel>(
                await reviewsTask);
        }

        private UserModel GetCurrentUser()
        {
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                EngagementMessage = "Phiên đăng nhập User không hợp lệ.";
                return null;
            }

            return user;
        }
    }
}
