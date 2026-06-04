using TravelApp.Models;
using TravelApp.ViewModels;

namespace TravelApp.Services.AuthenticationStrategy
{
    public interface IAuthStrategy
    {
        void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser);
    }
}