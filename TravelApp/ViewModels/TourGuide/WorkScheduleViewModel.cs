using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TravelApp.Data.Repositories;
using TravelApp.Models;
using TravelApp.Models.Enums;
using TravelApp.Services.Contracts;
using TravelApp.Services.Logging;

namespace TravelApp.ViewModels.TourGuide
{
    public partial class WorkScheduleViewModel : ObservableObject
    {
        private readonly ITravelContentRepository _contentRepository;
        private readonly IUserSessionService _sessionService;

        [ObservableProperty]
        private ObservableCollection<BookingModel> _workSchedule;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage;

        public WorkScheduleViewModel(
            ITravelContentRepository contentRepository,
            IUserSessionService sessionService)
        {
            _contentRepository = contentRepository;
            _sessionService = sessionService;
            WorkSchedule = new ObservableCollection<BookingModel>();
            _ = LoadWorkScheduleAsync();
        }

        [RelayCommand]
        public async Task LoadWorkScheduleAsync()
        {
            ErrorMessage = string.Empty;
            var guide = _sessionService.CurrentUser;
            if (guide == null || guide.Role != RoleType.TourGuide)
            {
                ErrorMessage = "Phiên đăng nhập Guide không hợp lệ.";
                return;
            }

            IsBusy = true;
            try
            {
                WorkSchedule = new ObservableCollection<BookingModel>(
                    await _contentRepository.GetWorkScheduleByGuideAsync(
                        guide.Id));
            }
            catch (Exception ex)
            {
                ErrorMessage = DatabaseErrorDiagnostics.Report(
                    "Load guide work schedule",
                    ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
