using System;
using TravelApp.Models;
using TravelApp.Models.Enums;

namespace TravelApp.Services.Contracts
{
    public interface IUserSessionService
    {
        event Action<UserModel> SessionChanged;

        UserModel CurrentUser { get; }
        bool IsAuthenticated { get; }

        void SignIn(UserModel user);
        void SignOut();
        bool HasRole(RoleType role);
    }
}
