using System;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;

namespace TravelApp.Services
{
    public sealed class UserSessionService : IUserSessionService
    {
        public event Action<UserModel> SessionChanged;

        public UserModel CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public void SignIn(UserModel user)
        {
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
            SessionChanged?.Invoke(CurrentUser);
        }

        public void SignOut()
        {
            CurrentUser = null;
            SessionChanged?.Invoke(null);
        }

        public bool HasRole(RoleType role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }
    }
}
