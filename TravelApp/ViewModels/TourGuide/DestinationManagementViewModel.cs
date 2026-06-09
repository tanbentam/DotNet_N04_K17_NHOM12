using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.ImageManagement;
using TravelApp.Services.Logging;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class DestinationManagementViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly ImageUploadService _imageUploadService;

        [ObservableProperty]
        private ObservableCollection<DestinationModel> _destinations;

        [ObservableProperty]
        private bool _isEditorOpen;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private int _destinationId;

        [ObservableProperty]
        private string _formTitle;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _country;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _imageUrl;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _successMessage;

        public DestinationManagementViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            ImageUploadService imageUploadService)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _imageUploadService = imageUploadService;
            Destinations = new ObservableCollection<DestinationModel>();
            _ = LoadDestinationsAsync();
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
                    "Destination");
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
        private async Task LoadDestinationsAsync()
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
                Destinations = new ObservableCollection<DestinationModel>(
                    await _contentRepository.GetDestinationsByGuideAsync(
                        guide.Id));
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load guide destinations",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CreateDestination()
        {
            ClearMessages();
            IsEditing = false;
            IsEditorOpen = true;
            DestinationId = 0;
            FormTitle = "Thêm điểm đến";
            Name = string.Empty;
            Country = string.Empty;
            Description = string.Empty;
            ImageUrl = string.Empty;
        }

        [RelayCommand]
        private void EditDestination(DestinationModel destination)
        {
            if (destination == null)
            {
                return;
            }

            ClearMessages();
            IsEditing = true;
            IsEditorOpen = true;
            DestinationId = destination.Id;
            FormTitle = "Chỉnh sửa điểm đến";
            Name = destination.Name;
            Country = destination.Country;
            Description = destination.Description;
            ImageUrl = destination.ImageUrl;
        }

        [RelayCommand]
        private async Task SaveDestinationAsync()
        {
            ClearMessages();
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Country))
            {
                ErrorMessage = "Tên và quốc gia là bắt buộc.";
                return;
            }

            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            var wasEditing = IsEditing;
            var destination = new DestinationModel
            {
                Id = DestinationId,
                Name = Name.Trim(),
                Country = Country.Trim(),
                Description = Description?.Trim(),
                ImageUrl = ImageUrl?.Trim(),
                AverageRating = 0,
                CreatedByGuideId = guide.Id,
                ApprovalStatus = ContentApprovalStatus.Pending
            };

            IsBusy = true;
            try
            {
                var saved = wasEditing
                    ? await _contentRepository.UpdateDestinationByGuideAsync(
                        destination,
                        guide.Id)
                    : await _contentRepository.CreateDestinationAsync(
                        destination);
                if (!saved)
                {
                    ErrorMessage =
                        "Không thể lưu điểm đến hoặc bạn không có quyền sửa nội dung này.";
                    return;
                }

                IsEditorOpen = false;
                await LoadDestinationsAsync();
                SuccessMessage = wasEditing
                    ? "Đã cập nhật và gửi lại điểm đến để Admin duyệt."
                    : "Đã tạo điểm đến và gửi Admin duyệt.";
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Save guide destination",
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

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}
