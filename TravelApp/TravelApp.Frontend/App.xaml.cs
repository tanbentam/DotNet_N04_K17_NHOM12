using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TravelApp.Frontend.ViewModels;

namespace TravelApp.Frontend
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Đăng ký ViewModels
            services.AddTransient<MainViewModel>();

            // Đăng ký Services (Sẽ thêm vào ở các phần sau)
            // services.AddSingleton<IAuthService, AuthService>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Khởi chạy MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}