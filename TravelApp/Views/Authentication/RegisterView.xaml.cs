using System.Windows.Controls;
using TravelApp.ViewModels.Authentication;

namespace TravelApp.Views.Authentication
{
    public partial class RegisterView : UserControl
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
