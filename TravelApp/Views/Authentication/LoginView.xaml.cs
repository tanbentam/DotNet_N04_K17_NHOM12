using System.Windows.Controls;
using TravelApp.ViewModels.Authentication;

namespace TravelApp.Views.Authentication
{
    public partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
