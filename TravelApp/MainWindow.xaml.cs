using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;
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
        private readonly IBookingService _bookingService;

        public MainWindow(
            DatabaseConnectionService databaseConnectionService,
            IServiceProvider services,
            IRoleNavigationService roleNavigationService,
            IUserSessionService sessionService,
            NotificationManager notificationManager,
            IBookingService bookingService)
        {
            _databaseConnectionService = databaseConnectionService;
            _services = services;
            _roleNavigationService = roleNavigationService;
            _sessionService = sessionService;
            _notificationManager = notificationManager;
            _bookingService = bookingService;
            InitializeComponent();
            NotificationPopup.DataContext = notificationManager;
            ShowHome();
            UpdateSessionDisplay();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            _roleNavigationService.DashboardRequested += ShowDashboardForUser;
            _roleNavigationService.AdminSectionRequested +=
                ShowAdminSection;
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
                    "Cơ sở dữ liệu",
                    result.Message);
                await CompleteExpiredBookingsAsync();
                return;
            }

            DatabaseStatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            DatabaseStatusText.Text = result.Message.IndexOf(
                "schema",
                StringComparison.OrdinalIgnoreCase) >= 0
                ? "Lỗi cấu trúc cơ sở dữ liệu"
                : "Lỗi cơ sở dữ liệu";
            DatabaseStatusText.ToolTip = result.Message;
            _notificationManager.ShowNotification(
                "Lỗi cơ sở dữ liệu",
                result.Message,
                true);
        }

        private async System.Threading.Tasks.Task CompleteExpiredBookingsAsync()
        {
            try
            {
                var completedCount =
                    await _bookingService.CompleteExpiredBookingsAsync();
                if (completedCount > 0)
                {
                    _notificationManager.ShowNotification(
                        "Cập nhật đặt tour",
                        completedCount +
                        " tour đã kết thúc được chuyển sang Hoàn thành.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogException(
                    "Auto complete expired bookings",
                    ex);
                _notificationManager.ShowNotification(
                    "Cảnh báo",
                    "Không thể tự động cập nhật tour đã kết thúc.",
                    true);
            }
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var destination = (sender as Button)?.Tag as string;

            switch (destination)
            {
                case "login": ShowView(
                    "Đăng nhập",
                    _services.GetRequiredService<LoginView>(),
                    LoginNavButton); break;
                case "register": ShowView(
                    "Đăng ký",
                    _services.GetRequiredService<RegisterView>(),
                    RegisterNavButton); break;
                case "user":
                    ShowRoleView(
                        RoleType.User,
                        "Bảng điều khiển người dùng",
                        () => _services.GetRequiredService<UserDashboardView>(),
                        UserDashboardNavButton);
                    break;
                case "guide":
                    ShowRoleView(
                        RoleType.TourGuide,
                        "Bảng điều khiển hướng dẫn viên",
                        () => _services.GetRequiredService<GuideDashboardView>(),
                        GuideDashboardNavButton);
                    break;
                case "admin":
                    ShowRoleView(
                        RoleType.Admin,
                        "Bảng điều khiển quản trị",
                        () => _services.GetRequiredService<AdminDashboardView>(),
                        AdminDashboardNavButton);
                    break;
                case "accounts":
                    ShowRoleView(
                        RoleType.Admin,
                        "Quản lý tài khoản",
                        () => _services.GetRequiredService<AccountManagementView>(),
                        AccountManagementNavButton);
                    break;
                case "content":
                    ShowRoleView(
                        RoleType.Admin,
                        "Quản lý nội dung",
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
                        "Bảng điều khiển quản trị",
                        _services.GetRequiredService<AdminDashboardView>(),
                        AdminDashboardNavButton);
                    break;
                case RoleType.TourGuide:
                    ShowView(
                        "Bảng điều khiển hướng dẫn viên",
                        _services.GetRequiredService<GuideDashboardView>(),
                        GuideDashboardNavButton);
                    break;
                case RoleType.User:
                    ShowView(
                        "Bảng điều khiển người dùng",
                        _services.GetRequiredService<UserDashboardView>(),
                        UserDashboardNavButton);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Vai trò người dùng không được hỗ trợ: {user.Role}");
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _roleNavigationService.DashboardRequested -= ShowDashboardForUser;
            _roleNavigationService.AdminSectionRequested -=
                ShowAdminSection;
            _sessionService.SessionChanged -= HandleSessionChanged;
        }

        private void ShowAdminSection(string section)
        {
            switch (section)
            {
                case "accounts":
                    ShowRoleView(
                        RoleType.Admin,
                        "Quản lý tài khoản",
                        () => _services.GetRequiredService<AccountManagementView>(),
                        AccountManagementNavButton);
                    break;
                case "content":
                    ShowRoleView(
                        RoleType.Admin,
                        "Quản lý nội dung",
                        () => _services.GetRequiredService<ContentManagementView>(),
                        ContentManagementNavButton);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(section),
                        section,
                        "Khu vực quản trị không được hỗ trợ.");
            }
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
                ? $"{user.FullName} ({GetRoleDisplayName(user.Role)})"
                : string.Empty;
            CurrentUserText.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;
            LogoutButton.Visibility = isAuthenticated
                ? Visibility.Visible
                : Visibility.Collapsed;

            UpdateNavigationVisibility(user);
        }

        private static string GetRoleDisplayName(RoleType role)
        {
            switch (role)
            {
                case RoleType.Admin:
                    return "Quản trị viên";
                case RoleType.TourGuide:
                    return "Hướng dẫn viên";
                default:
                    return "Người dùng";
            }
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
            ShowView("Đăng nhập", _services.GetRequiredService<LoginView>());
        }

        private void ShowAccessDenied(RoleType requiredRole)
        {
            PageTitle.Text = "Từ chối truy cập";
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
            PageTitle.Text = "Trang chủ";
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
                Text = "Chào mừng đến với Ứng dụng Du lịch",
                FontSize = 36,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            panel.Children.Add(new TextBlock
            {
                Text = "Sử dụng menu bên trái để mở các chức năng của ứng dụng.",
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
