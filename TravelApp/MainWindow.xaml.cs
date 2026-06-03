using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TravelApp.ViewModels;

namespace TravelApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Gán DataContext từ Dependency Injection Container
            DataContext = App.Current.Services.GetService<MainViewModel>();
        }
    }
}