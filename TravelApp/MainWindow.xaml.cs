using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravelApp.Services;
using TravelApp.Services.Logging;
using TravelApp.Views.Admin;
using TravelApp.Views.Authentication;
using TravelApp.Views.TourGuide;
using TravelApp.Views.User;

namespace TravelApp
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseConnectionService _databaseConnectionService;
        private readonly IServiceProvider _services;

        public MainWindow(
            DatabaseConnectionService databaseConnectionService,
            IServiceProvider services)
        {
            _databaseConnectionService = databaseConnectionService;
            _services = services;
            InitializeComponent();
            ShowHome();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            var result = await _databaseConnectionService.CheckConnectionAsync();
            if (result.IsConnected)
            {
                DatabaseStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                DatabaseStatusText.Text = result.Message;
                return;
            }

            DatabaseStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            DatabaseStatusText.Text = "Database unavailable";
            DatabaseStatusText.ToolTip = result.Message;
            LoggerService.LogDatabaseConnectionFailure(result.Message);
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var destination = (sender as Button)?.Tag as string;

            switch (destination)
            {
                case "login": ShowView(
                    "Login",
                    _services.GetRequiredService<LoginView>()); break;
                case "register": ShowView(
                    "Register",
                    _services.GetRequiredService<RegisterView>()); break;
                case "user": ShowView("User Dashboard", new UserDashboardView()); break;
                case "guide": ShowView("Tour Guide Dashboard", new GuideDashboardView()); break;
                case "admin": ShowView("Admin Dashboard", new AdminDashboardView()); break;
                case "accounts": ShowView(
                    "Account Management",
                    _services.GetRequiredService<AccountManagementView>()); break;
                case "content": ShowView(
                    "Content Management",
                    _services.GetRequiredService<ContentManagementView>()); break;
                default: ShowHome(); break;
            }
        }

        private void ShowView(string title, UserControl view)
        {
            PageTitle.Text = title;
            MainContent.Content = view;
        }

        private void ShowHome()
        {
            PageTitle.Text = "Home";
            MainContent.Content = CreateHomeContent();
        }

        private static UIElement CreateHomeContent()
        {
            var panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 620
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Welcome to Travel App",
                FontSize = 36,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            panel.Children.Add(new TextBlock
            {
                Text = "Use the menu on the left to open the available view components.",
                FontSize = 17,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 16, 0, 0)
            });

            return panel;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
