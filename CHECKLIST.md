# TravelApp Development Checklist

Checklist này phản ánh kiến trúc hiện tại của dự án:

- WPF trên .NET Framework 4.7.2
- MVVM với CommunityToolkit.Mvvm
- Material Design
- Entity Framework 6 kết nối MySQL
- Một desktop application monolith duy nhất
- UI, business logic, services và data access cùng nằm trong project `TravelApp`
- Không tách Frontend, Backend API hoặc Contracts thành project riêng

Ký hiệu:

- `[x]` Đã hoàn thành
- `[~]` Đã có nền tảng, cần hoàn thiện wiring hoặc nghiệp vụ
- `[ ]` Chưa thực hiện

## 1. Project Foundation

- [x] Khởi tạo Git repository và nhánh làm việc
- [x] Tạo solution và project WPF `TravelApp`
- [x] Tổ chức thư mục theo `Models`, `Views`, `ViewModels`, `Services`, `Data`, `Utils`
- [x] Thống nhất kiến trúc một project cho toàn bộ ứng dụng
- [x] Cấu hình MaterialDesignThemes
- [x] Cấu hình CommunityToolkit.Mvvm
- [x] Cấu hình Dependency Injection
- [x] Cấu hình Entity Framework 6 và MySQL provider
- [x] Khắc phục WPF markup compilation cho MVVM source generator
- [x] Build Debug thành công và tạo `TravelApp.exe`

## 2. Application Shell

- [x] Tạo `MainWindow`
- [x] Tạo sidebar navigation
- [x] Điều hướng tới Login và Register
- [x] Điều hướng tới User, Guide và Admin Dashboard
- [x] Điều hướng tới Account Management và Content Management
- [x] Tạo màn hình Home mặc định
- [x] Thêm code-behind cần thiết cho các WPF `UserControl`
- [~] Chuẩn hóa toàn bộ text tiếng Việt về UTF-8
- [x] Chuẩn hóa giao diện và trạng thái nút navigation

## 3. MVVM And Dependency Wiring

- [x] Tạo các ViewModel chính cho Authentication, Admin, Guide và User
- [x] Tạo command và observable properties bằng CommunityToolkit.Mvvm
- [~] Đăng ký đầy đủ Services và ViewModels trong DI container
- [~] Gán `DataContext` cho từng View
- [~] Chuyển navigation từ code-behind sang `MainViewModel` nếu cần MVVM hoàn chỉnh
- [~] Kết nối các ViewModel con trong Admin, Guide và User Dashboard

## 4. Data Layer

- [x] Tạo `ApplicationDbContext`
- [x] Tạo models cho User, Booking, Hotel và Destination
- [x] Tạo enums cho Role và Booking Status
- [x] Cấu hình connection string MySQL
- [x] Kiểm tra và hiển thị trạng thái kết nối database khi application khởi động
- [x] Cập nhật MySQL credentials hợp lệ để startup check trả về `Database connected`
- [x] Hoàn thiện mapping, relationships và constraints cho models
- [x] Tạo EF6 migrations ban đầu
- [x] Tạo hoặc cập nhật schema MySQL bằng migrations
- [x] Tách repository/data service khỏi ViewModels
- [x] Giữ data access trong thư mục `Data` và `Services` của cùng project

## 5. Authentication And Roles

- [x] Tạo `IAuthService` và `AuthService`
- [x] Tạo Login và Register ViewModel
- [x] Tạo validation email và số điện thoại
- [x] Tạo role navigation strategy cho Admin, Guide và User
- [x] Kết nối form Login/Register với DI và `AuthService`
- [x] Ẩn Login/Register sau khi đăng nhập và ẩn navigation không đúng role
- [x] Thay mock `admin/admin` bằng truy vấn database
- [x] Lưu tài khoản đăng ký vào database
- [x] Thay SHA-256 bằng password hashing có salt như PBKDF2, bcrypt hoặc Argon2
- [x] Điều hướng tự động theo role sau khi đăng nhập
- [x] Quản lý phiên đăng nhập và logout

## 6. Admin Features

- [x] Tạo Admin Dashboard
- [x] Tạo Account Management UI và ViewModel
- [x] Tạo Content Management UI và ViewModel
- [x] Hiển thị danh sách tài khoản bằng dữ liệu thật
- [x] Tạo, sửa và xóa User/Guide trong database
- [x] Quản lý Destination và Hotel
- [x] Duyệt Hotel và nội dung do Guide tạo
- [x] Theo dõi và quản lý Booking

## 7. Guide Features

- [x] Tạo Guide Dashboard
- [x] Tạo Schedule Management ViewModel
- [x] Tạo Booking Requests ViewModel
- [x] Nối Schedule và Booking Requests vào Guide Dashboard
- [x] Lưu lịch trống vào database
- [x] Tạo và chỉnh sửa Destination
- [x] Tạo và chỉnh sửa Hotel
- [x] Chấp nhận hoặc từ chối Booking bằng dữ liệu thật

## 8. User Features

- [x] Tạo User Dashboard
- [x] Tạo Advanced Search ViewModel
- [x] Tạo Tour Booking ViewModel
- [x] Tạo Payment Simulation ViewModel
- [x] Nối search, booking và payment vào User Dashboard
- [x] Tìm kiếm Destination, Hotel và Guide từ database
- [x] Tạo và hủy Booking bằng dữ liệu thật
- [x] Hoàn thiện hồ sơ người dùng
- [x] Hoàn thiện yêu thích và đánh giá
- [x] Hoàn thiện quy trình thanh toán mô phỏng

## 9. Supporting Services

- [x] Tạo local Notification Queue
- [x] Tạo file Logger Service
- [x] Hiển thị Popup Notification trong application shell
- [x] Kết nối logging vào các luồng lỗi thực tế
- [~] Hoàn thiện Image Upload Service
- [ ] Lưu ảnh cục bộ hoặc trên storage thật
- [ ] Hoàn thiện Booking Service và pricing rules

## 10. Quality And Delivery

- [x] Project build thành công
- [ ] Xử lý toàn bộ compiler warnings
- [ ] Viết Unit Tests cho validation, authentication và booking
- [ ] Viết Integration Tests cho EF6/MySQL
- [ ] Viết smoke test cho các luồng UI chính
- [ ] Bổ sung README với hướng dẫn setup và build
- [x] Di chuyển database credentials khỏi `App.config`
- [ ] Thêm cấu hình theo environment
- [ ] Thực hiện security review
- [ ] Chuẩn bị release build và deployment package

## Project Architecture

Toàn bộ ứng dụng được phát triển trong một project `TravelApp`:

- `Views`: giao diện WPF
- `ViewModels`: state, binding và commands
- `Models`: entity và domain models
- `Data`: `DbContext`, EF6 configuration và migrations
- `Services`: authentication, booking, notification, image và logging
- `Utils`: constants, validation và helper dùng chung

Các lớp vẫn cần tách trách nhiệm rõ ràng, nhưng không cần tạo thêm project
Frontend, Backend, API hoặc Contracts trong solution.

## Current Milestone

Dự án đã hoàn thành foundation, build configuration, application shell và
skeleton giao diện theo kiến trúc monolith một project. Giai đoạn hiện tại là
hoàn thiện MVVM/DI wiring và thay dữ liệu mô phỏng bằng EF6/MySQL data thực
ngay trong project `TravelApp`.
