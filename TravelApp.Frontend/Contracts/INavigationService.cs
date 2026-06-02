using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TravelApp.Frontend.Models;

namespace TravelApp.Frontend.Contracts
{
    public interface INavigationService
    {
        event Action<ObservableObject> CurrentViewModelChanged;

        void NavigateToLogin();
        void NavigateToRegister();
        void NavigateToRoleDashboard(UserModel user);
        void NavigateToAdminDashboard();
        void NavigateToGuideDashboard();
        void NavigateToUserDashboard();
    }
}
