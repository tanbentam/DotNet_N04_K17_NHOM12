# TravelApp

TravelApp là ứng dụng desktop quản lý du lịch được xây dựng bằng WPF trên .NET Framework 4.7.2. Dự án sử dụng mô hình MVVM, Material Design cho giao diện, CommunityToolkit.Mvvm để hỗ trợ binding/command, Entity Framework 6 để kết nối MySQL.

Hiện tại TravelApp được phát triển theo kiến trúc monolith, nghĩa là toàn bộ giao diện, xử lý nghiệp vụ, service và truy cập dữ liệu đều nằm trong một project duy nhất tên là `TravelApp`.

## 1. Công nghệ sử dụng

Dự án sử dụng các công nghệ chính sau:

* WPF
* .NET Framework 4.7.2
* MVVM
* CommunityToolkit.Mvvm
* MaterialDesignThemes
* Entity Framework 6
* MySQL
* Dependency Injection

## 2. Kiến trúc dự án

Toàn bộ source code nằm trong project `TravelApp`.

Cấu trúc thư mục chính:

```txt
TravelApp/
├── Models/
├── Views/
├── ViewModels/
├── Services/
├── Data/
└── Utils/
```

Ý nghĩa từng thư mục:

```txt
Models
```

Chứa các entity và domain model như `User`, `Booking`, `Hotel`, `Destination`.

```txt
Views
```

Chứa giao diện WPF như `MainWindow`, Login, Register, Admin Dashboard, Guide Dashboard, User Dashboard.

```txt
ViewModels
```

Chứa state, binding properties và command cho từng màn hình theo mô hình MVVM.

```txt
Services
```

Chứa các service xử lý nghiệp vụ như authentication, booking, notification, image upload và logging.

```txt
Data
```

Chứa `ApplicationDbContext`, cấu hình Entity Framework 6, connection string và migrations.

```txt
Utils
```

Chứa các helper, constants và validation dùng chung.

## 3. Chức năng hiện có

Dự án hiện đã hoàn thành phần nền tảng chính:

* Khởi tạo solution và project WPF `TravelApp`
* Tổ chức thư mục theo kiến trúc MVVM
* Cấu hình Material Design
* Cấu hình CommunityToolkit.Mvvm
* Cấu hình Dependency Injection
* Cấu hình Entity Framework 6 và MySQL provider
* Tạo `ApplicationDbContext`
* Tạo các model cơ bản
* Build Debug thành công và tạo file `TravelApp.exe`

## 4. Giao diện ứng dụng

Ứng dụng hiện đã có application shell cơ bản, bao gồm:

* `MainWindow`
* Sidebar navigation
* Màn hình Home mặc định
* Màn hình Login
* Màn hình Register
* User Dashboard
* Guide Dashboard
* Admin Dashboard
* Account Management
* Content Management

Một số phần giao diện vẫn cần tiếp tục chuẩn hóa, đặc biệt là text tiếng Việt UTF-8 và trạng thái các nút navigation.

## 5. Authentication và phân quyền

Dự án đã có nền tảng cho authentication:

* `IAuthService`
* `AuthService`
* Login ViewModel
* Register ViewModel
* Validation email
* Validation số điện thoại
* Điều hướng theo role: Admin, Guide, User

Những phần cần tiếp tục hoàn thiện:

* Kết nối form Login/Register với DI và `AuthService`
* Thay tài khoản mock `admin/admin` bằng dữ liệu thật trong database
* Lưu tài khoản đăng ký vào database
* Thay SHA-256 bằng password hashing có salt như PBKDF2, bcrypt hoặc Argon2
* Tự động điều hướng theo role sau khi đăng nhập
* Quản lý phiên đăng nhập và logout

## 6. Chức năng Admin

Admin hiện đã có:

* Admin Dashboard
* Account Management UI
* Account Management ViewModel
* Content Management UI
* Content Management ViewModel

Cần tiếp tục hoàn thiện:

* Hiển thị danh sách tài khoản từ dữ liệu thật
* Tạo, sửa, xóa User/Guide trong database
* Quản lý Destination và Hotel
* Duyệt Hotel và nội dung do Guide tạo
* Theo dõi và quản lý Booking

## 7. Chức năng Guide

Guide hiện đã có:

* Guide Dashboard
* Schedule Management ViewModel
* Booking Requests ViewModel

Cần tiếp tục hoàn thiện:

* Gắn Schedule và Booking Requests vào Guide Dashboard
* Lưu lịch trống vào database
* Tạo và chỉnh sửa Destination
* Tạo và chỉnh sửa Hotel
* Chấp nhận hoặc từ chối Booking bằng dữ liệu thật

## 8. Chức năng User

User hiện đã có:

* User Dashboard
* Advanced Search ViewModel
* Tour Booking ViewModel
* Payment Simulation ViewModel

Cần tiếp tục hoàn thiện:

* Gắn search, booking và payment vào User Dashboard
* Tìm kiếm Destination, Hotel và Guide từ database
* Tạo và hủy Booking bằng dữ liệu thật
* Hoàn thiện hồ sơ người dùng
* Hoàn thiện yêu thích và đánh giá
* Hoàn thiện quy trình thanh toán mô phỏng

## 9. Database

Dự án sử dụng Entity Framework 6 để kết nối MySQL.

Hiện đã có:

* `ApplicationDbContext`
* Model cho User, Booking, Hotel, Destination
* Enum cho Role và Booking Status
* Connection string MySQL
* Kiểm tra trạng thái kết nối database khi ứng dụng khởi động

Cần tiếp tục hoàn thiện:

* Cập nhật MySQL credentials hợp lệ
* Hoàn thiện mapping, relationships và constraints
* Tạo EF6 migrations ban đầu
* Tạo hoặc cập nhật schema MySQL bằng migrations
* Tách data access khỏi ViewModel và đưa vào Service/Data phù hợp

## 10. Supporting Services

Dự án đã có một số service hỗ trợ:

* Local Notification Queue
* File Logger Service
* Image Upload Service nền tảng

Cần tiếp tục hoàn thiện:

* Hiển thị popup notification trong application shell
* Kết nối logging vào các luồng lỗi thực tế
* Hoàn thiện Image Upload Service
* Lưu ảnh cục bộ hoặc trên storage thật
* Hoàn thiện Booking Service và pricing rules

## 11. Cách chạy dự án

### Yêu cầu môi trường

Cần cài đặt:

* Visual Studio
* .NET Framework 4.7.2 Developer Pack
* MySQL Server
* NuGet packages cần thiết cho WPF, EF6, MaterialDesignThemes và CommunityToolkit.Mvvm

### Các bước chạy

Clone repository:

```bash
git clone <repository-url>
```

Mở solution bằng Visual Studio.

Restore NuGet packages nếu Visual Studio chưa tự restore.

Cập nhật connection string MySQL trong file cấu hình của project.

Ví dụ:

```xml
<connectionStrings>
  <add name="ApplicationDbContext"
       connectionString="server=localhost;database=travelapp;user id=root;password=your_password;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

Build project ở chế độ Debug.

Chạy project `TravelApp`.

Nếu cấu hình database hợp lệ, ứng dụng sẽ kiểm tra kết nối database khi khởi động.

## 12. Trạng thái hiện tại

Dự án hiện đã hoàn thành phần foundation, build configuration, application shell và skeleton giao diện chính.

Giai đoạn hiện tại của dự án là hoàn thiện MVVM/DI wiring và thay dữ liệu mô phỏng bằng dữ liệu thật từ EF6/MySQL trong cùng project `TravelApp`.

## 13. Việc cần làm tiếp theo

Các việc nên ưu tiên trong giai đoạn tiếp theo:

1. Cập nhật MySQL credentials hợp lệ.
2. Hoàn thiện mapping và relationships cho các model.
3. Tạo EF6 migrations ban đầu.
4. Kết nối Login/Register với database.
5. Thay mock account bằng dữ liệu thật.
6. Gán đầy đủ `DataContext` cho các View.
7. Đăng ký đầy đủ Services và ViewModels trong DI container.
8. Hoàn thiện luồng User, Guide và Admin bằng dữ liệu thật.
9. Viết unit test cho validation, authentication và booking.
10. Chuẩn bị release build và deployment package.

## 14. Ghi chú phát triển

Dự án không tách thành Frontend, Backend API hoặc Contracts riêng.

Tất cả code vẫn nằm trong một project duy nhất là `TravelApp`. Tuy nhiên, mỗi lớp vẫn cần được tách trách nhiệm rõ ràng để dễ bảo trì:

* View chỉ xử lý giao diện
* ViewModel xử lý binding và command
* Service xử lý nghiệp vụ
* Data xử lý truy cập database
* Model mô tả dữ liệu và entity

Cách tổ chức này giúp dự án đơn giản, phù hợp với desktop application, nhưng vẫn giữ được cấu trúc rõ ràng theo MVVM.
