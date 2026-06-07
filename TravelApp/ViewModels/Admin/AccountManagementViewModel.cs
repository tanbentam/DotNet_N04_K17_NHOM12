using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;

namespace TravelApp.ViewModels.Admin
{
    public partial class AccountManagementViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;

        [ObservableProperty]
        private ObservableCollection<UserModel> _usersList;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private int _userCount;

        [ObservableProperty]
        private bool _isEmpty;

        [ObservableProperty]
        private bool _hasUsers;

        public AccountManagementViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            UsersList = new ObservableCollection<UserModel>();
            _ = LoadAccountsAsync();
        }

        [RelayCommand]
        private async Task LoadAccountsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

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
                ErrorMessage = "Không thể tải danh sách tài khoản: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CreateGuideAccount()
        {
            // Account creation is implemented in the next checklist item.
        }

        [RelayCommand]
        private void CreateUserAccount()
        {
            // Account creation is implemented in the next checklist item.
        }

        [RelayCommand]
        private async Task DeleteAccountAsync(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                if (!await _userRepository.DeleteAsync(user.Id))
                {
                    ErrorMessage = "Không tìm thấy tài khoản cần xóa.";
                    return;
                }

                UsersList.Remove(user);
                UpdateSummary();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
            }
        }

        private void UpdateSummary()
        {
            UserCount = UsersList.Count;
            IsEmpty = UserCount == 0;
            HasUsers = UserCount > 0;
        }
    }
}
