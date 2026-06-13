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
using TravelApp.Utils;

namespace TravelApp.ViewModels.Admin
{
    public partial class AccountManagementViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        [ObservableProperty] private ObservableCollection<UserModel> _usersList;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _errorMessage;
        [ObservableProperty] private string _successMessage;
        [ObservableProperty] private int _userCount;
        [ObservableProperty] private bool _isEmpty;
        [ObservableProperty] private bool _hasUsers;

        [ObservableProperty] private bool _isEditorOpen;
        [ObservableProperty] private bool _isEditing;
        [ObservableProperty] private int _editingUserId;
        [ObservableProperty] private string _formTitle;
        [ObservableProperty] private string _formFullName;
        [ObservableProperty] private string _formEmail;
        [ObservableProperty] private string _formPhone;
        [ObservableProperty] private string _formPassword;
        [ObservableProperty] private RoleType _formRole;

        public AccountManagementViewModel(
            IUserRepository userRepository,
            IAuthService authService,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _userRepository = userRepository;
            _authService = authService;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            UsersList = new ObservableCollection<UserModel>();
            _ = LoadAccountsAsync();
        }

        [RelayCommand]
        private async Task LoadAccountsAsync()
        {
            IsLoading = true;
            ClearMessages();

            try
            {
                var users = await _userRepository.GetAllAsync();
                UsersList = new ObservableCollection<UserModel>(users);
                UpdateSummary();
            }
            catch (Exception ex)
            {
                UsersList.Clear();
                UpdateSummary();
                SetLoggedError(
                    "Load admin accounts",
                    ex,
                    "Không thể tải danh sách tài khoản");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CreateGuideAccount()
        {
            OpenCreateEditor(RoleType.TourGuide);
        }

        [RelayCommand]
        private void CreateUserAccount()
        {
            OpenCreateEditor(RoleType.User);
        }

        [RelayCommand]
        private void EditAccount(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            ClearMessages();
            if (user.Role == RoleType.Admin)
            {
                ErrorMessage = "Không thể chỉnh sửa tài khoản Admin tại màn hình này.";
                return;
            }

            IsEditing = true;
            IsEditorOpen = true;
            EditingUserId = user.Id;
            FormTitle = "Edit Account";
            FormFullName = user.FullName;
            FormEmail = user.Email;
            FormPhone = user.Phone;
            FormPassword = string.Empty;
            FormRole = user.Role;
        }

        [RelayCommand]
        private async Task SaveAccountAsync()
        {
            ClearMessages();

            if (!ValidateForm())
            {
                return;
            }

            IsLoading = true;
            var user = new UserModel
            {
                Id = EditingUserId,
                FullName = FormFullName.Trim(),
                Email = FormEmail.Trim().ToLowerInvariant(),
                Phone = FormPhone.Trim(),
                Role = FormRole
            };

            try
            {
                bool saved;
                if (IsEditing)
                {
                    var passwordHash = string.IsNullOrWhiteSpace(FormPassword)
                        ? null
                        : PasswordHelper.HashPassword(FormPassword);
                    saved = await _userRepository.UpdateAsync(user, passwordHash);
                }
                else
                {
                    saved = await _authService.RegisterAsync(user, FormPassword);
                }

                if (!saved)
                {
                    ErrorMessage =
                        "Không thể lưu tài khoản. Email hoặc số điện thoại có thể đã tồn tại.";
                    NotifyError(ErrorMessage);
                    return;
                }

                var successMessage = IsEditing
                    ? "Cập nhật tài khoản thành công."
                    : "Tạo tài khoản thành công.";
                CloseEditor();
                await LoadAccountsAsync();
                SuccessMessage = successMessage;
                NotifySuccess(successMessage);
            }
            catch (Exception ex)
            {
                SetLoggedError(
                    "Save admin account",
                    ex,
                    "Không thể lưu tài khoản",
                    "UserId=" + user.Id + "; Role=" + user.Role);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            CloseEditor();
            ClearMessages();
        }

        [RelayCommand]
        private async Task DeleteAccountAsync(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            ClearMessages();

            if (_sessionService.CurrentUser?.Id == user.Id)
            {
                ErrorMessage = "Không thể xóa tài khoản đang đăng nhập.";
                NotifyError(ErrorMessage);
                return;
            }

            if (user.Role == RoleType.Admin)
            {
                ErrorMessage = "Không thể xóa tài khoản Admin tại màn hình này.";
                NotifyError(ErrorMessage);
                return;
            }

            try
            {
                if (!await _userRepository.DeleteAsync(user.Id))
                {
                    ErrorMessage =
                        "Không thể xóa tài khoản. Tài khoản có thể đang được sử dụng.";
                    NotifyError(ErrorMessage);
                    return;
                }

                UsersList.Remove(user);
                UpdateSummary();
                SuccessMessage = "Xóa tài khoản thành công.";
                NotifySuccess(SuccessMessage);
            }
            catch (Exception ex)
            {
                SetLoggedError(
                    "Delete admin account",
                    ex,
                    "Không thể xóa tài khoản",
                    "UserId=" + user.Id);
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
            NotifyError(ErrorMessage);
        }

        private void OpenCreateEditor(RoleType role)
        {
            ClearMessages();
            IsEditing = false;
            IsEditorOpen = true;
            EditingUserId = 0;
            FormTitle = role == RoleType.TourGuide
                ? "Create Guide Account"
                : "Create User Account";
            FormFullName = string.Empty;
            FormEmail = string.Empty;
            FormPhone = string.Empty;
            FormPassword = string.Empty;
            FormRole = role;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(FormFullName))
            {
                ErrorMessage = "Vui lòng nhập họ tên.";
                return false;
            }

            if (!ValidationHelper.IsValidEmail(FormEmail))
            {
                ErrorMessage = "Email không hợp lệ.";
                return false;
            }

            if (!ValidationHelper.IsValidPhoneNumber(FormPhone))
            {
                ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.";
                return false;
            }

            if (!IsEditing && string.IsNullOrWhiteSpace(FormPassword))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu.";
                return false;
            }

            if (FormRole != RoleType.User && FormRole != RoleType.TourGuide)
            {
                ErrorMessage = "Admin chỉ có thể quản lý tài khoản User hoặc TourGuide.";
                return false;
            }

            return true;
        }

        private void CloseEditor()
        {
            IsEditorOpen = false;
            EditingUserId = 0;
            FormPassword = string.Empty;
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private void UpdateSummary()
        {
            UserCount = UsersList.Count;
            IsEmpty = UserCount == 0;
            HasUsers = UserCount > 0;
        }

        private void NotifySuccess(string message)
        {
            _notificationManager.ShowNotification(
                "Account Management",
                message);
        }

        private void NotifyError(string message)
        {
            _notificationManager.ShowNotification(
                "Account Management",
                message,
                true);
        }
    }
}
