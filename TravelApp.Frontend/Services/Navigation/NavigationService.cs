using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels.Admin;
using TravelApp.Frontend.ViewModels.Authentication;
using TravelApp.Frontend.ViewModels.TourGuide;
using TravelApp.Frontend.ViewModels.User;

namespace TravelApp.Frontend.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _services;

        public event Action<ObservableObject> CurrentViewModelChanged;

        public NavigationService(IServiceProvider services)
        {
            _services = services;
        }

        public void NavigateToLogin()
        {
            Navigate(_services.GetRequiredService<LoginViewModel>());
        }

        public void NavigateToRegister()
        {
            Navigate(_services.GetRequiredService<RegisterViewModel>());
        }

        public void NavigateToRoleDashboard(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToAdminDashboard();
                return;
            }

            if (string.Equals(user.Role, "Guide", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(user.Role, "TourGuide", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToGuideDashboard();
                return;
            }

            NavigateToUserDashboard();
        }

        public void NavigateToAdminDashboard()
        {
            Navigate(_services.GetRequiredService<AdminDashboardViewModel>());
        }

        public void NavigateToGuideDashboard()
        {
            Navigate(_services.GetRequiredService<GuideDashboardViewModel>());
        }

        public void NavigateToUserDashboard()
        {
            Navigate(_services.GetRequiredService<UserDashboardViewModel>());
        }

        private void Navigate(ObservableObject viewModel)
        {
            CurrentViewModelChanged?.Invoke(viewModel);
        }
    }
}
