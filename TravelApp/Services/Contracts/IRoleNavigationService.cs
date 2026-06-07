using System;
using TravelApp.Models;

namespace TravelApp.Services.Contracts
{
    public interface IRoleNavigationService
    {
        event Action<UserModel> DashboardRequested;

        void NavigateToDashboard(UserModel user);
    }
}
