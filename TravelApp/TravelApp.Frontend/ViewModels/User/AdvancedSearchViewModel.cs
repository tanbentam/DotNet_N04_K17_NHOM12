using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Common.Models;

namespace TravelApp.Frontend.ViewModels.User
{
    public partial class AdvancedSearchViewModel : ObservableObject
    {
        [cite_start]// Các bộ lọc tìm kiếm [cite: 146-150]
        [ObservableProperty] private string _searchProvince;     // Province [cite: 146]
        [ObservableProperty] private decimal _searchMaxPrice;    // Price [cite: 147]
        [ObservableProperty] private double _searchMinRating;    // Rating [cite: 148]
        [ObservableProperty] private string _searchGuideName;    // Guide [cite: 149]
        [ObservableProperty] private string _searchTime;         // Time [cite: 150]

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
            [cite_start]// Cần xây dựng API GET search: vd: Constants.User_SearchHotels_Endpoint [cite: 55, 189]
            // Endpoint này phải nhận các query parameters: Province, Price, Rating, Guide, Time.
            [cite_start]// Kết quả trả về phải gộp Điểm đến [cite: 61-63] và Khách sạn [cite: 55-57].

            await Task.Delay(1000); // Bất đồng bộ - Giả lập call API [cite: 156-157]

            IsSearching = false;
        }
    }
}