using System.Windows.Controls;
using TravelApp.ViewModels.Admin;

namespace TravelApp.Views.Admin
{
    public partial class ContentManagementView : UserControl
    {
        public ContentManagementView(ContentManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
