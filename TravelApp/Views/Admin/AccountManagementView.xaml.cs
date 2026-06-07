using System.Windows.Controls;
using TravelApp.ViewModels.Admin;

namespace TravelApp.Views.Admin
{
    public partial class AccountManagementView : UserControl
    {
        public AccountManagementView(AccountManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
