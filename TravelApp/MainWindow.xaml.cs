using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services;
using TravelApp.Services.Contracts;
using TravelApp.Services.NotificationQueue;
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
        private readonly IRoleNavigationService _roleNavigationService;
        private readonly IUserSessionService _sessionService;
        private readonly NotificationManager _notificationManager;

        public MainWindow(
            DatabaseConnectionService databaseConnectionService,
            IServiceProvider services,
            IRoleNavigationService roleNavigationService,
            IUserSessionService sessionService,
            NotificationManager notificationManager)
        {
            _databaseConnectionService = databaseConnectionService;
            _services = services;
            _roleNavigationService = roleNavigationService;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            InitializeComponent();
            NotificationPopup.DataContext = notificationManager;
            ShowHome();
            UpdateSessionDisplay();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            _roleNavigationService.DashboardRequested += ShowDashboardForUser;
            _sessionService.SessionChanged += HandleSessionChanged;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            var result = await _databaseConnectionService.CheckConnectionAsync();
            if (result.IsConnected)
            {
                DatabaseStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                DatabaseStatusText.Text = result.Message;
                _notificationManager.ShowNotification(
                    "Database",
                    result.Message);
                return;
            }

            DatabaseStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            DatabaseStatusText.Text = result.Message.IndexOf(
                "schema",
                StringComparison.OrdinalIgnoreCase) >= 0
                ? "Database schema error"
                : "Database error";
            DatabaseStatusText.ToolTip = result.Message;
            _notificationManager.ShowNotification(
                "Lỗi cơ sở dữ liệu",
                result.Message,
                true);
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var destination = (sender as Button)?.Tag as string;

            switch (destination)
            {
                case "login": ShowView(
                    "Login",
                    _services.GetRequiredService<LoginView>(),
                    LoginNavButton); break;
                case "register": ShowView(
                    "Register",
                    _services.GetRequiredService<RegisterView>(),
                    RegisterNavButton); break;
                case "user":
                    ShowRoleView(
                        RoleType.User,
                        "User Dashboard",
                        () => _services.GetRequiredService<UserDashboardView>(),
                        UserDashboardNavButton);
                    break;
                case "guide":
                    ShowRoleView(
                        RoleType.TourGuide,
                        "Tour Guide Dashboard",
                        () => _services.GetRequiredService<GuideDashboardView>(),
                        GuideDashboardNavButton);
                    break;
                case "admin":
                    ShowRoleView(
                        RoleType.Admin,
                        "Admin Dashboard",
                        () => _services.GetRequiredService<AdminDashboardView>(),
                        AdminDashboardNavButton);
                    break;
                case "accounts":
                    ShowRoleView(
                        RoleType.Admin,
                        "Account Management",
                        () => _services.GetRequiredService<AccountManagementView>(),
                        AccountManagementNavButton);
                    break;
                case "content":
                    ShowRoleView(
                        RoleType.Admin,
                        "Content Management",
                        () => _services.GetRequiredService<ContentManagementView>(),
                        ContentManagementNavButton);
                    break;
                default: ShowHome(); break;
            }
        }

        private void ShowRoleView(
            RoleType requiredRole,
            string title,
            Func<UserControl> viewFactory,
            Button activeNavButton)
        {
            if (!_sessionService.IsAuthenticated)
            {
                ShowLogin();
                return;
            }

            if (!_sessionService.HasRole(requiredRole))
            {
                ShowAccessDenied(requiredRole);
                return;
            }

            ShowView(title, viewFactory(), activeNavButton);
        }

        private void ShowDashboardForUser(UserModel user)
        {
            switch (user.Role)
            {
                case RoleType.Admin:
                    ShowView(
                        "Admin Dashboard",
                        _services.GetRequiredService<AdminDashboardView>(),
                        AdminDashboardNavButton);
                    break;
                case RoleType.TourGuide:
                    ShowView(
                        "Tour Guide Dashboard",
                        _services.GetRequiredService<GuideDashboardView>(),
                        GuideDashboardNavButton);
                    break;
                case RoleType.User:
                    ShowView(
                        "User Dashboard",
                        _services.GetRequiredService<UserDashboardView>(),
                        UserDashboardNavButton);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported user role: {user.Role}");
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _roleNavigationService.DashboardRequested -= ShowDashboardForUser;
            _sessionService.SessionChanged -= HandleSessionChanged;
        }

        private void HandleSessionChanged(UserModel user)
        {
            UpdateSessionDisplay();

            if (user == null)
            {
                ShowLogin();
            }
        }

        private void UpdateSessionDisplay()
        {
            var user = _sessionService.CurrentUser;
            var isAuthenticated = user != null;

            CurrentUserText.Text = isAuthenticated
                ? $"{user.FullName} ({user.Role})"
                : string.Empty;
            CurrentUserText.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;
            LogoutButton.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;

            UpdateNavigationVisibility(user);
        }

        private void UpdateNavigationVisibility(UserModel user)
        {
            var isAuthenticated = user != null;

            LoginNavButton.Visibility = isAuthenticated
                ? Visibility.Collapsed
                : Visibility.Visible;
            RegisterNavButton.Visibility = isAuthenticated
                ? Visibility.Collapsed
                : Visibility.Visible;

            RoleSectionSeparator.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;
            RoleSectionTitle.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;

            UserDashboardNavButton.Visibility = IsRole(user, RoleType.User)
                ? Visibility.Visible
                : Visibility.Collapsed;
            GuideDashboardNavButton.Visibility = IsRole(user, RoleType.TourGuide)
                ? Visibility.Visible
                : Visibility.Collapsed;
            AdminDashboardNavButton.Visibility = IsRole(user, RoleType.Admin)
                ? Visibility.Visible
                : Visibility.Collapsed;
            AccountManagementNavButton.Visibility = IsRole(user, RoleType.Admin)
                ? Visibility.Visible
                : Visibility.Collapsed;
            ContentManagementNavButton.Visibility = IsRole(user, RoleType.Admin)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private static bool IsRole(UserModel user, RoleType role)
        {
            return user != null && user.Role == role;
        }

        private void ShowLogin()
        {
            ShowView("Login", _services.GetRequiredService<LoginView>());
        }

        private void ShowAccessDenied(RoleType requiredRole)
        {
            PageTitle.Text = "Access Denied";
            MainContent.Content = new TextBlock
            {
                Text = $"Tài khoản hiện tại không có quyền {requiredRole}.",
                FontSize = 20,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ClearActiveNavigationButton();
        }

        private void ShowView(string title, UserControl view, Button activeNavButton = null)
        {
            PageTitle.Text = title;
            MainContent.Content = view;
            SetActiveNavigationButton(activeNavButton);
        }

        private void ShowHome()
        {
            PageTitle.Text = "Home";
            MainContent.Content = CreateHomeContent();
            SetActiveNavigationButton(HomeNavButton);
        }

        private void SetActiveNavigationButton(Button activeButton)
        {
            ClearActiveNavigationButton();

            if (activeButton == null)
                return;

            activeButton.Background = new SolidColorBrush(Color.FromRgb(94, 53, 177));
            activeButton.FontWeight = FontWeights.SemiBold;
        }

        private void ClearActiveNavigationButton()
        {
            foreach (var button in GetNavigationButtons())
            {
                button.ClearValue(BackgroundProperty);
                button.ClearValue(FontWeightProperty);
            }
        }

        private Button[] GetNavigationButtons()
        {
            return new[]
            {
                HomeNavButton,
                LoginNavButton,
                RegisterNavButton,
                UserDashboardNavButton,
                GuideDashboardNavButton,
                AdminDashboardNavButton,
                AccountManagementNavButton,
                ContentManagementNavButton
            };
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _sessionService.SignOut();
        }
    }
}
