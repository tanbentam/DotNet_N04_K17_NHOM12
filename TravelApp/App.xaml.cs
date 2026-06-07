using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TravelApp.Data.Repositories;
using TravelApp.Services;
using TravelApp.Services.Contracts;
using TravelApp.ViewModels;
using TravelApp.ViewModels.Admin;
using TravelApp.ViewModels.Authentication;
using TravelApp.Views.Admin;
using TravelApp.Views.Authentication;

namespace TravelApp
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
            services.AddSingleton<DatabaseConnectionService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITravelContentRepository, TravelContentRepository>();
            services.AddTransient<AccountManagementViewModel>();
            services.AddTransient<ContentManagementViewModel>();
            services.AddTransient<AccountManagementView>();
            services.AddTransient<ContentManagementView>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Khởi chạy MainWindow
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
