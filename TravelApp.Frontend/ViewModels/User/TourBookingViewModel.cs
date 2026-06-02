using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.User
{
    public partial class TourBookingViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        public ObservableCollection<DestinationModel> AvailableDestinations { get; }
        public ObservableCollection<UserModel> AvailableGuides { get; }
        public ObservableCollection<HotelModel> AvailableHotels { get; }

        [ObservableProperty] private DestinationModel _selectedDestination;
        [ObservableProperty] private UserModel _selectedGuide;
        [ObservableProperty] private HotelModel _selectedHotel;
        [ObservableProperty] private int _tripDurationDays = 1;

        public TourBookingViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            AvailableDestinations = new ObservableCollection<DestinationModel>
            {
                new DestinationModel { Id = 1, Name = "Da Nang Beach", Province = "Da Nang", Rating = 4.7, GuidePriceFrom = 450000 },
                new DestinationModel { Id = 2, Name = "Hoi An Ancient Town", Province = "Quang Nam", Rating = 4.8, GuidePriceFrom = 520000 }
            };

            AvailableGuides = new ObservableCollection<UserModel>
            {
                new UserModel { Email = "guide@travelapp.local", PhoneNumber = "0900000001", Province = "Da Nang", Role = "Guide" },
                new UserModel { Email = "localguide@travelapp.local", PhoneNumber = "0900000003", Province = "Quang Nam", Role = "Guide" }
            };

            AvailableHotels = new ObservableCollection<HotelModel>
            {
                new HotelModel { Id = 1, Name = "Seaside Hotel", Province = "Da Nang", PricePerNight = 780000, Rating = 4.4, IsApproved = true },
                new HotelModel { Id = 2, Name = "Lantern Stay", Province = "Quang Nam", PricePerNight = 650000, Rating = 4.6, IsApproved = true }
            };
        }

        [RelayCommand]
        private async Task SubmitBookingAsync()
        {
            if (SelectedDestination == null || SelectedGuide == null)
            {
                _notificationManager.ShowNotification("Booking incomplete", "Please select a destination and tour guide.", true);
                return;
            }

            if (TripDurationDays >= 2 && SelectedHotel == null)
            {
                _notificationManager.ShowNotification("Hotel required", "Trips of 2 days or longer require a hotel selection.", true);
                return;
            }

            // API INTEGRATION POINT:
            // POST /api/bookings with destinationId, guideId, hotelId, tripDurationDays, paymentStatus.
            // Backend should notify the guide when the selected slot matches guide availability.
            await Task.Delay(500);

            _notificationManager.ShowNotification("Booking sent", "The tour request was sent to the selected guide.", false);
        }

        [RelayCommand]
        private async Task CancelBookingAsync(string bookingId)
        {
            // API INTEGRATION POINT: DELETE or PATCH /api/bookings/{bookingId}/cancel.
            await Task.Delay(500);
            _notificationManager.ShowNotification("Booking canceled", $"Booking {bookingId} was canceled.", true);
        }
    }
}
