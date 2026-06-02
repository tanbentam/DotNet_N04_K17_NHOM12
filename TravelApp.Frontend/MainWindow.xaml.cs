using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TravelApp.Frontend.ViewModels;

namespace TravelApp.Frontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<MainViewModel>();
        }
    }
}
