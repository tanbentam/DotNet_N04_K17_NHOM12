using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.TourGuide
{
    public partial class GuideContentManagementViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;
        private int _nextDestinationId = 3;
        private int _nextHotelId = 3;

        public ObservableCollection<string> ContentTypes { get; }
        public ObservableCollection<DestinationModel> Destinations { get; }
        public ObservableCollection<HotelModel> Hotels { get; }

        [ObservableProperty] private string _selectedContentType;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _province;
        [ObservableProperty] private string _description;
        [ObservableProperty] private string _address;
        [ObservableProperty] private string _imageUrl;
        [ObservableProperty] private decimal _price;
        [ObservableProperty] private double _rating;
        [ObservableProperty] private string _statusMessage;

        public GuideContentManagementViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            ContentTypes = new ObservableCollection<string> { "Destination", "Hotel" };
            SelectedContentType = "Destination";
            Destinations = new ObservableCollection<DestinationModel>();
            Hotels = new ObservableCollection<HotelModel>();
            SeedContent();
        }

        [RelayCommand]
        private async Task SaveContentAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Province))
            {
                StatusMessage = "Name and province/city are required.";
                return;
            }

            if (SelectedContentType == "Hotel" && string.IsNullOrWhiteSpace(Address))
            {
                StatusMessage = "Hotel address is required.";
                return;
            }

            // API INTEGRATION POINT:
            // Destination: POST /api/guide/destinations or PUT /api/guide/destinations/{id}.
            // Hotel: POST /api/guide/hotels or PUT /api/guide/hotels/{id}. New hotels must return IsApproved=false until Admin approves.
            await Task.Delay(300);

            if (SelectedContentType == "Hotel")
            {
                var hotel = new HotelModel
                {
                    Id = _nextHotelId++,
                    Name = Name.Trim(),
                    Province = Province.Trim(),
                    Address = Address.Trim(),
                    ImageUrl = ImageUrl,
                    PricePerNight = Price,
                    Rating = Rating,
                    IsApproved = false
                };

                Hotels.Add(hotel);
                _notificationManager.ShowNotification("Hotel saved", "Hotel was submitted and is pending admin approval.", false);
            }
            else
            {
                var destination = new DestinationModel
                {
                    Id = _nextDestinationId++,
                    Name = Name.Trim(),
                    Province = Province.Trim(),
                    Description = Description,
                    ImageUrl = ImageUrl,
                    Rating = Rating,
                    GuidePriceFrom = Price
                };

                Destinations.Add(destination);
                _notificationManager.ShowNotification("Destination saved", $"{destination.Name} was added.", false);
            }

            StatusMessage = "Content saved.";
            ClearForm();
        }

        [RelayCommand]
        private void LoadDestination(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            SelectedContentType = "Destination";
            Name = destination.Name;
            Province = destination.Province;
            Description = destination.Description;
            ImageUrl = destination.ImageUrl;
            Price = destination.GuidePriceFrom;
            Rating = destination.Rating;
            Address = string.Empty;
            StatusMessage = "Loaded destination for editing. Saving will add a new mock row until backend IDs are wired.";
        }

        [RelayCommand]
        private void LoadHotel(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            SelectedContentType = "Hotel";
            Name = hotel.Name;
            Province = hotel.Province;
            Address = hotel.Address;
            ImageUrl = hotel.ImageUrl;
            Price = hotel.PricePerNight;
            Rating = hotel.Rating;
            Description = string.Empty;
            StatusMessage = "Loaded hotel for editing. Saving will resubmit approval in the mock flow.";
        }

        [RelayCommand]
        private async Task DeleteDestinationAsync(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            // API INTEGRATION POINT: DELETE /api/guide/destinations/{id}.
            await Task.Delay(300);
            Destinations.Remove(destination);
            _notificationManager.ShowNotification("Destination deleted", destination.Name, true);
        }

        [RelayCommand]
        private async Task DeleteHotelAsync(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            // API INTEGRATION POINT: DELETE /api/guide/hotels/{id}.
            await Task.Delay(300);
            Hotels.Remove(hotel);
            _notificationManager.ShowNotification("Hotel deleted", hotel.Name, true);
        }

        [RelayCommand]
        private void ClearForm()
        {
            Name = string.Empty;
            Province = string.Empty;
            Description = string.Empty;
            Address = string.Empty;
            ImageUrl = string.Empty;
            Price = 0;
            Rating = 0;
        }

        private void SeedContent()
        {
            Destinations.Add(new DestinationModel
            {
                Id = 1,
                Name = "Marble Mountains",
                Province = "Da Nang",
                Description = "Half-day cultural route with caves, viewpoints, and local craft stops.",
                Rating = 4.6,
                GuidePriceFrom = 420000
            });

            Destinations.Add(new DestinationModel
            {
                Id = 2,
                Name = "Hoi An Lantern Walk",
                Province = "Quang Nam",
                Description = "Evening walking tour with food stops and heritage highlights.",
                Rating = 4.8,
                GuidePriceFrom = 520000
            });

            Hotels.Add(new HotelModel
            {
                Id = 1,
                Name = "Guide Partner Hotel",
                Province = "Da Nang",
                Address = "Bach Dang",
                PricePerNight = 720000,
                Rating = 4.3,
                IsApproved = true
            });

            Hotels.Add(new HotelModel
            {
                Id = 2,
                Name = "New Riverside Stay",
                Province = "Quang Nam",
                Address = "Nguyen Phuc Chu",
                PricePerNight = 680000,
                Rating = 4.2,
                IsApproved = false
            });
        }
    }
}
