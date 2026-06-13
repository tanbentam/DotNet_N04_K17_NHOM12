using System;
using TravelApp.Models;
using TravelApp.Services.Contracts;

namespace TravelApp.Services
{
    public sealed class RoleNavigationService : IRoleNavigationService
    {
        public event Action<UserModel> DashboardRequested;
        public event Action<string> AdminSectionRequested;

        public void NavigateToDashboard(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            DashboardRequested?.Invoke(user);
        }

        public void NavigateToAdminSection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                throw new ArgumentException(
                    "Admin section cannot be empty.",
                    nameof(section));
            }

            AdminSectionRequested?.Invoke(section);
        }
    }
}
