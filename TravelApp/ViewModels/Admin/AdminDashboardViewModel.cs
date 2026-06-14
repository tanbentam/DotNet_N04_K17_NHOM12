using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;

namespace TravelApp.ViewModels.Admin
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;
        private readonly IRoleNavigationService _navigationService;

        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _errorMessage;
        [ObservableProperty] private string _adminName;
        [ObservableProperty] private string _lastUpdatedText;
        [ObservableProperty] private int _totalAccounts;
        [ObservableProperty] private int _totalUsers;
        [ObservableProperty] private int _totalGuides;
        [ObservableProperty] private int _pendingContent;
        [ObservableProperty] private int _pendingBookings;
        [ObservableProperty] private int _pendingCancellations;
        [ObservableProperty] private int _paidTours;
        [ObservableProperty] private decimal _simulatedRevenue;
        [ObservableProperty]
        private ObservableCollection<BookingModel> _attentionBookings =
            new ObservableCollection<BookingModel>();

        public AdminDashboardViewModel(
            IUserRepository userRepository,
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService,
            IRoleNavigationService navigationService)
        {
            _userRepository = userRepository;
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            _navigationService = navigationService;
            AdminName = _sessionService.CurrentUser?.FullName ?? "Quản trị viên";
            _ = LoadDashboardAsync();
        }

        public bool HasAttentionItems => AttentionBookings.Count > 0;

        public bool HasNoAttentionItems => !HasAttentionItems;

        [RelayCommand]
        private async Task LoadDashboardAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var accountsTask = _userRepository.GetAllAsync();
                var destinationsTask = _contentRepository.GetDestinationsAsync();
                var hotelsTask = _contentRepository.GetHotelsAsync();
                var bookingsTask = _contentRepository.GetBookingsAsync();

                await Task.WhenAll(
                    accountsTask,
                    destinationsTask,
                    hotelsTask,
                    bookingsTask);

                var accounts = await accountsTask;
                var destinations = await destinationsTask;
                var hotels = await hotelsTask;
                var bookings = await bookingsTask;

                TotalAccounts = accounts.Count;
                TotalUsers = accounts.Count(user => user.Role == RoleType.User);
                TotalGuides = accounts.Count(
                    user => user.Role == RoleType.TourGuide);
                PendingContent =
                    destinations.Count(item =>
                        item.ApprovalStatus == ContentApprovalStatus.Pending) +
                    hotels.Count(item =>
                        item.ApprovalStatus == ContentApprovalStatus.Pending);
                PendingBookings = bookings.Count(
                    item => item.Status == BookingStatus.Pending);
                PendingCancellations = bookings.Count(
                    item => item.HasPendingGuideCancellation ||
                        item.HasPendingRefundRequest);
                PaidTours = bookings.Count(item =>
                    item.Status == BookingStatus.Paid ||
                    item.Status == BookingStatus.Completed);
                SimulatedRevenue = bookings
                    .Where(item =>
                        item.Status == BookingStatus.Paid ||
                        item.Status == BookingStatus.Completed)
                    .Sum(item => item.Price);

                AttentionBookings = new ObservableCollection<BookingModel>(
                    bookings
                        .Where(item =>
                            item.HasPendingGuideCancellation ||
                            item.HasPendingRefundRequest ||
                            item.Status == BookingStatus.Pending ||
                            item.Status == BookingStatus.Paid)
                        .OrderByDescending(item =>
                            item.HasPendingGuideCancellation)
                        .ThenBy(item => GetStatusPriority(item.Status))
                        .ThenBy(item => item.StartDate)
                        .Take(8));
                OnPropertyChanged(nameof(HasAttentionItems));
                OnPropertyChanged(nameof(HasNoAttentionItems));

                AdminName = _sessionService.CurrentUser?.FullName ?? "Quản trị viên";
                LastUpdatedText =
                    "Cập nhật lúc " + DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load admin dashboard",
                    ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void NavigateToAccounts()
        {
            _navigationService.NavigateToAdminSection("accounts");
        }

        [RelayCommand]
        private void NavigateToContent()
        {
            _navigationService.NavigateToAdminSection("content");
        }

        [RelayCommand]
        private void Logout()
        {
            _sessionService.SignOut();
        }

        private static int GetStatusPriority(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.Pending:
                    return 0;
                case BookingStatus.Paid:
                    return 1;
                default:
                    return 2;
            }
        }
    }
}
