using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TravelApp.Data.Repositories;
using TravelApp.Services;
using TravelApp.Services.Contracts;
using TravelApp.ViewModels;
using TravelApp.ViewModels.Admin;
using TravelApp.ViewModels.Authentication;
using TravelApp.ViewModels.TourGuide;
using TravelApp.ViewModels.User;
using TravelApp.ViewModels.Shared; // Đã thêm thư viện này
using TravelApp.Views.Admin;
using TravelApp.Views.Authentication;
using TravelApp.Views.TourGuide;
using TravelApp.Views.User;

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

            // Đăng ký ViewModels và Services
            services.AddTransient<MainViewModel>();
            services.AddSingleton<DatabaseConnectionService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITravelContentRepository, TravelContentRepository>();
            services.AddTransient<AccountManagementViewModel>();
            services.AddTransient<ContentManagementViewModel>();
            services.AddTransient<AccountManagementView>();
            services.AddTransient<ContentManagementView>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddSingleton<IRoleNavigationService, RoleNavigationService>();
            services.AddSingleton<IUserSessionService, UserSessionService>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<AdminDashboardViewModel>();
            services.AddTransient<GuideDashboardViewModel>();

            // --- CÁC VIEWMODEL CON CỦA USER DASHBOARD ĐÃ ĐƯỢC ĐĂNG KÝ VÀO ĐÂY ---
            services.AddTransient<AdvancedSearchViewModel>();
            services.AddTransient<TourBookingViewModel>();
            services.AddTransient<PaymentSimulationViewModel>();
            services.AddSingleton<TravelApp.Services.NotificationQueue.NotificationManager>();
            // -------------------------------------------------------------------

            services.AddTransient<UserDashboardViewModel>();
            services.AddTransient<AdminDashboardView>();
            services.AddTransient<GuideDashboardView>();
            services.AddTransient<UserDashboardView>();
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}