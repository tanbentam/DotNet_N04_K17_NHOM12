using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TravelApp.Frontend.Contracts;
using TravelApp.Frontend.Services.Api;
using TravelApp.Frontend.Services.Navigation;
using TravelApp.Frontend.Services.NotificationQueue;
using TravelApp.Frontend.ViewModels;
using TravelApp.Frontend.ViewModels.Admin;
using TravelApp.Frontend.ViewModels.Authentication;
using TravelApp.Frontend.ViewModels.TourGuide;
using TravelApp.Frontend.ViewModels.User;

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

            services.AddSingleton<IAuthService, MockAuthService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<NotificationManager>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<AdminDashboardViewModel>();
            services.AddTransient<AccountManagementViewModel>();
            services.AddTransient<ContentManagementViewModel>();
            services.AddTransient<GuideDashboardViewModel>();
            services.AddTransient<UserDashboardViewModel>();
            services.AddTransient<BookingRequestsViewModel>();
            services.AddTransient<ScheduleManagementViewModel>();
            services.AddTransient<AdvancedSearchViewModel>();
            services.AddTransient<TourBookingViewModel>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
