using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TravelApp.Frontend.Contracts;

namespace TravelApp.Frontend.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly AccountManagementViewModel _accountManagementViewModel;
        private readonly ContentManagementViewModel _contentManagementViewModel;

        [ObservableProperty]
        private ObservableObject _currentAdminContent;

        public AdminDashboardViewModel(
            INavigationService navigationService,
            AccountManagementViewModel accountManagementViewModel,
            ContentManagementViewModel contentManagementViewModel)
        {
            _navigationService = navigationService;
            _accountManagementViewModel = accountManagementViewModel;
            _contentManagementViewModel = contentManagementViewModel;
            CurrentAdminContent = _accountManagementViewModel;
        }

        [RelayCommand]
        private void NavigateToAccounts()
        {
            CurrentAdminContent = _accountManagementViewModel;
        }

        [RelayCommand]
        private void NavigateToContent()
        {
            CurrentAdminContent = _contentManagementViewModel;
        }

        [RelayCommand]
        private void Logout()
        {
            _navigationService.NavigateToLogin();
        }
    }
}
