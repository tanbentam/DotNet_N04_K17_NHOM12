using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Models;

namespace TravelApp.ViewModels.User
{
    public partial class AdvancedSearchViewModel : ObservableObject
    {
        // Các bộ lọc tìm kiếm
        [ObservableProperty] private string _searchProvince;     // Province
        [ObservableProperty] private decimal _searchMaxPrice;    // Price
        [ObservableProperty] private double _searchMinRating;    // Rating
        [ObservableProperty] private string _searchGuideName;    // Guide
        [ObservableProperty] private string _searchTime;         // Time

        [ObservableProperty] private ObservableCollection<DestinationModel> _searchResults;
        [ObservableProperty] private bool _isSearching;

        public AdvancedSearchViewModel()
        {
            SearchResults = new ObservableCollection<DestinationModel>();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            IsSearching = true;

            // [BACKEND DEVELOPER NOTE] 
            // Cần xây dựng API GET search: vd: Constants.User_SearchHotels_Endpoint
            // Endpoint này phải nhận các query parameters: Province, Price, Rating, Guide, Time.
            // Kết quả trả về phải gộp Điểm đến và Khách sạn.

            await Task.Delay(1000); // Bất đồng bộ - Giả lập call API

            IsSearching = false;
        }
    }
}