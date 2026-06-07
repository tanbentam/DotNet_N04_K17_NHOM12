using System.Windows.Controls;
using TravelApp.ViewModels.User;

namespace TravelApp.Views.User
{
    public partial class UserDashboardView : UserControl
    {
        public UserDashboardView(UserDashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
