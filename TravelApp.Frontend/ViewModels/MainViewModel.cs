using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Services.NotificationQueue;
using TravelApp.Frontend.ViewModels.Admin;
using TravelApp.Frontend.ViewModels.Authentication;
using TravelApp.Frontend.ViewModels.TourGuide;
using TravelApp.Frontend.ViewModels.User;
using TravelApp.Frontend.Views.Admin;
using TravelApp.Frontend.Views.Authentication;
using TravelApp.Frontend.Views.TourGuide;
using TravelApp.Frontend.Views.User;

namespace TravelApp.Frontend.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private ObservableObject _currentViewModel;
        private UserControl _currentView;
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
            set
            {
                if (SetProperty(ref _currentViewModel, value))
                {
                    CurrentView = BuildRootView(value);
                }
            }
        }

        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainViewModel(INavigationService navigationService, NotificationManager notificationManager)
        {
            _navigationService = navigationService;
            NotificationManager = notificationManager;
            _navigationService.CurrentViewModelChanged += viewModel => CurrentViewModel = viewModel;
            _navigationService.NavigateToLogin();
        }

        private static UserControl BuildRootView(ObservableObject viewModel)
        {
            UserControl view;

            if (viewModel is LoginViewModel)
            {
                view = new LoginView();
            }
            else if (viewModel is RegisterViewModel)
            {
                view = new RegisterView();
            }
            else if (viewModel is AdminDashboardViewModel)
            {
                view = new AdminDashboardView();
            }
            else if (viewModel is GuideDashboardViewModel)
            {
                view = new GuideDashboardView();
            }
            else if (viewModel is UserDashboardViewModel)
            {
                view = new UserDashboardView();
            }
            else
            {
                view = new UserControl();
            }

            view.DataContext = viewModel;
            return view;
        }
    }
}
