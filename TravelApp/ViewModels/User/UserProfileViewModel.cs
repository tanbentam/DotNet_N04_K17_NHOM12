using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.NotificationQueue;
using TravelApp.Utils;

namespace TravelApp.ViewModels.User
{
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private string _fullName;
        [ObservableProperty] private string _email;
        [ObservableProperty] private string _phone;
        [ObservableProperty] private string _profileMessage;
        [ObservableProperty] private bool _isBusy;

        public UserProfileViewModel(
            IUserRepository userRepository,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            LoadCurrentProfile();
        }

        [RelayCommand]
        private void LoadCurrentProfile()
        {
            ProfileMessage = string.Empty;
            var user = _sessionService.CurrentUser;
            if (user == null || user.Role != RoleType.User)
            {
                FullName = string.Empty;
                Email = string.Empty;
                Phone = string.Empty;
                ProfileMessage = "Phiên đăng nhập User không hợp lệ.";
                return;
            }

            FullName = user.FullName;
            Email = user.Email;
            Phone = user.Phone;
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            ProfileMessage = string.Empty;
            var currentUser = _sessionService.CurrentUser;
            if (currentUser == null || currentUser.Role != RoleType.User)
            {
                ProfileMessage = "Phiên đăng nhập User không hợp lệ.";
                return;
            }

            if (!ValidateProfile())
            {
                return;
            }

            var updatedProfile = new UserModel
            {
                Id = currentUser.Id,
                FullName = FullName.Trim(),
                Email = Email.Trim().ToLowerInvariant(),
                Phone = Phone.Trim(),
                Role = currentUser.Role
            };

            IsBusy = true;
            try
            {
                var saved = await _userRepository.UpdateAsync(
                    updatedProfile,
                    null);
                if (!saved)
                {
                    ProfileMessage =
                        "Không thể cập nhật. Email hoặc số điện thoại có thể đã tồn tại.";
                    return;
                }

                var refreshedUser =
                    await _userRepository.FindByIdentifierAsync(
                        updatedProfile.Email);
                if (refreshedUser == null ||
                    refreshedUser.Id != currentUser.Id)
                {
                    ProfileMessage =
                        "Đã lưu hồ sơ nhưng không thể làm mới phiên đăng nhập.";
                    return;
                }

                _sessionService.SignIn(refreshedUser);
                LoadCurrentProfile();
                ProfileMessage = "Cập nhật hồ sơ thành công.";
                _notificationManager.ShowNotification(
                    "Thành công",
                    "Thông tin cá nhân đã được cập nhật.",
                    false);
            }
            catch (Exception ex)
            {
                ProfileMessage = "Không thể cập nhật hồ sơ: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateProfile()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ProfileMessage = "Vui lòng nhập họ tên.";
                return false;
            }

            if (FullName.Trim().Length > 100)
            {
                ProfileMessage = "Họ tên không được vượt quá 100 ký tự.";
                return false;
            }

            if (!ValidationHelper.IsValidEmail(Email))
            {
                ProfileMessage = "Email không hợp lệ.";
                return false;
            }

            if (Email.Trim().Length > 254)
            {
                ProfileMessage = "Email không được vượt quá 254 ký tự.";
                return false;
            }

            if (!ValidationHelper.IsValidPhoneNumber(Phone))
            {
                ProfileMessage = "Số điện thoại phải có đúng 10 chữ số.";
                return false;
            }

            return true;
        }
    }
}
