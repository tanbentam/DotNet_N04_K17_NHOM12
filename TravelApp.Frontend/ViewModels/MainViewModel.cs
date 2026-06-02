using CommunityToolkit.Mvvm.ComponentModel;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Services.NotificationQueue;

namespace TravelApp.Frontend.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private ObservableObject _currentViewModel;
        private string _windowTitle = "Digital Travel Application";

        public NotificationManager NotificationManager { get; }

        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel(INavigationService navigationService, NotificationManager notificationManager)
        {
            _navigationService = navigationService;
            NotificationManager = notificationManager;
            _navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;
            _navigationService.NavigateToLogin();
        }
    }
}
