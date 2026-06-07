using System;
using TravelApp.Models;
using TravelApp.Services.Contracts;

namespace TravelApp.Services
{
    public sealed class RoleNavigationService : IRoleNavigationService
    {
        public event Action<UserModel> DashboardRequested;

        public void NavigateToDashboard(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            DashboardRequested?.Invoke(user);
        }
    }
}
