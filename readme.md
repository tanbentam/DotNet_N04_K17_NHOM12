# TravelApp

TravelApp là ứng dụng desktop quản lý du lịch xây dựng bằng WPF trên
.NET Framework 4.7.2. Ứng dụng hỗ trợ ba vai trò `User`, `TourGuide` và
`Admin`, sử dụng Entity Framework 6 kết nối MySQL và tổ chức code theo MVVM
trong một project monolith.

## Mục lục

- [Tính năng](#tính-năng)
- [Công nghệ](#công-nghệ)
- [Kiến trúc](#kiến-trúc)
- [Yêu cầu môi trường](#yêu-cầu-môi-trường)
- [Cài đặt](#cài-đặt)
- [Cấu hình database](#cấu-hình-database)
- [Khởi tạo tài khoản Admin](#khởi-tạo-tài-khoản-admin)
- [Build và chạy](#build-và-chạy)
- [Migration](#migration)
- [Kiểm thử](#kiểm-thử)
- [Dữ liệu và file cục bộ](#dữ-liệu-và-file-cục-bộ)
- [Quy tắc nghiệp vụ](#quy-tắc-nghiệp-vụ)
- [Trạng thái hiện tại](#trạng-thái-hiện-tại)
- [Xử lý lỗi thường gặp](#xử-lý-lỗi-thường-gặp)

## Tính năng

### Chung

- Đăng ký và đăng nhập bằng email hoặc số điện thoại.
- Mật khẩu được băm bằng PBKDF2-SHA256 với salt.
- Quản lý phiên đăng nhập, logout và điều hướng theo role.
- Ẩn navigation không thuộc quyền của tài khoản hiện tại.
- Kiểm tra kết nối và schema MySQL khi ứng dụng khởi động.
- Popup notification và file logging cục bộ.
- Upload và lưu ảnh Destination/Hotel trên máy.

### User

- Tìm kiếm Destination, Hotel và Guide từ database.
- Lọc theo địa điểm, giá, rating, tên Guide và lịch trống.
- Chọn Destination, Hotel, Guide, ngày khởi hành và số ngày.
- Tính giá tour, phí dịch vụ và giảm giá chuyến dài.
- Tạo và theo dõi booking.
- Hủy booking đang `Pending` hoặc `Accepted`.
- Thanh toán mô phỏng bằng QR hoặc chuyển khoản.
- Xem lịch sử giao dịch thành công/thất bại.
- Cập nhật hồ sơ cá nhân.
- Thêm/bỏ yêu thích Hotel và Guide.
- Tạo, cập nhật và xóa đánh giá.

### TourGuide

- Khai báo lịch trống theo từng ngày trong tuần.
- Tạo và chỉnh sửa Destination để Admin duyệt.
- Tạo và chỉnh sửa Hotel để Admin duyệt.
- Chấp nhận hoặc từ chối yêu cầu booking.
- Xem lịch làm việc, ngày bắt đầu, ngày hoàn thành và tiến độ tour.
- Gửi yêu cầu hủy booking `Accepted` để Admin duyệt hoặc từ chối.

### Admin

- Tạo, sửa và xóa tài khoản User/TourGuide.
- Quản lý Destination và Hotel.
- Duyệt hoặc từ chối nội dung do Guide tạo.
- Theo dõi và cập nhật trạng thái booking.
- Xem, duyệt hoặc từ chối yêu cầu hủy của Guide.
- Chuyển booking `Paid` sang `Completed`.

## Công nghệ

| Thành phần | Công nghệ |
|---|---|
| UI | WPF |
| Runtime | .NET Framework 4.7.2 |
| Ngôn ngữ | C# 8.0 |
| Pattern | MVVM |
| MVVM Toolkit | CommunityToolkit.Mvvm 8.4.2 |
| UI Library | MaterialDesignThemes 5.3.2 |
| ORM | Entity Framework 6.5.2 |
| Database | MySQL |
| MySQL Provider | MySql.Data / MySql.Data.EntityFramework 8.1.0 |
| DI | Microsoft.Extensions.DependencyInjection 10.0.8 |

## Kiến trúc

Repository có một solution file và một project ứng dụng:

```text
TravelApp.slnx
TravelApp/
└── TravelApp.csproj
```

Các thư mục chính:

```text
TravelApp/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   └── Repositories/
├── Models/
│   └── Enums/
├── Services/
│   ├── Booking/
│   ├── Contracts/
│   ├── ImageManagement/
│   ├── Logging/
│   └── NotificationQueue/
├── Utils/
├── ViewModels/
│   ├── Admin/
│   ├── Authentication/
│   ├── Shared/
│   ├── TourGuide/
│   └── User/
└── Views/
    ├── Admin/
    ├── Authentication/
    ├── Components/
    ├── TourGuide/
    └── User/
```

Trách nhiệm:

- `Views`: XAML và code-behind tối thiểu cho giao diện.
- `ViewModels`: state, binding và command.
- `Services`: authentication, booking và dịch vụ hỗ trợ.
- `Repositories`: truy cập database bằng EF6.
- `Models`: entity và thuộc tính domain.
- `Data/Migrations`: schema history của EF6.
- `Utils`: password hashing và validation dùng chung.

Đây là desktop monolith; project không tách riêng Frontend, Backend API hoặc
Contracts assembly.

## Yêu cầu môi trường

### Bắt buộc

- Windows 10/11.
- Visual Studio có workload **.NET desktop development**.
- .NET Framework 4.7.2 Developer Pack/Targeting Pack.
- Visual Studio MSBuild hỗ trợ WPF .NET Framework.
- MySQL Server hoặc MySQL cloud service như Aiven.
- Quyền tạo/cập nhật schema trên database trong lần chạy đầu.

### Khuyến nghị

- Visual Studio 2022 hoặc mới hơn.
- Git.
- MySQL Workbench, DBeaver hoặc client SQL tương đương.

> `dotnet build` không phải cách build đáng tin cậy cho project WPF
> .NET Framework kiểu cũ này. Hãy dùng Visual Studio hoặc MSBuild đi kèm
> Visual Studio.

## Cài đặt

```powershell
git clone <repository-url>
cd DotNet_N04_K17_NHOM12
```

Mở `TravelApp.slnx` trong Visual Studio. Nếu IDE không hỗ trợ `.slnx`, mở trực
tiếp `TravelApp\TravelApp.csproj`.

Restore package:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  TravelApp\TravelApp.csproj `
  /t:Restore
```

Đường dẫn MSBuild có thể khác tùy phiên bản Visual Studio. Có thể tìm bằng:

```powershell
& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
  -latest `
  -products * `
  -requires Microsoft.Component.MSBuild `
  -find "MSBuild\**\Bin\MSBuild.exe"
```

## Cấu hình database

`TravelApp\App.config` chứa connection string local và đã được `.gitignore`.
Không commit file có credential thật.

Nếu chưa có file local, tạo `TravelApp\App.config`. Tên connection string phải
chính xác là `DefaultConnection`.

Mẫu cấu hình đầy đủ:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section
      name="entityFramework"
      type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
      requirePermission="false" />
  </configSections>

  <connectionStrings>
    <add
      name="DefaultConnection"
      connectionString="SslMode=Required;
                        Server=YOUR_HOST;
                        Port=3306;
                        Database=travelapp_dev;
                        Uid=YOUR_USERNAME;
                        Pwd=YOUR_PASSWORD;"
      providerName="MySql.Data.MySqlClient" />
  </connectionStrings>

  <system.data>
    <DbProviderFactories>
      <remove invariant="MySql.Data.MySqlClient" />
      <add
        name="MySQL Data Provider"
        invariant="MySql.Data.MySqlClient"
        description=".NET Framework Data Provider for MySQL"
        type="MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=8.1.0.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />
    </DbProviderFactories>
  </system.data>

  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>

  <entityFramework>
    <defaultConnectionFactory
      type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework">
      <parameters>
        <parameter value="mssqlLocalDb" />
      </parameters>
    </defaultConnectionFactory>
    <providers>
      <provider
        invariantName="MySql.Data.MySqlClient"
        type="MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.EntityFramework, Version=8.1.0.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />
    </providers>
  </entityFramework>
</configuration>
```

Với MySQL local không dùng TLS, đổi `SslMode` theo cấu hình server, ví dụ
`SslMode=None`.

Database phải tồn tại trước khi chạy ứng dụng:

```sql
CREATE DATABASE travelapp_dev
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
```

Ứng dụng dùng:

```csharp
MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>
```

Khi khởi động, EF6 sẽ áp dụng các migration còn thiếu. Thanh trạng thái phía
trái hiển thị `Database ready` khi kết nối, migration và truy vấn health check
đều thành công.

## Khởi tạo tài khoản Admin

Project không seed sẵn tài khoản Admin và không dùng tài khoản mock.

Cách đơn giản để tạo Admin đầu tiên:

1. Chạy ứng dụng.
2. Đăng ký một account qua màn hình Register.
3. Đóng ứng dụng hoặc logout.
4. Trong MySQL, đổi role của account đó sang `Admin`.

```sql
UPDATE Users
SET Role = 1
WHERE Email = 'admin@example.com';
```

Giá trị role:

| Role | Giá trị |
|---|---:|
| Admin | 1 |
| TourGuide | 2 |
| User | 3 |

Account được đăng ký qua UI đã có password hash PBKDF2 hợp lệ, vì vậy không
cần tự tạo hoặc lưu password dạng rõ trong database.

Sau khi cập nhật role, đăng nhập lại bằng email/số điện thoại và password đã
đăng ký.

## Build và chạy

### Visual Studio

1. Mở `TravelApp.slnx` hoặc `TravelApp\TravelApp.csproj`.
2. Chọn `Debug` và `Any CPU`.
3. Restore NuGet packages.
4. Chọn project `TravelApp` làm startup project.
5. Build và chạy.

### PowerShell

```powershell
$msbuild = "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"

& $msbuild `
  TravelApp\TravelApp.csproj `
  /t:Rebuild `
  /p:Configuration=Debug `
  /restore `
  /m

.\TravelApp\bin\Debug\TravelApp.exe
```

Output:

```text
TravelApp\bin\Debug\TravelApp.exe
TravelApp\bin\Debug\TravelApp.exe.config
TravelApp\bin\Debug\TravelApp.pdb
```

Build Release:

```powershell
& $msbuild `
  TravelApp\TravelApp.csproj `
  /t:Rebuild `
  /p:Configuration=Release `
  /restore `
  /m
```

## Migration

Migration hiện có:

| Migration | Nội dung |
|---|---|
| `InitialCreate` | Users, Bookings, Destinations, Hotels, GuideAvailabilities |
| `SyncMySqlModelSnapshot` | Đồng bộ kiểu dữ liệu MySQL |
| `AddFavoritesAndReviews` | Favorites và Reviews |
| `AddPayments` | Payment history |
| `AddGuideCancellationRequests` | Yêu cầu hủy booking của Guide |

### Áp dụng migration

Thông thường chỉ cần chạy ứng dụng. Initializer sẽ gọi migration đến phiên bản
mới nhất.

Có thể chạy thủ công bằng EF6 CLI sau khi đã build:

```powershell
$ef6 = "$env:USERPROFILE\.nuget\packages\entityframework\6.5.2\tools\net45\any\ef6.exe"

Push-Location TravelApp\bin\Debug

& $ef6 database update `
  --assembly TravelApp.exe `
  --project-dir "..\.." `
  --config TravelApp.exe.config `
  --migrations-config TravelApp.Data.Migrations.Configuration

Pop-Location
```

### Tạo migration mới

1. Sửa model và mapping.
2. Build Debug thành công.
3. Đảm bảo database cấu hình đã có toàn bộ migration cũ.
4. Chạy:

```powershell
$ef6 = "$env:USERPROFILE\.nuget\packages\entityframework\6.5.2\tools\net45\any\ef6.exe"

Push-Location TravelApp\bin\Debug

& $ef6 migrations add MigrationName `
  --assembly TravelApp.exe `
  --project-dir "..\.." `
  --config TravelApp.exe.config `
  --migrations-config TravelApp.Data.Migrations.Configuration `
  --root-namespace TravelApp

Pop-Location
```

Sau khi scaffold, thêm file `.cs`, `.Designer.cs` và `.resx` mới vào
`TravelApp.csproj` vì đây là project format cũ, không tự include file.

Không bật automatic destructive migration trên database có dữ liệu quan trọng.

## Kiểm thử

### Smoke test nhanh

1. Build Debug.
2. Chạy ứng dụng.
3. Kiểm tra `Database ready`.
4. Đăng nhập lần lượt User, Guide và Admin.
5. Kiểm tra navigation đúng role và logout.

### Full regression

Tài liệu test đầy đủ:

- [Full regression test flow](FULL_REGRESSION_TEST_FLOW.md)
- [Development checklist](CHECKLIST.md)

Flow regression bao gồm hơn 200 checkpoint cho:

- Authentication và role navigation.
- Account/content CRUD.
- Guide availability và approval workflow.
- Search, pricing và booking.
- Payment success/failure.
- Work schedule và cancellation request.
- Profile, favorites, reviews.
- Persistence, logs và image storage.

Project hiện chưa có automated test project. Unit test, integration test và UI
smoke test tự động vẫn nằm trong backlog.

## Dữ liệu và file cục bộ

### Ảnh upload

Ảnh hợp lệ:

- JPG/JPEG
- PNG
- BMP
- GIF
- Tối đa 5 MB

Vị trí lưu:

```text
%LocalAppData%\TravelApp\Images\Destination
%LocalAppData%\TravelApp\Images\Hotel
```

Tên file được tạo từ SHA-256 của nội dung ảnh, giúp tránh lưu trùng cùng một
file.

### Log

Log nằm cạnh file executable:

```text
TravelApp\bin\Debug\TravelApp_ErrorLog.txt
TravelApp\bin\Release\TravelApp_ErrorLog.txt
```

Logger ghi các nhóm lỗi như:

- `LOGIN_FAILURE`
- `BOOKING_FAILURE`
- `DATABASE_CONNECTION`
- `DATABASE_ERROR`
- `WARNING`
- `ERROR`

Password không được ghi trực tiếp. Một số khóa nhạy cảm phổ biến được che bằng
`***`.

## Quy tắc nghiệp vụ

### Booking status

```text
Pending -> Accepted -> Paid -> Completed
Pending -> Rejected
Pending -> Cancelled
Accepted -> Cancelled
```

- Guide chỉ được xử lý booking `Pending`.
- Guide có thể `Accept` hoặc `Reject`.
- User có thể hủy `Pending` hoặc `Accepted`.
- Chỉ booking `Accepted` mới được thanh toán.
- Admin chuyển booking `Paid` sang `Completed`.
- Booking `Rejected`, `Cancelled` hoặc `Completed` không chuyển tiếp.

### Lịch và trùng lịch

- Tour từ 1 đến 30 ngày.
- Chỉ đặt tối đa 365 ngày trước ngày hiện tại.
- Booking mới phải phù hợp lịch trống của Guide.
- Booking `Accepted` và `Paid` được dùng để kiểm tra trùng lịch.
- Một Guide không thể nhận hai tour chồng thời gian.
- Một User không thể có hai tour `Accepted/Paid` chồng thời gian.

### Giá tour

```text
Guide fee = 500,000 x số ngày
Hotel fee = giá phòng/đêm x số ngày
Giảm giá = 10% subtotal nếu tour từ 7 ngày
Phí dịch vụ = 5% sau giảm giá
Tổng = subtotal - giảm giá + phí dịch vụ
```

### Yêu cầu hủy của Guide

- Chỉ gửi cho booking `Accepted`, chưa thanh toán.
- Tour phải chưa bắt đầu.
- Lý do từ 10 đến 500 ký tự.
- Trong lúc chờ Admin, User không thể thanh toán booking.
- Admin có thể duyệt để chuyển booking sang `Cancelled`, hoặc từ chối để giữ
  `Accepted`.

### Approval

- Nội dung do Guide tạo hoặc sửa có trạng thái `Pending`.
- User chỉ tìm thấy Destination và Hotel `Approved`.
- Hotel chỉ hợp lệ khi Destination của Hotel cũng `Approved`.

## Trạng thái hiện tại

Phần cốt lõi đã hoạt động:

- Authentication và phân quyền.
- CRUD account/content.
- Approval workflow.
- Search, booking và kiểm tra lịch.
- Payment simulation.
- Guide work schedule và cancellation request.
- Profile, favorites, reviews.
- Notification, logging và image upload.

Backlog chính:

- Tự động chuyển tour đã kết thúc sang `Completed`.
- Chỉ cho đánh giá Guide/Hotel sau khi hoàn thành tour tương ứng.
- Hủy và hoàn tiền booking đã thanh toán.
- Lịch sử hoàn tiền.
- Thông báo đa vai trò cho hủy/hoàn tiền.
- Chuẩn hóa toàn bộ text tiếng Việt.
- Unit, integration và UI tests tự động.
- Environment-specific configuration.
- Security review và release package.

Theo dõi chi tiết trong [CHECKLIST.md](CHECKLIST.md).

Hành vi cần lưu ý của phiên bản hiện tại:

- Form Register có nhãn `Tỉnh/Thành phố` nhưng giá trị đang được lưu vào
  `UserModel.FullName`. User có thể sửa lại họ tên trong tab hồ sơ.
- Payment chỉ là mô phỏng, không kết nối cổng thanh toán thật.
- Booking chưa tự động chuyển sang `Completed`.
- Review hiện chưa yêu cầu User phải có tour `Completed`.

## Xử lý lỗi thường gặp

### `Database error` khi khởi động

Kiểm tra:

- Database đã tồn tại.
- Host/port/user/password đúng.
- `SslMode` phù hợp server.
- User MySQL có quyền tạo và thay đổi bảng.
- Connection string tên `DefaultConnection`.
- MySQL provider trong `App.config` đầy đủ.

Thông tin chi tiết được ghi trong `TravelApp_ErrorLog.txt`.

### `dotnet build` báo thiếu EF6 hoặc CommunityToolkit

Dùng Visual Studio MSBuild thay cho `dotnet build`:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  TravelApp\TravelApp.csproj `
  /t:Rebuild `
  /p:Configuration=Debug `
  /restore
```

### WPF XAML không nhận command/property generated

- Restore `CommunityToolkit.Mvvm`.
- Kiểm tra analyzer source generator trong `TravelApp.csproj`.
- Clean `bin`/`obj`, sau đó rebuild bằng Visual Studio MSBuild.
- Đảm bảo file ViewModel mới đã được thêm vào `TravelApp.csproj`.

### Migration mới không được compile

Project dùng old-style `.csproj`. Thêm thủ công:

- Migration `.cs`
- Migration `.Designer.cs`
- Migration `.resx`

vào đúng `Compile` và `EmbeddedResource` item trong project.

### Không xóa được User, Guide, Hotel hoặc Destination

Đây thường là ràng buộc dữ liệu:

- User/Guide đang có booking, payment, favorite hoặc review.
- Hotel đang có booking.
- Destination đang có Hotel hoặc booking.

Xóa dữ liệu phụ thuộc trước hoặc giữ lại dữ liệu làm lịch sử.

### Guide không nhận được booking

Kiểm tra:

- Booking còn `Pending`.
- Tour nằm trong lịch trống của Guide.
- Không trùng booking `Accepted/Paid` khác.
- Guide đăng nhập đúng tài khoản được User chọn.

### User không thấy booking trong tab thanh toán

Booking phải:

- Thuộc User hiện tại.
- Có trạng thái `Accepted`.
- Không có yêu cầu hủy của Guide đang chờ Admin xử lý.

## Tài liệu liên quan

- [Development checklist](CHECKLIST.md)
- [Full regression test flow](FULL_REGRESSION_TEST_FLOW.md)
- [Project tree](tree.md)

## Bảo mật

- Không commit `TravelApp\App.config` có credential thật.
- Không ghi password rõ vào database hoặc log.
- Không chia sẻ connection string qua issue, commit hoặc ảnh chụp màn hình.
- Nếu credential từng bị commit, phải rotate credential và làm sạch Git
  history; chỉ thêm `.gitignore` là chưa đủ.

## License

Project hiện chưa khai báo license.
