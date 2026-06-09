using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;

namespace TravelApp.ViewModels.User
{
    public partial class AdvancedSearchViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;

        [ObservableProperty] private string _searchProvince;
        [ObservableProperty] private decimal _searchMaxPrice;
        [ObservableProperty] private decimal _searchMinRating;
        [ObservableProperty] private string _searchGuideName;
        [ObservableProperty] private string _searchTime;

        [ObservableProperty] private ObservableCollection<DestinationModel> _destinationResults;
        [ObservableProperty] private ObservableCollection<HotelModel> _hotelResults;
        [ObservableProperty] private ObservableCollection<UserModel> _guideResults;
        [ObservableProperty] private bool _isSearching;
        [ObservableProperty] private string _searchMessage;

        public AdvancedSearchViewModel(
            ITravelContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
            DestinationResults = new ObservableCollection<DestinationModel>();
            HotelResults = new ObservableCollection<HotelModel>();
            GuideResults = new ObservableCollection<UserModel>();
            _ = SearchAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            SearchMessage = string.Empty;

            if (SearchMaxPrice < 0 || SearchMinRating < 0 ||
                SearchMinRating > 5)
            {
                SearchMessage =
                    "Giá không được âm và đánh giá phải nằm trong khoảng 0-5.";
                return;
            }

            IsSearching = true;
            try
            {
                var destinationsTask =
                    _contentRepository.SearchApprovedDestinationsAsync(
                        SearchProvince,
                        SearchMinRating,
                        SearchGuideName);
                var hotelsTask = _contentRepository.SearchApprovedHotelsAsync(
                    SearchProvince,
                    SearchMaxPrice,
                    SearchMinRating,
                    SearchGuideName);
                var guidesTask = _contentRepository.SearchGuidesAsync(
                    SearchGuideName,
                    SearchTime);

                await Task.WhenAll(
                    destinationsTask,
                    hotelsTask,
                    guidesTask);

                DestinationResults =
                    new ObservableCollection<DestinationModel>(
                        await destinationsTask);
                HotelResults = new ObservableCollection<HotelModel>(
                    await hotelsTask);
                GuideResults = new ObservableCollection<UserModel>(
                    await guidesTask);

                SearchMessage = string.Format(
                    "Tìm thấy {0} điểm đến, {1} khách sạn và {2} Guide.",
                    DestinationResults.Count,
                    HotelResults.Count,
                    GuideResults.Count);
            }
            catch (Exception ex)
            {
                SearchMessage = "Không thể tìm kiếm dữ liệu: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsSearching = false;
            }
        }
    }
}
