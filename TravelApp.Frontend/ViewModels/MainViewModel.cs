using CommunityToolkit.Mvvm.ComponentModel;
using TravelApp.Frontend.Contracts;

namespace TravelApp.Frontend.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _windowTitle = "Digital Travel Application";

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;
            _navigationService.NavigateToLogin();
        }
    }
}
