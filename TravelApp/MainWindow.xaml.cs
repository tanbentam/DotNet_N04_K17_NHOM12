using System.Windows;
using System.Windows.Controls;
using TravelApp.Views.Admin;
using TravelApp.Views.Authentication;
using TravelApp.Views.TourGuide;
using TravelApp.Views.User;

namespace TravelApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowHome();
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var destination = (sender as Button)?.Tag as string;

            switch (destination)
            {
                case "login": ShowView("Login", new LoginView()); break;
                case "register": ShowView("Register", new RegisterView()); break;
                case "user": ShowView("User Dashboard", new UserDashboardView()); break;
                case "guide": ShowView("Tour Guide Dashboard", new GuideDashboardView()); break;
                case "admin": ShowView("Admin Dashboard", new AdminDashboardView()); break;
                case "accounts": ShowView("Account Management", new AccountManagementView()); break;
                case "content": ShowView("Content Management", new ContentManagementView()); break;
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
