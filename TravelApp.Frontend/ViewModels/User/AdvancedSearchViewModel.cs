using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.ViewModels.User
{
    public partial class AdvancedSearchViewModel : ObservableObject
    {
        [ObservableProperty] private string _searchProvince;
        [ObservableProperty] private decimal _searchMaxPrice;
        [ObservableProperty] private double _searchMinRating;
        [ObservableProperty] private string _searchGuideName;
        [ObservableProperty] private string _searchTime;

        [ObservableProperty] private ObservableCollection<DestinationModel> _searchResults;
        [ObservableProperty] private bool _isSearching;

        public AdvancedSearchViewModel()
        {
            SearchResults = new ObservableCollection<DestinationModel>();
            SeedSearchResults();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            IsSearching = true;

            // API INTEGRATION POINT:
            // Replace this mock with GET /api/search?province=&maxPrice=&rating=&guide=&time=.
            // Expected response should combine destinations, hotel hints, and available guide slots.
            await Task.Delay(1000);

            if (SearchResults.Count == 0)
            {
                SeedSearchResults();
            }

            IsSearching = false;
        }

        private void SeedSearchResults()
        {
            SearchResults.Add(new DestinationModel
            {
                Id = 1,
                Name = "Da Nang Beach",
                Province = "Da Nang",
                Description = "Coastal destination with hotels, food tours, and flexible guide schedules.",
                Rating = 4.7,
                GuidePriceFrom = 450000
            });

            SearchResults.Add(new DestinationModel
            {
                Id = 2,
                Name = "Hoi An Ancient Town",
                Province = "Quang Nam",
                Description = "Walking tours, lantern streets, local food, and heritage hotels.",
                Rating = 4.8,
                GuidePriceFrom = 520000
            });
        }
    }
}
