using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TravelApp.Services.Contracts;

namespace TravelApp.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentAdminContent;

        private readonly AccountManagementViewModel _accountManagementVM;
        private readonly ContentManagementViewModel _contentManagementVM;
        private readonly IUserSessionService _sessionService;

        public AdminDashboardViewModel(
            AccountManagementViewModel accountManagementVM,
            ContentManagementViewModel contentManagementVM,
            IUserSessionService sessionService)
        {
            _accountManagementVM = accountManagementVM;
            _contentManagementVM = contentManagementVM;
            _sessionService = sessionService;
            CurrentAdminContent = _accountManagementVM;
        }

        [RelayCommand]
        private void NavigateToAccounts()
        {
            CurrentAdminContent = _accountManagementVM;
        }

        [RelayCommand]
        private void NavigateToContent()
        {
            CurrentAdminContent = _contentManagementVM;
        }

        [RelayCommand]
        private void Logout()
        {
            _sessionService.SignOut();
        }
    }
}
