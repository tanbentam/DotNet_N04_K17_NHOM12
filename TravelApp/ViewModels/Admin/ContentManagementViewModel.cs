using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;

namespace TravelApp.ViewModels.Admin
{
    public partial class ContentManagementViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;

        [ObservableProperty]
        private ObservableCollection<DestinationModel> _destinations;

        [ObservableProperty]
        private ObservableCollection<HotelModel> _hotels;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _bookings;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public ContentManagementViewModel(ITravelContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
            Destinations = new ObservableCollection<DestinationModel>();
            Hotels = new ObservableCollection<HotelModel>();
            Bookings = new ObservableCollection<BookingModel>();

            LoadContentDataAsync();
        }

        private async void LoadContentDataAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var destinationsTask = _contentRepository.GetDestinationsAsync();
                var hotelsTask = _contentRepository.GetHotelsAsync();
                var bookingsTask = _contentRepository.GetBookingsAsync();

                await Task.WhenAll(destinationsTask, hotelsTask, bookingsTask);

                Destinations = new ObservableCollection<DestinationModel>(
                    await destinationsTask);
                Hotels = new ObservableCollection<HotelModel>(
                    await hotelsTask);
                Bookings = new ObservableCollection<BookingModel>(
                    await bookingsTask);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
