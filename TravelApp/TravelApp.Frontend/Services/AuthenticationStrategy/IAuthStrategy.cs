using TravelApp.Frontend.Models;
using TravelApp.Frontend.ViewModels;

namespace TravelApp.Frontend.Services.AuthenticationStrategy
{
    [cite_start]// Interface định nghĩa Strategy 
    public interface IAuthStrategy
    {
        void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser);
    }
}