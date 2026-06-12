using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.ImageManagement;
using TravelApp.Services.Logging;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class HotelManagementViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly ImageUploadService _imageUploadService;

        [ObservableProperty]
        private ObservableCollection<HotelModel> _hotels;

        [ObservableProperty]
        private ObservableCollection<DestinationModel> _destinations;

        [ObservableProperty]
        private bool _isEditorOpen;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private int _hotelId;

        [ObservableProperty]
        private int _destinationId;

        [ObservableProperty]
        private string _formTitle;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _address;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _pricePerNight;

        [ObservableProperty]
        private int _rating;

        [ObservableProperty]
        private string _imageUrl;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _successMessage;

        public HotelManagementViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            ImageUploadService imageUploadService)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _imageUploadService = imageUploadService;
            Hotels = new ObservableCollection<HotelModel>();
            Destinations = new ObservableCollection<DestinationModel>();
            _ = LoadHotelsAsync();
        }

        [RelayCommand]
        private async Task SelectImageAsync()
        {
            ClearMessages();
            var selectedFile = _imageUploadService.SelectImageFile();
            if (string.IsNullOrWhiteSpace(selectedFile))
            {
                return;
            }

            IsBusy = true;
            try
            {
                ImageUrl = await _imageUploadService.UploadImageAsync(
                    selectedFile,
                    "Hotel");
                SuccessMessage = "Đã chọn ảnh hợp lệ.";
            }
            catch (ImageUploadException ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadHotelsAsync()
        {
            ClearMessages();
            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            IsBusy = true;
            try
            {
                var destinationsTask =
                    _contentRepository.GetApprovedDestinationsAsync();
                var hotelsTask = _contentRepository.GetHotelsByGuideAsync(
                    guide.Id);
                await Task.WhenAll(destinationsTask, hotelsTask);

                Destinations = new ObservableCollection<DestinationModel>(
                    await destinationsTask);
                Hotels = new ObservableCollection<HotelModel>(
                    await hotelsTask);
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load guide hotels",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CreateHotel()
        {
            ClearMessages();
            if (!Destinations.Any())
            {
                ErrorMessage =
                    "Cần có ít nhất một điểm đến đã được Admin duyệt trước khi thêm khách sạn.";
                return;
            }

            IsEditing = false;
            IsEditorOpen = true;
            HotelId = 0;
            DestinationId = Destinations[0].Id;
            FormTitle = "Thêm khách sạn";
            Name = string.Empty;
            Address = string.Empty;
            Description = string.Empty;
            PricePerNight = 0;
            Rating = 0;
            ImageUrl = string.Empty;
        }

        [RelayCommand]
        private void EditHotel(HotelModel hotel)
        {
            if (hotel == null)
            {
                return;
            }

            ClearMessages();
            IsEditing = true;
            IsEditorOpen = true;
            HotelId = hotel.Id;
            DestinationId = hotel.DestinationId;
            FormTitle = "Chỉnh sửa khách sạn";
            Name = hotel.Name;
            Address = hotel.Address;
            Description = hotel.Description;
            PricePerNight = hotel.PricePerNight;
            Rating = hotel.Rating;
            ImageUrl = hotel.ImageUrl;
        }

        [RelayCommand]
        private async Task SaveHotelAsync()
        {
            ClearMessages();
            if (!ValidateHotel())
            {
                return;
            }

            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            var hotel = new HotelModel
            {
                Id = HotelId,
                DestinationId = DestinationId,
                Name = Name.Trim(),
                Address = Address.Trim(),
                Description = Description?.Trim(),
                PricePerNight = PricePerNight,
                Rating = Rating,
                ImageUrl = ImageUrl?.Trim(),
                CreatedByGuideId = guide.Id,
                ApprovalStatus = ContentApprovalStatus.Pending
            };

            IsBusy = true;
            try
            {
                var saved = IsEditing
                    ? await _contentRepository.UpdateHotelByGuideAsync(
                        hotel,
                        guide.Id)
                    : await _contentRepository.CreateHotelAsync(hotel);
                if (!saved)
                {
                    ErrorMessage =
                        "Không thể lưu khách sạn hoặc bạn không có quyền sửa nội dung này.";
                    return;
                }

                var wasEditing = IsEditing;
                IsEditorOpen = false;
                await LoadHotelsAsync();
                SuccessMessage = wasEditing
                    ? "Đã cập nhật và gửi lại khách sạn để Admin duyệt."
                    : "Đã tạo khách sạn và gửi Admin duyệt.";
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Save guide hotel",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditorOpen = false;
            ClearMessages();
        }

        private bool ValidateHotel()
        {
            if (DestinationId <= 0 ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Address))
            {
                ErrorMessage =
                    "Điểm đến, tên và địa chỉ khách sạn là bắt buộc.";
                return false;
            }

            if (PricePerNight < 0 || Rating < 0 || Rating > 5)
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
