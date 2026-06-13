using System;
using TravelApp.Models;

namespace TravelApp.Services.Contracts
{
    public interface IRoleNavigationService
    {
        event Action<UserModel> DashboardRequested;
        event Action<string> AdminSectionRequested;

        void NavigateToDashboard(UserModel user);
        void NavigateToAdminSection(string section);
    }
}
