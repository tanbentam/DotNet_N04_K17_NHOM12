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

        public AccountManagementViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            UsersList = new ObservableCollection<UserModel>();
            LoadAccountsAsync();
        }

        private async void LoadAccountsAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var users = await _userRepository.GetAllAsync();
                UsersList = new ObservableCollection<UserModel>(users);
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
        private void CreateGuideAccount()
        {
            // The account form will call the repository when create/edit is implemented.
        }

        [RelayCommand]
        private void CreateUserAccount()
        {
            // The account form will call the repository when create/edit is implemented.
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
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
            }
        }
    }
}
