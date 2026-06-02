using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.Services.NotificationQueue;
using TravelApp.Frontend.Utils;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class AccountManagementViewModel : ObservableObject
    {
        private readonly NotificationManager _notificationManager;

        public ObservableCollection<string> AccountRoles { get; }

        [ObservableProperty] private ObservableCollection<UserModel> _usersList;
        [ObservableProperty] private UserModel _selectedUser;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _newEmail;
        [ObservableProperty] private string _newPassword;
        [ObservableProperty] private string _newPhoneNumber;
        [ObservableProperty] private string _newProvince;
        [ObservableProperty] private string _selectedRole;
        [ObservableProperty] private string _statusMessage;

        public AccountManagementViewModel(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
            AccountRoles = new ObservableCollection<string> { "Guide", "User" };
            SelectedRole = "Guide";
            UsersList = new ObservableCollection<UserModel>();
            LoadAccountsAsync();
        }

        private async void LoadAccountsAsync()
        {
            IsLoading = true;

            // API INTEGRATION POINT:
            // Replace this mock with GET /api/admin/accounts.
            await Task.Delay(500);

            UsersList.Add(new UserModel { Email = "guide1@travel.com", Role = "Guide", PhoneNumber = "0123456789", Province = "Da Nang" });
            UsersList.Add(new UserModel { Email = "user1@travel.com", Role = "User", PhoneNumber = "0987654321", Province = "Ho Chi Minh City" });
            UsersList.Add(new UserModel { Email = "guide2@travel.com", Role = "Guide", PhoneNumber = "0900000004", Province = "Quang Nam" });

            IsLoading = false;
        }

        [RelayCommand]
        private async Task CreateAccountAsync()
        {
            if (!ValidationHelper.IsValidEmail(NewEmail))
            {
                StatusMessage = "Enter a valid email address.";
                return;
            }

            if (!ValidationHelper.IsValidPhoneNumber(NewPhoneNumber))
            {
                StatusMessage = "Phone number must contain exactly 10 digits.";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(NewProvince) || string.IsNullOrWhiteSpace(SelectedRole))
            {
                StatusMessage = "Password, province/city, and role are required.";
                return;
            }

            // API INTEGRATION POINT:
            // POST /api/admin/accounts with email, password, phoneNumber, province, role.
            // Only Admin can create Guide accounts.
            await Task.Delay(300);

            var user = new UserModel
            {
                Email = NewEmail.Trim(),
                PhoneNumber = NewPhoneNumber.Trim(),
                Province = NewProvince.Trim(),
                Role = SelectedRole
            };

            UsersList.Add(user);
            ClearForm();
            StatusMessage = "Account created.";
            _notificationManager.ShowNotification("Account created", $"{user.Role} account was added.", false);
        }

        [RelayCommand]
        private void LoadSelectedUser(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            SelectedUser = user;
            NewEmail = user.Email;
            NewPhoneNumber = user.PhoneNumber;
            NewProvince = user.Province;
            SelectedRole = user.Role;
            NewPassword = string.Empty;
            StatusMessage = "Editing selected account.";
        }

        [RelayCommand]
        private async Task SaveSelectedAccountAsync()
        {
            if (SelectedUser == null)
            {
                StatusMessage = "Select an account to update.";
                return;
            }

            if (!ValidationHelper.IsValidEmail(NewEmail) || !ValidationHelper.IsValidPhoneNumber(NewPhoneNumber))
            {
                StatusMessage = "Email or phone number is invalid.";
                return;
            }

            // API INTEGRATION POINT:
            // PUT /api/admin/accounts/{id or email} with editable profile fields.
            await Task.Delay(300);

            SelectedUser.Email = NewEmail.Trim();
            SelectedUser.PhoneNumber = NewPhoneNumber.Trim();
            SelectedUser.Province = NewProvince.Trim();
            SelectedUser.Role = SelectedRole;

            OnPropertyChanged(nameof(UsersList));
            StatusMessage = "Account updated.";
            _notificationManager.ShowNotification("Account updated", $"{SelectedUser.Email} was saved.", false);
        }

        [RelayCommand]
        private async Task DeleteAccountAsync(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            // API INTEGRATION POINT:
            // DELETE /api/admin/accounts/{id or email}.
            await Task.Delay(300);

            UsersList.Remove(user);
            if (SelectedUser == user)
            {
                ClearForm();
            }

            _notificationManager.ShowNotification("Account deleted", $"{user.Email} was removed.", true);
        }

        [RelayCommand]
        private void ClearForm()
        {
            SelectedUser = null;
            NewEmail = string.Empty;
            NewPassword = string.Empty;
            NewPhoneNumber = string.Empty;
            NewProvince = string.Empty;
            SelectedRole = "Guide";
        }
    }
}
