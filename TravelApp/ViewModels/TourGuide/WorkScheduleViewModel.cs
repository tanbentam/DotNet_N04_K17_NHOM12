using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class WorkScheduleViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly IBookingService _bookingService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _workSchedule;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private string _successMessage;

        [ObservableProperty]
        private BookingModel _selectedBooking;

        [ObservableProperty]
        private string _cancellationReason;

        public WorkScheduleViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            IBookingService bookingService,
            NotificationManager notificationManager)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _bookingService = bookingService;
            _notificationManager = notificationManager;
            WorkSchedule = new ObservableCollection<BookingModel>();
            _ = LoadWorkScheduleAsync();
        }

        [RelayCommand]
        public async Task LoadWorkScheduleAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            IsBusy = true;
            try
            {
                WorkSchedule = new ObservableCollection<BookingModel>(
                    await _contentRepository.GetWorkScheduleByGuideAsync(
                        guide.Id));
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load guide work schedule",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RequestCancellationAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide ||
                SelectedBooking == null)
            {
                ErrorMessage = "Hãy chọn tour cần gửi yêu cầu hủy.";
                return;
            }

            IsBusy = true;
            try
            {
                var result =
                    await _bookingService.RequestCancellationByGuideAsync(
                        SelectedBooking.Id,
                        guide.Id,
                        CancellationReason);
                if (!result.Succeeded)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                var bookingCode = SelectedBooking.BookingId;
                CancellationReason = string.Empty;
                await LoadWorkScheduleAsync();
                SelectedBooking = null;
                SuccessMessage =
                    "Đã gửi yêu cầu hủy booking " + bookingCode + ".";
                _notificationManager.ShowNotification(
                    "Đã gửi yêu cầu",
                    "Admin sẽ xem xét yêu cầu hủy booking " +
                        bookingCode + ".",
                    false);
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Request guide booking cancellation",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
