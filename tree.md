.
├── .gitignore
├── CHECKLIST.md
├── readme.md
├── TravelApp.slnx
└── TravelApp
    ├── App.config
    ├── App.xaml
    ├── App.xaml.cs
    ├── Converters.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    ├── TravelApp.csproj
    ├── Data
    │   └── ApplicationDbContext.cs
    ├── Models
    │   ├── BookingModel.cs
    │   ├── DestinationModel.cs
    │   ├── HotelModel.cs
    │   ├── UserModel.cs
    │   └── Enums
    │       ├── BookingStatus.cs
    │       └── RoleType.cs
    ├── Properties
    │   ├── AssemblyInfo.cs
    │   ├── Resources.Designer.cs
    │   ├── Resources.resx
    │   ├── Settings.Designer.cs
    │   └── Settings.settings
    ├── Services
    │   ├── AuthService.cs
    │   ├── BookingService.cs
    │   ├── DatabaseConnectionService.cs
    │   ├── AuthenticationStrategy
    │   │   ├── AdminAuthStrategy.cs
    │   │   ├── AuthContext.cs
    │   │   ├── GuideAuthStrategy.cs
    │   │   ├── IAuthStrategy.cs
    │   │   └── UserAuthStrategy.cs
    │   ├── Contracts
    │   │   └── IAuthService.cs
    │   ├── ImageManagement
    │   │   └── ImageUploadService.cs
    │   ├── Logging
    │   │   └── LoggerService.cs
    │   └── NotificationQueue
    │       ├── NotificationManager.cs
    │       └── NotificationMessage.cs
    ├── Utils
    │   ├── Constants.cs
    │   ├── PasswordHelper.cs
    │   └── ValidationHelper.cs
    ├── ViewModels
    │   ├── MainViewModel.cs
    │   ├── Admin
    │   │   ├── AccountManagementViewModel.cs
    │   │   ├── AdminDashboardViewModel.cs
    │   │   └── ContentManagementViewModel.cs
    │   ├── Authentication
    │   │   ├── LoginViewModel.cs
    │   │   └── RegisterViewModel.cs
    │   ├── Shared
    │   │   └── PaymentSimulationViewModel.cs
    │   ├── TourGuide
    │   │   ├── BookingRequestsViewModel.cs
    │   │   ├── GuideDashboardViewModel.cs
    │   │   └── ScheduleManagementViewModel.cs
    │   └── User
    │       ├── AdvancedSearchViewModel.cs
    │       ├── TourBookingViewModel.cs
    │       └── UserDashboardViewModel.cs
    └── Views
        ├── Admin
        │   ├── AccountManagementView.xaml
        │   ├── AccountManagementView.xaml.cs
        │   ├── AdminDashboardView.xaml
        │   ├── AdminDashboardView.xaml.cs
        │   ├── ContentManagementView.xaml
        │   └── ContentManagementView.xaml.cs
        ├── Authentication
        │   ├── LoginView.xaml
        │   ├── LoginView.xaml.cs
        │   ├── RegisterView.xaml
        │   └── RegisterView.xaml.cs
        ├── Components
        │   └── PopupNotification.xaml
        ├── TourGuide
        │   ├── GuideDashboardView.xaml
        │   └── GuideDashboardView.xaml.cs
        └── User
            ├── UserDashboardView.xaml
            └── UserDashboardView.xaml.cs