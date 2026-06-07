using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;

namespace TravelApp.ViewModels.Admin
{
    public partial class ContentManagementViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;

        [ObservableProperty] private ObservableCollection<DestinationModel> _destinations;
        [ObservableProperty] private ObservableCollection<HotelModel> _hotels;
        [ObservableProperty] private ObservableCollection<BookingModel> _bookings;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _errorMessage;
        [ObservableProperty] private string _successMessage;

        [ObservableProperty] private bool _isDestinationEditorOpen;
        [ObservableProperty] private bool _isEditingDestination;
        [ObservableProperty] private int _destinationId;
        [ObservableProperty] private string _destinationFormTitle;
        [ObservableProperty] private string _destinationName;
        [ObservableProperty] private string _destinationCountry;
        [ObservableProperty] private string _destinationDescription;
        [ObservableProperty] private string _destinationImageUrl;
        [ObservableProperty] private decimal _destinationRating;

        [ObservableProperty] private bool _isHotelEditorOpen;
        [ObservableProperty] private bool _isEditingHotel;
        [ObservableProperty] private int _hotelId;
        [ObservableProperty] private string _hotelFormTitle;
        [ObservableProperty] private int _hotelDestinationId;
        [ObservableProperty] private string _hotelName;
        [ObservableProperty] private string _hotelAddress;
        [ObservableProperty] private string _hotelDescription;
        [ObservableProperty] private decimal _hotelPricePerNight;
        [ObservableProperty] private int _hotelRating;
        [ObservableProperty] private string _hotelImageUrl;

        public ContentManagementViewModel(
            ITravelContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
            Destinations = new ObservableCollection<DestinationModel>();
            Hotels = new ObservableCollection<HotelModel>();
            Bookings = new ObservableCollection<BookingModel>();
            _ = LoadContentDataAsync();
        }

        [RelayCommand]
        private async Task LoadContentDataAsync()
        {
            ClearMessages();
            await RefreshDataAsync();
        }

        [RelayCommand]
        private void CreateDestination()
        {
            ClearMessages();
            IsEditingDestination = false;
            IsDestinationEditorOpen = true;
            DestinationId = 0;
            DestinationFormTitle = "Create Destination";
            DestinationName = string.Empty;
            DestinationCountry = string.Empty;
            DestinationDescription = string.Empty;
            DestinationImageUrl = string.Empty;
            DestinationRating = 0;
        }

        [RelayCommand]
        private void EditDestination(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            ClearMessages();
            IsEditingDestination = true;
            IsDestinationEditorOpen = true;
            DestinationId = destination.Id;
            DestinationFormTitle = "Edit Destination";
            DestinationName = destination.Name;
            DestinationCountry = destination.Country;
            DestinationDescription = destination.Description;
            DestinationImageUrl = destination.ImageUrl;
            DestinationRating = destination.AverageRating;
        }

        [RelayCommand]
        private async Task SaveDestinationAsync()
        {
            ClearMessages();
            if (!ValidateDestination())
            {
                return;
            }

            var wasEditing = IsEditingDestination;
            var destination = new DestinationModel
            {
                Id = DestinationId,
                Name = DestinationName.Trim(),
                Country = DestinationCountry.Trim(),
                Description = DestinationDescription?.Trim(),
                ImageUrl = DestinationImageUrl?.Trim(),
                AverageRating = DestinationRating,
                ApprovalStatus = ContentApprovalStatus.Approved
            };

            IsLoading = true;
            try
            {
                var saved = wasEditing
                    ? await _contentRepository.UpdateDestinationAsync(destination)
                    : await _contentRepository.CreateDestinationAsync(destination);
                if (!saved)
                {
                    ErrorMessage = "Không thể lưu điểm đến.";
                    return;
                }

                IsDestinationEditorOpen = false;
                await RefreshDataAsync();
                SuccessMessage = wasEditing
                    ? "Cập nhật điểm đến thành công."
                    : "Tạo điểm đến thành công.";
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

        [RelayCommand]
        private async Task DeleteDestinationAsync(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            ClearMessages();
            if (!await _contentRepository.DeleteDestinationAsync(destination.Id))
            {
                ErrorMessage =
                    "Không thể xóa điểm đến đang có khách sạn hoặc booking.";
                return;
            }

            await RefreshDataAsync();
            SuccessMessage = "Xóa điểm đến thành công.";
        }

        [RelayCommand]
        private Task ApproveDestinationAsync(DestinationModel destination)
        {
            return SetDestinationApprovalAsync(
                destination,
                ContentApprovalStatus.Approved);
        }

        [RelayCommand]
        private Task RejectDestinationAsync(DestinationModel destination)
        {
            return SetDestinationApprovalAsync(
                destination,
                ContentApprovalStatus.Rejected);
        }

        [RelayCommand]
        private void CancelDestinationEdit()
        {
            IsDestinationEditorOpen = false;
            ClearMessages();
        }

        [RelayCommand]
        private void CreateHotel()
        {
            ClearMessages();
            if (Destinations.Count == 0)
            {
                ErrorMessage = "Hãy tạo ít nhất một điểm đến trước.";
                return;
            }

            IsEditingHotel = false;
            IsHotelEditorOpen = true;
            HotelId = 0;
            HotelFormTitle = "Create Hotel";
            HotelDestinationId = Destinations[0].Id;
            HotelName = string.Empty;
            HotelAddress = string.Empty;
            HotelDescription = string.Empty;
            HotelPricePerNight = 0;
            HotelRating = 0;
            HotelImageUrl = string.Empty;
        }

        [RelayCommand]
        private void EditHotel(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            ClearMessages();
            IsEditingHotel = true;
            IsHotelEditorOpen = true;
            HotelId = hotel.Id;
            HotelFormTitle = "Edit Hotel";
            HotelDestinationId = hotel.DestinationId;
            HotelName = hotel.Name;
            HotelAddress = hotel.Address;
            HotelDescription = hotel.Description;
            HotelPricePerNight = hotel.PricePerNight;
            HotelRating = hotel.Rating;
            HotelImageUrl = hotel.ImageUrl;
        }

        [RelayCommand]
        private async Task SaveHotelAsync()
        {
            ClearMessages();
            if (!ValidateHotel())
            {
                return;
            }

            var wasEditing = IsEditingHotel;
            var hotel = new HotelModel
            {
                Id = HotelId,
                DestinationId = HotelDestinationId,
                Name = HotelName.Trim(),
                Address = HotelAddress.Trim(),
                Description = HotelDescription?.Trim(),
                PricePerNight = HotelPricePerNight,
                Rating = HotelRating,
                ImageUrl = HotelImageUrl?.Trim(),
                ApprovalStatus = ContentApprovalStatus.Approved
            };

            IsLoading = true;
            try
            {
                var saved = wasEditing
                    ? await _contentRepository.UpdateHotelAsync(hotel)
                    : await _contentRepository.CreateHotelAsync(hotel);
                if (!saved)
                {
                    ErrorMessage = "Không thể lưu khách sạn.";
                    return;
                }

                IsHotelEditorOpen = false;
                await RefreshDataAsync();
                SuccessMessage = wasEditing
                    ? "Cập nhật khách sạn thành công."
                    : "Tạo khách sạn thành công.";
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

        [RelayCommand]
        private async Task DeleteHotelAsync(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            ClearMessages();
            if (!await _contentRepository.DeleteHotelAsync(hotel.Id))
            {
                ErrorMessage = "Không thể xóa khách sạn đang có booking.";
                return;
            }

            await RefreshDataAsync();
            SuccessMessage = "Xóa khách sạn thành công.";
        }

        [RelayCommand]
        private Task ApproveHotelAsync(HotelModel hotel)
        {
            return SetHotelApprovalAsync(hotel, ContentApprovalStatus.Approved);
        }

        [RelayCommand]
        private Task RejectHotelAsync(HotelModel hotel)
        {
            return SetHotelApprovalAsync(hotel, ContentApprovalStatus.Rejected);
        }

        [RelayCommand]
        private void CancelHotelEdit()
        {
            IsHotelEditorOpen = false;
            ClearMessages();
        }

        private async Task RefreshDataAsync()
        {
            IsLoading = true;
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
                ErrorMessage = "Không thể tải dữ liệu nội dung: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SetDestinationApprovalAsync(
            DestinationModel destination,
            ContentApprovalStatus status)
        {
            if (destination == null)
            {
                return;
            }

            ClearMessages();
            IsLoading = true;
            try
            {
                var updated = await _contentRepository
                    .UpdateDestinationApprovalAsync(destination.Id, status);
                if (!updated)
                {
                    ErrorMessage = "Không thể cập nhật trạng thái duyệt.";
                    return;
                }

                await RefreshDataAsync();
                SuccessMessage = status == ContentApprovalStatus.Approved
                    ? "Đã duyệt điểm đến."
                    : "Đã từ chối điểm đến.";
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

        private async Task SetHotelApprovalAsync(
            HotelModel hotel,
            ContentApprovalStatus status)
        {
            if (hotel == null)
            {
                return;
            }

            ClearMessages();
            IsLoading = true;
            try
            {
                var updated = await _contentRepository
                    .UpdateHotelApprovalAsync(hotel.Id, status);
                if (!updated)
                {
                    ErrorMessage = "Không thể cập nhật trạng thái duyệt.";
                    return;
                }

                await RefreshDataAsync();
                SuccessMessage = status == ContentApprovalStatus.Approved
                    ? "Đã duyệt khách sạn."
                    : "Đã từ chối khách sạn.";
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

        private bool ValidateDestination()
        {
            if (string.IsNullOrWhiteSpace(DestinationName) ||
                string.IsNullOrWhiteSpace(DestinationCountry))
            {
                ErrorMessage = "Tên và quốc gia của điểm đến là bắt buộc.";
                return false;
            }

            if (DestinationRating < 0 || DestinationRating > 9.99m)
            {
                ErrorMessage = "Đánh giá điểm đến phải từ 0 đến 9.99.";
                return false;
            }

            return true;
        }

        private bool ValidateHotel()
        {
            if (HotelDestinationId <= 0 ||
                string.IsNullOrWhiteSpace(HotelName) ||
                string.IsNullOrWhiteSpace(HotelAddress))
            {
                ErrorMessage =
                    "Điểm đến, tên và địa chỉ khách sạn là bắt buộc.";
                return false;
            }

            if (HotelPricePerNight < 0 || HotelRating < 0 || HotelRating > 5)
            {
                ErrorMessage =
                    "Giá phải không âm và đánh giá khách sạn từ 0 đến 5.";
                return false;
            }

            return true;
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}
