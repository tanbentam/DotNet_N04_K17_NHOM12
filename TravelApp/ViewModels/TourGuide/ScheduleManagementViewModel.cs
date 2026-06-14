using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;
using TravelApp.Services.NotificationQueue;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class AvailableDay : ObservableObject
    {
        public int DayNumber { get; set; }
        public string DayOfWeek { get; set; }

        [ObservableProperty]
        private bool _isAvailable;

        [ObservableProperty]
        private string _timeSlot;
    }

    public partial class ScheduleManagementViewModel : ObservableObject
    {
        private readonly IGuideAvailabilityRepository _availabilityRepository;
        private readonly IUserSessionService _userSessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty]
        private ObservableCollection<AvailableDay> _weeklySchedule;

        public ScheduleManagementViewModel(
            IGuideAvailabilityRepository availabilityRepository,
            IUserSessionService userSessionService,
            NotificationManager notificationManager)
        {
            _availabilityRepository = availabilityRepository;
            _userSessionService = userSessionService;
            _notificationManager = notificationManager;
            WeeklySchedule = CreateDefaultSchedule();

            _ = LoadScheduleAsync();
        }

        [RelayCommand]
        private async Task SaveScheduleAsync()
        {
            var guide = _userSessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                _notificationManager.ShowNotification(
                    "Không thể lưu lịch",
                    "Bạn cần đăng nhập bằng tài khoản hướng dẫn viên.",
                    true);
                return;
            }

            try
            {
                var saved = await _availabilityRepository.SaveWeeklyScheduleAsync(
                    guide.Id,
                    WeeklySchedule.Select(day => new GuideAvailabilityModel
                    {
                        DayOfWeek = day.DayNumber,
                        DayName = day.DayOfWeek,
                        IsAvailable = day.IsAvailable,
                        TimeSlot = string.IsNullOrWhiteSpace(day.TimeSlot)
                            ? null
                            : day.TimeSlot.Trim()
                    }));

                if (!saved)
                {
                    LoggerService.LogWarning(
                        "Save guide schedule",
                        "Repository rejected schedule save.",
                        "GuideId=" + guide.Id);
                }

                _notificationManager.ShowNotification(
                    saved ? "Đã lưu lịch" : "Không thể lưu lịch",
                    saved
                ? "Lịch trống của bạn đã được cập nhật trong cơ sở dữ liệu."
                : "Vui lòng kiểm tra kết nối cơ sở dữ liệu và thử lại.",
                    !saved);
            }
            catch (Exception ex)
            {
                var errorId = LoggerService.LogException(
                    "Save guide schedule",
                    ex,
                    "GuideId=" + guide.Id);
                _notificationManager.ShowNotification(
                    "Không thể lưu lịch",
                    "Đã xảy ra lỗi. Mã lỗi: " + errorId,
                    true);
            }
        }

        private async Task LoadScheduleAsync()
        {
            var guide = _userSessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                return;
            }

            try
            {
                var savedSchedule =
                    await _availabilityRepository.GetByGuideIdAsync(guide.Id);
                ApplySavedSchedule(savedSchedule);
            }
            catch (Exception ex)
            {
                var errorId = LoggerService.LogException(
                    "Load guide schedule",
                    ex,
                    "GuideId=" + guide.Id);
                _notificationManager.ShowNotification(
                    "Không thể tải lịch",
                    "Đã xảy ra lỗi. Mã lỗi: " + errorId,
                    true);
            }
        }

        private void ApplySavedSchedule(
            IReadOnlyList<GuideAvailabilityModel> savedSchedule)
        {
            foreach (var savedDay in savedSchedule)
            {
                var day = WeeklySchedule.FirstOrDefault(
                    item => item.DayNumber == savedDay.DayOfWeek);
                if (day == null)
                {
                    continue;
                }

                day.IsAvailable = savedDay.IsAvailable;
                day.TimeSlot = savedDay.TimeSlot;
            }
        }

        private static ObservableCollection<AvailableDay> CreateDefaultSchedule()
        {
            return new ObservableCollection<AvailableDay>
            {
                new AvailableDay { DayNumber = 1, DayOfWeek = "Thứ 2", IsAvailable = false },
                new AvailableDay { DayNumber = 2, DayOfWeek = "Thứ 3", IsAvailable = false },
                new AvailableDay { DayNumber = 3, DayOfWeek = "Thứ 4", IsAvailable = false },
                new AvailableDay { DayNumber = 4, DayOfWeek = "Thứ 5", IsAvailable = false },
                new AvailableDay { DayNumber = 5, DayOfWeek = "Thứ 6", IsAvailable = false },
                new AvailableDay { DayNumber = 6, DayOfWeek = "Thứ 7", IsAvailable = false },
                new AvailableDay { DayNumber = 7, DayOfWeek = "Chủ Nhật", IsAvailable = false }
            };
        }
    }
}
