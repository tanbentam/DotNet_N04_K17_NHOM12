using System.Windows.Controls;
using TravelApp.ViewModels.Admin;

namespace TravelApp.Views.Admin
{
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView(AdminDashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
