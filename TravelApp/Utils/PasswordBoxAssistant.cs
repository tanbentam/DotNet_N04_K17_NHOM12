using System.Windows;
using System.Windows.Controls;

namespace TravelApp.Utils
{
    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxAssistant),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordBoxAssistant),
                new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordBoxAssistant));

        public static string GetBoundPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(
            DependencyObject dependencyObject,
            string value)
        {
            dependencyObject.SetValue(BoundPasswordProperty, value);
        }

        public static bool GetBindPassword(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(BindPasswordProperty);
        }

        public static void SetBindPassword(
            DependencyObject dependencyObject,
            bool value)
        {
            dependencyObject.SetValue(BindPasswordProperty, value);
        }

        private static void OnBindPasswordChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is PasswordBox passwordBox))
            {
                return;
            }

            passwordBox.PasswordChanged -= HandlePasswordChanged;
            if ((bool)eventArgs.NewValue)
            {
                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void OnBoundPasswordChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(dependencyObject is PasswordBox passwordBox) ||
                (bool)passwordBox.GetValue(IsUpdatingProperty))
            {
                return;
            }

            passwordBox.PasswordChanged -= HandlePasswordChanged;
            passwordBox.Password = eventArgs.NewValue as string ??
                string.Empty;
            passwordBox.PasswordChanged += HandlePasswordChanged;
        }

        private static void HandlePasswordChanged(
            object sender,
            RoutedEventArgs eventArgs)
        {
            var passwordBox = (PasswordBox)sender;
            passwordBox.SetValue(IsUpdatingProperty, true);
            SetBoundPassword(passwordBox, passwordBox.Password);
            passwordBox.SetValue(IsUpdatingProperty, false);
        }
    }
}
