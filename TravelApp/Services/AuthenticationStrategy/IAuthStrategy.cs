using TravelApp.Common.Models;
using TravelApp.Frontend.ViewModels;

namespace TravelApp.Services.AuthenticationStrategy
{
    [cite_start]// Interface định nghĩa Strategy 
    public interface IAuthStrategy
    {
        void NavigateToDashboard(MainViewModel mainViewModel, UserModel currentUser);
    }
}