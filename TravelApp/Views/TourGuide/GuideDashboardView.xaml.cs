using System.Windows.Controls;
using TravelApp.ViewModels.TourGuide;

namespace TravelApp.Views.TourGuide
{
    public partial class GuideDashboardView : UserControl
    {
        public GuideDashboardView(GuideDashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
