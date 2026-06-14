using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TravelApp
{
    public sealed class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool flag && !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool flag && !flag;
        }
    }

    public sealed class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class EnumNotEqualConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value == null ||
                !string.Equals(
                    value.ToString(),
                    parameter?.ToString(),
                    StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class EnumToVietnameseConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            switch (value.ToString())
            {
                case "Admin":
                    return "Quản trị viên";
                case "TourGuide":
                    return "Hướng dẫn viên";
                case "User":
                    return "Người dùng";
                case "Pending":
                    return "Đang chờ";
                case "Accepted":
                    return "Đã chấp nhận";
                case "Rejected":
                    return "Đã từ chối";
                case "Paid":
                    return "Đã thanh toán";
                case "Cancelled":
                    return "Đã hủy";
                case "Completed":
                    return "Hoàn thành";
                case "Approved":
                    return "Đã duyệt";
                case "Successful":
                    return "Thành công";
                case "Failed":
                    return "Thất bại";
                case "Refunded":
                    return "Đã hoàn tiền";
                case "QrCode":
                    return "Mã QR";
                case "BankTransfer":
                    return "Chuyển khoản";
                default:
                    return value.ToString();
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
