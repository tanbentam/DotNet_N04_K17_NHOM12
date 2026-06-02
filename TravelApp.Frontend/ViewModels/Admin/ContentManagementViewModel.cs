using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Models.Enums;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class ContentManagementViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private ObservableCollection<DestinationModel> _destinations;
        [ObservableProperty] private ObservableCollection<HotelModel> _hotels;
        [ObservableProperty] private ObservableCollection<BookingModel> _bookings;
        [ObservableProperty] private ObservableCollection<GuideAvailabilityModel> _guideSchedules;

        public ContentManagementViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            Destinations = new ObservableCollection<DestinationModel>();
            Hotels = new ObservableCollection<HotelModel>();
            Bookings = new ObservableCollection<BookingModel>();
            GuideSchedules = new ObservableCollection<GuideAvailabilityModel>();
            LoadContentData();
        }

        private void LoadContentData()
        {
            // API INTEGRATION POINT:
            // Replace sample data with:
            // GET /api/admin/destinations
            // GET /api/admin/hotels
            // GET /api/admin/bookings
            // GET /api/admin/guides/availability
            Destinations.Add(new DestinationModel { Id = 1, Name = "Da Nang Beach", Province = "Da Nang", Rating = 4.7, GuidePriceFrom = 450000 });
            Destinations.Add(new DestinationModel { Id = 2, Name = "Hoi An Ancient Town", Province = "Quang Nam", Rating = 4.8, GuidePriceFrom = 520000 });

            Hotels.Add(new HotelModel { Id = 1, Name = "Seaside Hotel", Province = "Da Nang", Address = "Vo Nguyen Giap", PricePerNight = 780000, Rating = 4.4, IsApproved = true });
            Hotels.Add(new HotelModel { Id = 2, Name = "Pending Boutique Stay", Province = "Quang Nam", Address = "Tran Phu", PricePerNight = 690000, Rating = 4.2, IsApproved = false });

            Bookings.Add(new BookingModel { BookingId = "BK001", UserName = "Nguyen Van A", DestinationName = "Da Nang Beach", HotelName = "Seaside Hotel", Status = BookingStatus.Pending });
            Bookings.Add(new BookingModel { BookingId = "BK002", UserName = "Tran Thi B", DestinationName = "Hoi An Ancient Town", HotelName = "Pending Boutique Stay", Status = BookingStatus.Paid });

            GuideSchedules.Add(new GuideAvailabilityModel { GuideEmail = "guide1@travel.com", DayOfWeek = "Monday", TimeSlot = "08:00 - 12:00", IsAvailable = true });
            GuideSchedules.Add(new GuideAvailabilityModel { GuideEmail = "guide2@travel.com", DayOfWeek = "Saturday", TimeSlot = "13:00 - 18:00", IsAvailable = true });
        }

        [RelayCommand]
        private async Task ApproveHotelAsync(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            // API INTEGRATION POINT: PATCH /api/admin/hotels/{id}/approve.
            await Task.Delay(300);

            hotel.IsApproved = true;
            OnPropertyChanged(nameof(Hotels));
            _notificationManager.ShowNotification("Hotel approved", $"{hotel.Name} is now public.", false);
        }

        [RelayCommand]
        private async Task DeleteDestinationAsync(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            // API INTEGRATION POINT: DELETE /api/admin/destinations/{id}.
            await Task.Delay(300);

            Destinations.Remove(destination);
            _notificationManager.ShowNotification("Destination deleted", destination.Name, true);
        }

        [RelayCommand]
        private async Task CancelBookingAsync(BookingModel booking)
        {
            if (booking == null)
            {
                return;
            }

            // API INTEGRATION POINT: PATCH /api/admin/bookings/{id}/cancel.
            await Task.Delay(300);

            booking.Status = BookingStatus.Cancelled;
            OnPropertyChanged(nameof(Bookings));
            _notificationManager.ShowNotification("Booking canceled", booking.BookingId, true);
        }
    }
}
