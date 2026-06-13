using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.ImageManagement;
using TravelApp.Services.Logging;
using TravelApp.Services.Contracts;

namespace TravelApp.ViewModels.Admin
{
    public partial class ContentManagementViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly ImageUploadService _imageUploadService;
        private readonly IBookingService _bookingService;

        [ObservableProperty] private ObservableCollection<DestinationModel> _destinations;
        [ObservableProperty] private ObservableCollection<HotelModel> _hotels;
        [ObservableProperty] private ObservableCollection<BookingModel> _bookings;
        [ObservableProperty] private BookingModel _selectedBooking;
        [ObservableProperty] private BookingStatus _selectedBookingStatus;
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

        public IReadOnlyList<BookingStatus> BookingStatuses { get; } =
            (BookingStatus[])Enum.GetValues(typeof(BookingStatus));

        public ContentManagementViewModel(
            ITravelContentRepository contentRepository,
            ImageUploadService imageUploadService,
            IBookingService bookingService)
        {
            _contentRepository = contentRepository;
            _imageUploadService = imageUploadService;
            _bookingService = bookingService;
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
                SetLoggedError(
                    "Save destination",
                    ex,
                    "Không thể lưu điểm đến",
                    "DestinationId=" + destination.Id);
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
                SetLoggedError(
                    "Save hotel",
                    ex,
                    "Không thể lưu khách sạn",
                    "HotelId=" + hotel.Id);
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

        partial void OnSelectedBookingChanged(BookingModel value)
        {
            if (value != null)
            {
                SelectedBookingStatus = value.Status;
            }
        }

        [RelayCommand]
        private async Task SaveBookingStatusAsync()
        {
            ClearMessages();
            if (SelectedBooking == null)
            {
                ErrorMessage = "Hãy chọn một booking cần quản lý.";
                return;
            }

            IsLoading = true;
            try
            {
                var result = await _bookingService.UpdateByAdminAsync(
                    SelectedBooking.Id,
                    SelectedBookingStatus);
                if (!result.Succeeded)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                var bookingCode = SelectedBooking.BookingId;
                await RefreshDataAsync();
                SelectedBooking = null;
                SuccessMessage =
                    "Đã cập nhật booking " + bookingCode + " thành công.";
            }
            catch (Exception ex)
            {
                SetLoggedError(
                    "Update admin booking status",
                    ex,
                    "Không thể cập nhật trạng thái booking",
                    "BookingId=" + SelectedBooking?.Id);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private Task ApproveGuideCancellationAsync()
        {
            return ResolveGuideCancellationAsync(true);
        }

        [RelayCommand]
        private Task RejectGuideCancellationAsync()
        {
            return ResolveGuideCancellationAsync(false);
        }

        private async Task ResolveGuideCancellationAsync(bool approve)
        {
            ClearMessages();
            if (SelectedBooking == null ||
                !SelectedBooking.HasPendingGuideCancellation)
            {
                ErrorMessage =
                    "Hãy chọn booking có yêu cầu hủy đang chờ xử lý.";
                return;
            }

            IsLoading = true;
            try
            {
                var bookingCode = SelectedBooking.BookingId;
                var result =
                    await _bookingService.ResolveGuideCancellationRequestAsync(
                        SelectedBooking.Id,
                        approve);
                if (!result.Succeeded)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                await RefreshDataAsync();
                SelectedBooking = null;
                SuccessMessage = approve
                    ? "Đã duyệt hủy booking " + bookingCode + "."
                    : "Đã từ chối yêu cầu hủy booking " + bookingCode + ".";
            }
            catch (Exception ex)
            {
                SetLoggedError(
                    "Resolve guide cancellation request",
                    ex,
                    "Không thể xử lý yêu cầu hủy",
                    "BookingId=" + SelectedBooking?.Id +
                    "; Approve=" + approve);
            }
            finally
            {
                IsLoading = false;
            }
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
                SetLoggedError(
                    "Load admin content",
                    ex,
                    "Không thể tải dữ liệu nội dung");
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
                SetLoggedError(
                    "Update destination approval",
                    ex,
                    "Không thể cập nhật trạng thái duyệt điểm đến",
                    "DestinationId=" + destination.Id +
                    "; Status=" + status);
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
                SetLoggedError(
                    "Update hotel approval",
                    ex,
                    "Không thể cập nhật trạng thái duyệt khách sạn",
                    "HotelId=" + hotel.Id +
                    "; Status=" + status);
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

        [RelayCommand]
        private async Task SelectDestinationImageAsync()
        {
            await SelectImageAsync(
                "Destination",
                value => DestinationImageUrl = value);
        }

        [RelayCommand]
        private async Task SelectHotelImageAsync()
        {
            await SelectImageAsync(
                "Hotel",
                value => HotelImageUrl = value);
        }

        private async Task SelectImageAsync(
            string targetType,
            Action<string> applyImage)
        {
            ClearMessages();
            var selectedFile = _imageUploadService.SelectImageFile();
            if (string.IsNullOrWhiteSpace(selectedFile))
            {
                return;
            }

            IsLoading = true;
            try
            {
                applyImage(await _imageUploadService.UploadImageAsync(
                    selectedFile,
                    targetType));
                SuccessMessage = "Đã chọn ảnh hợp lệ.";
            }
            catch (ImageUploadException ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SetLoggedError(
            string operation,
            Exception exception,
            string message,
            string context = null)
        {
            var errorId = LoggerService.LogException(
                operation,
                exception,
                context);
            ErrorMessage = message + ". Mã lỗi: " + errorId;
        }
    }
}
