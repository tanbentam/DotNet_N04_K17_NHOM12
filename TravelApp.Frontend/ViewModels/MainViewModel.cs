using CommunityToolkit.Mvvm.ComponentModel;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public NotificationManager NotificationManager { get; }

        [ObservableProperty]
        private string _windowTitle = "Digital Travel Application";

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel(INavigationService navigationService, NotificationManager notificationManager)
        {
            _navigationService = navigationService;
            NotificationManager = notificationManager;
            _navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;
            _navigationService.NavigateToLogin();
        }
    }
}
