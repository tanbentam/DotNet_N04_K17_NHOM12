# TravelApp Full Regression Test Flow

Tài liệu này dùng để kiểm thử thủ công toàn bộ chức năng hiện có của TravelApp
theo một luồng xuyên suốt. Mỗi lần test nên tạo dữ liệu mới bằng `RUN_ID` để
không trùng email, số điện thoại hoặc nội dung của lần test trước.

## 1. Phạm vi

Flow này kiểm tra:

- Khởi động ứng dụng và kết nối MySQL.
- Đăng ký, đăng nhập bằng email/số điện thoại, phân quyền và đăng xuất.
- Admin quản lý tài khoản, điểm đến, khách sạn và booking.
- Guide khai báo lịch trống, tạo nội dung, xử lý booking và xem lịch làm việc.
- User tìm kiếm, đặt tour, hủy booking, thanh toán mô phỏng.
- Guide gửi yêu cầu hủy và Admin duyệt hoặc từ chối.
- User cập nhật hồ sơ, yêu thích và đánh giá.
- Upload ảnh, popup notification, validation và các nhánh lỗi quan trọng.

Các mục chưa triển khai trong `CHECKLIST.md` như giới hạn đánh giá sau tour
không được xem là lỗi regression của phiên bản hiện tại.

## 2. Quy ước kết quả

Đánh dấu từng bước:

- `[ ]` Chưa test
- `[x]` Pass
- `[!]` Fail
- `[-]` Bỏ qua và ghi rõ lý do

Thông tin phiên test:

| Trường | Giá trị |
|---|---|
| Ngày test | |
| Người test | |
| Branch/commit | |
| Cấu hình | Debug / Release |
| Database | |
| RUN_ID | |
| Kết quả chung | Pass / Fail |

## 3. Chuẩn bị

### 3.1 Build

Từ thư mục gốc:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  TravelApp\TravelApp.csproj `
  /t:Rebuild `
  /p:Configuration=Debug `
  /restore
```

Kỳ vọng:

- Build thành công.
- Không có compiler error.
- File `TravelApp\bin\Debug\TravelApp.exe` được tạo.

### 3.2 Chạy ứng dụng

```powershell
.\TravelApp\bin\Debug\TravelApp.exe
```

### 3.3 Dữ liệu test

Chọn `RUN_ID` gồm 6 chữ số, ví dụ `130626`.

| Dữ liệu | Mẫu |
|---|---|
| User email | `user.RUN_ID@test.local` |
| User phone | `09RUN_ID00` |
| User password | `Test@123456` |
| Guide email | `guide.RUN_ID@test.local` |
| Guide phone | `08RUN_ID00` |
| Guide password | `Guide@123456` |
| User phụ email | `temp.RUN_ID@test.local` |
| User phụ phone | `07RUN_ID00` |
| Destination chính | `Da Lat RUN_ID` |
| Destination bị từ chối | `Rejected Place RUN_ID` |
| Hotel chính | `Travel Hotel RUN_ID` |
| Hotel bị từ chối | `Rejected Hotel RUN_ID` |

Số điện thoại phải đúng 10 chữ số. Nếu `RUN_ID` không có 6 chữ số, tự điều
chỉnh để số điện thoại vẫn đủ 10 chữ số và không trùng dữ liệu cũ.

Chuẩn bị thêm:

- Một tài khoản Admin đang hoạt động.
- Một ảnh PNG/JPG hợp lệ dưới 5 MB.
- Một file hỏng nhưng có đuôi `.png` để test validation ảnh, nếu cần.
- Ngày tour `D1`: một ngày trong tương lai ít nhất 14 ngày.
- Ngày tour `D2`: sau `D1` ít nhất 10 ngày.
- Ngày tour `D3`: sau `D2` ít nhất 10 ngày.

## 4. Ma trận booking

Tạo các booking sau để mỗi booking phục vụ một nhánh riêng:

| Mã ghi chú | Ngày | Số ngày | Mục đích | Kết quả cuối |
|---|---:|---:|---|---|
| BK-A | D1 | 3 | Happy path + thanh toán | Completed |
| BK-B | D2 | 2 | Guide từ chối | Rejected |
| BK-C | D2 | 2 | User tự hủy khi Pending | Cancelled |
| BK-D | D3 | 2 | Guide request hủy, Admin từ chối | Accepted |
| BK-E | D3 + 5 ngày | 2 | Guide request hủy, Admin duyệt | Cancelled |
| BK-F | Trùng D1 | 2 | Kiểm tra trùng lịch Guide | Pending hoặc Rejected |
| BK-G | Sau D3 ít nhất 10 ngày | 2 | Hủy sau thanh toán và hoàn tiền | Cancelled |

Ghi lại mã booking thật sau khi tạo:

| Ghi chú | Booking ID |
|---|---|
| BK-A | |
| BK-B | |
| BK-C | |
| BK-D | |
| BK-E | |
| BK-F | |
| BK-G | |

## 5. Startup, navigation và database

- [ ] Mở ứng dụng.
- [ ] Màn hình Home xuất hiện và không crash.
- [ ] Khu vực `DATABASE` chuyển từ `Checking connection...` sang
  `Cơ sở dữ liệu sẵn sàng`.
- [ ] Popup database xuất hiện.
- [ ] Khi chưa đăng nhập chỉ thấy Home, Login và Register.
- [ ] Các menu User, Guide, Admin, Account Management và Content Management
  chưa xuất hiện.
- [ ] Nút Exit đóng ứng dụng bình thường. Mở lại để tiếp tục test.

## 6. Authentication

### 6.1 Validation đăng ký

- [ ] Mở Register.
- [ ] Nhập email sai định dạng, nhấn Đăng ký.
  - Kỳ vọng: báo `Email không hợp lệ`.
- [ ] Nhập số điện thoại không đủ 10 chữ số.
  - Kỳ vọng: báo lỗi số điện thoại.
- [ ] Bỏ trống mật khẩu hoặc họ tên.
  - Kỳ vọng: yêu cầu điền đầy đủ.

### 6.2 Đăng ký User

- [ ] Nhập email, phone và password của User test.
- [ ] Trường `Họ và tên` nhập `Test User RUN_ID`.
- [ ] Nhấn Đăng ký.
  - Kỳ vọng: báo đăng ký thành công và form được xóa.

- [ ] Đăng ký lại cùng email hoặc phone.
  - Kỳ vọng: bị từ chối vì trùng dữ liệu.

### 6.3 Validation đăng nhập

- [ ] Mở Login và để trống một trường.
  - Kỳ vọng: yêu cầu nhập identifier và password.
- [ ] Nhập password sai.
  - Kỳ vọng: báo thông tin đăng nhập không chính xác.
- [ ] Đăng nhập User bằng email.
  - Kỳ vọng: tự chuyển đến User Dashboard.
- [ ] Kiểm tra Login/Register bị ẩn.
- [ ] Chỉ menu User Dashboard xuất hiện trong nhóm role.
- [ ] Logout.
- [ ] Đăng nhập lại bằng số điện thoại.
  - Kỳ vọng: đăng nhập thành công.
- [ ] Logout để tiếp tục flow Admin.

## 7. Admin - Account Management

Đăng nhập bằng tài khoản Admin.

- [ ] Tự chuyển đến Admin Dashboard.
- [ ] Chỉ thấy menu Admin Dashboard, Account Management và Content Management.
- [ ] Dashboard hiển thị tên Admin và thời điểm cập nhật gần nhất.
- [ ] Các KPI tài khoản, User, Guide, nội dung chờ duyệt, booking chờ,
  yêu cầu hủy, tour đã thanh toán và doanh thu mô phỏng tải thành công.
- [ ] Đối chiếu nhanh KPI với dữ liệu hiện có trong Account Management và
  Content Management.
- [ ] Bảng `Booking cần chú ý` ưu tiên yêu cầu hủy đang chờ, sau đó hiển thị
  booking `Pending` và `Paid`.
- [ ] Nút `Làm mới` tải lại KPI và cập nhật thời gian.
- [ ] Trong Admin Dashboard, nút Account Management chuyển sang đúng màn hình,
  cập nhật tiêu đề và highlight menu Account Management.
- [ ] Nút Content Management chuyển sang đúng màn hình, cập nhật tiêu đề và
  highlight menu Content Management.
- [ ] Bấm menu Admin Dashboard để quay lại tổng quan và tải dữ liệu mới nhất.
- [ ] Nút Logout kết thúc phiên và quay về Login. Đăng nhập lại Admin để tiếp tục.
- [ ] Mở Account Management.
- [ ] Danh sách tài khoản và tổng số account tải thành công.

### 7.1 Tạo Guide

- [ ] Nhấn `Create Guide Account`.
- [ ] Bỏ trống họ tên rồi Save.
  - Kỳ vọng: validation không cho lưu.
- [ ] Nhập dữ liệu Guide test và Save.
  - Kỳ vọng: tạo thành công, account xuất hiện với role `TourGuide`.

### 7.2 Tạo, sửa và xóa User phụ

- [ ] Nhấn `Create User Account`.
- [ ] Tạo User phụ bằng dữ liệu `temp.RUN_ID`.
- [ ] Chọn User phụ và nhấn Edit.
- [ ] Sửa họ tên thành `Temporary User Edited RUN_ID`.
- [ ] Để password trống và Save.
  - Kỳ vọng: thông tin đổi, password cũ vẫn giữ nguyên.
- [ ] Edit lại và nhập password mới `Temp@654321`.
  - Kỳ vọng: lưu thành công.
- [ ] Thử đổi email/phone thành giá trị đang thuộc User test.
  - Kỳ vọng: bị từ chối do trùng.
- [ ] Xóa User phụ.
  - Kỳ vọng: xóa thành công vì chưa có dữ liệu phụ thuộc.
- [ ] Thử Edit/Delete tài khoản Admin.
  - Kỳ vọng: ứng dụng từ chối.

## 8. Guide - lịch trống và nội dung

Logout Admin, đăng nhập Guide test.

### 8.1 Lịch trống

- [ ] Mở tab `LỊCH TRỐNG`.
- [ ] Đánh dấu cả 7 ngày là có thể dẫn tour.
- [ ] Nhập khung giờ `08:00 - 18:00` cho từng ngày.
- [ ] Nhấn Lưu lịch.
  - Kỳ vọng: popup thành công.
- [ ] Logout và đăng nhập lại Guide.
- [ ] Mở lại lịch.
  - Kỳ vọng: dữ liệu vừa lưu được tải từ database.

### 8.2 Tạo Destination chính

- [ ] Mở tab `NỘI DUNG`, nhấn thêm điểm đến.
- [ ] Thử lưu khi thiếu tên hoặc quốc gia.
  - Kỳ vọng: báo validation.
- [ ] Chọn ảnh hợp lệ.
  - Kỳ vọng: ảnh được copy vào `%LocalAppData%\TravelApp\Images\Destination`.
- [ ] Nhập:
  - Tên: `Da Lat RUN_ID`
  - Quốc gia: `Vietnam`
  - Mô tả: `Regression destination RUN_ID`
- [ ] Lưu.
  - Kỳ vọng: Destination có trạng thái `Pending`.

### 8.3 Test ảnh lỗi

- [ ] Mở tạo Destination khác và chọn file ảnh hỏng hoặc trên 5 MB.
  - Kỳ vọng: báo file không hợp lệ hoặc quá 5 MB.
- [ ] Hủy editor, không tạo dữ liệu rác.

### 8.4 Tạo Destination để test Reject

- [ ] Tạo `Rejected Place RUN_ID`.
  - Kỳ vọng: trạng thái `Pending`.

## 9. Admin - duyệt Destination

Logout Guide, đăng nhập Admin, mở Content Management → Destinations.

- [ ] Hai Destination mới xuất hiện và có người tạo là Guide test.
- [ ] Approve `Da Lat RUN_ID`.
  - Kỳ vọng: trạng thái `Approved`.
- [ ] Reject `Rejected Place RUN_ID`.
  - Kỳ vọng: trạng thái `Rejected`.
- [ ] User chỉ có thể tìm thấy Destination đã duyệt.

### 9.1 Admin CRUD Destination

- [ ] Tạo một Destination tạm `Admin Temp Place RUN_ID`.
- [ ] Edit tên hoặc mô tả và Save.
- [ ] Delete Destination tạm trước khi tạo Hotel/Booking liên quan.
  - Kỳ vọng: xóa thành công.
- [ ] Thử nhập rating ngoài khoảng `0 - 9.99`.
  - Kỳ vọng: validation từ chối.

## 10. Guide - tạo Hotel và sửa nội dung

Logout Admin, đăng nhập Guide.

### 10.1 Tạo Hotel chính

- [ ] Mở tab `KHÁCH SẠN`.
- [ ] Danh sách Destination chọn được có `Da Lat RUN_ID`.
- [ ] Nhấn thêm khách sạn.
- [ ] Thử giá âm hoặc rating lớn hơn 5.
  - Kỳ vọng: validation từ chối.
- [ ] Chọn ảnh hợp lệ.
- [ ] Nhập:
  - Destination: `Da Lat RUN_ID`
  - Tên: `Travel Hotel RUN_ID`
  - Địa chỉ: `01 Test Street`
  - Mô tả: `Regression hotel RUN_ID`
  - Giá/đêm: `800000`
  - Rating: `4`
- [ ] Lưu.
  - Kỳ vọng: Hotel có trạng thái `Pending`.

### 10.2 Tạo Hotel để test Reject

- [ ] Tạo `Rejected Hotel RUN_ID` với dữ liệu hợp lệ.
  - Kỳ vọng: trạng thái `Pending`.

### 10.3 Sửa Destination

- [ ] Sửa mô tả `Da Lat RUN_ID`.
  - Kỳ vọng: nội dung quay lại `Pending` để Admin duyệt lại.

## 11. Admin - duyệt Hotel và nội dung sửa

Logout Guide, đăng nhập Admin, mở Content Management.

- [ ] Re-approve `Da Lat RUN_ID`.
- [ ] Approve `Travel Hotel RUN_ID`.
- [ ] Reject `Rejected Hotel RUN_ID`.
- [ ] Edit Hotel chính, giữ giá `800000`, Save.
- [ ] Thử rating lớn hơn 5 hoặc giá âm.
  - Kỳ vọng: validation từ chối.
- [ ] Hotel bị Reject không xuất hiện trong tìm kiếm User.
- [ ] Không xóa Destination chính khi đang có Hotel.
  - Kỳ vọng: hệ thống từ chối.

## 12. User - hồ sơ và tìm kiếm

Logout Admin, đăng nhập User test.

### 12.1 Hồ sơ

- [ ] Mở `HỒ SƠ CỦA TÔI`.
- [ ] Đổi họ tên thành `Regression User RUN_ID`.
- [ ] Save.
  - Kỳ vọng: popup thành công và tên trên header được cập nhật.
- [ ] Nhập email sai định dạng.
  - Kỳ vọng: validation từ chối.
- [ ] Nhập phone không đủ 10 số.
  - Kỳ vọng: validation từ chối.
- [ ] Nhấn Hoàn tác.
  - Kỳ vọng: dữ liệu quay lại giá trị đang lưu.

### 12.2 Search

- [ ] Mở `KHÁM PHÁ & ĐẶT TOUR`.
- [ ] Search không nhập bộ lọc.
  - Kỳ vọng: tải Destination, Hotel và Guide đã được duyệt/hợp lệ.
- [ ] Search theo `Da Lat RUN_ID`.
  - Kỳ vọng: thấy Destination và Hotel chính.
- [ ] Search theo tên Guide test.
  - Kỳ vọng: thấy Guide.
- [ ] Search thời gian `Thứ 2` hoặc `08:00`.
  - Kỳ vọng: Guide test xuất hiện do lịch trống đã lưu.
- [ ] Nhập giá tối đa âm.
  - Kỳ vọng: báo giá không được âm.
- [ ] Nhập rating lớn hơn 5.
  - Kỳ vọng: validation từ chối.
- [ ] Xác nhận Destination/Hotel bị Reject không xuất hiện.

## 13. User - giá tour và tạo booking

Chọn Destination chính, Hotel chính và Guide test.

### 13.1 Pricing

- [ ] Nhập 3 ngày.
- [ ] Kiểm tra giá:
  - Guide: `500,000 x 3 = 1,500,000`
  - Hotel: `800,000 x 3 = 2,400,000`
  - Giảm giá: `0`
  - Phí dịch vụ: `195,000`
  - Tổng: `4,095,000`
- [ ] Nhập 7 ngày.
  - Kỳ vọng: có giảm giá 10% trước khi tính phí dịch vụ 5%.
- [ ] Nhập 0 hoặc 31 ngày.
  - Kỳ vọng: validation từ chối.
- [ ] Chọn ngày quá khứ hoặc quá 365 ngày.
  - Kỳ vọng: validation từ chối.

### 13.2 Tạo BK-A

- [ ] Chọn ngày D1 và 3 ngày, gửi yêu cầu.
- [ ] Ghi lại Booking ID vào bảng BK-A.
- [ ] Mở `BOOKING CỦA TÔI`.
  - Kỳ vọng: BK-A có trạng thái `Pending`.

### 13.3 Tạo các booking nhánh

- [ ] Tạo BK-B tại D2, 2 ngày.
- [ ] Tạo BK-C tại ngày không trùng BK-A/B, 2 ngày.
- [ ] Tạo BK-D tại D3, 2 ngày.
- [ ] Tạo BK-E tại D3 + 5 ngày, 2 ngày.
- [ ] Tạo BK-F trùng thời gian BK-A.
- [ ] Tạo BK-G ở ngày không trùng các booking khác.
- [ ] Ghi lại toàn bộ Booking ID.

### 13.4 User hủy BK-C

- [ ] Trong `BOOKING CỦA TÔI`, nhấn Hủy BK-C.
  - Kỳ vọng: trạng thái thành `Cancelled`.
- [ ] Thử hủy lại BK-C.
  - Kỳ vọng: hệ thống từ chối.

## 14. Guide - xử lý booking

Logout User, đăng nhập Guide, mở tab `BOOKING`.

- [ ] Các booking Pending trừ BK-C xuất hiện.
- [ ] Reject BK-B.
  - Kỳ vọng: biến mất khỏi danh sách Pending.
- [ ] Accept BK-A.
  - Kỳ vọng: thành công và xuất hiện trong `LỊCH LÀM VIỆC`.
- [ ] Accept BK-F đang trùng BK-A.
  - Kỳ vọng: bị từ chối do trùng lịch.
- [ ] Accept BK-D, BK-E và BK-G.
  - Kỳ vọng: thành công nếu không trùng các tour khác.
- [ ] Trong lịch làm việc, kiểm tra:
  - Có BK-A, BK-D và BK-E.
  - Ngày hoàn thành = ngày bắt đầu + số ngày - 1.
  - Tour tương lai có tiến độ `Sắp diễn ra`.
  - Thông tin User, Destination và Hotel đúng.

## 15. User - kiểm tra trạng thái và thanh toán

Logout Guide, đăng nhập User.

- [ ] Làm mới `BOOKING CỦA TÔI`.
- [ ] BK-A, BK-D, BK-E là `Accepted`.
- [ ] BK-B là `Rejected`.
- [ ] BK-C là `Cancelled`.
- [ ] BK-F vẫn Pending nếu Guide chưa Reject.

### 15.1 Thanh toán thất bại

- [ ] Mở `THANH TOÁN`.
- [ ] Chọn BK-A.
- [ ] Chuẩn bị QR.
- [ ] Chọn kết quả mô phỏng thất bại.
- [ ] Xác nhận.
  - Kỳ vọng: giao dịch `Failed` được lưu trong lịch sử.
  - BK-A vẫn `Accepted` và còn trong danh sách có thể thanh toán.

### 15.2 Thanh toán thành công

- [ ] Chọn lại BK-A.
- [ ] Chuẩn bị chuyển khoản.
- [ ] Không nhập mã tham chiếu và xác nhận.
  - Kỳ vọng: yêu cầu nhập mã tham chiếu.
- [ ] Nhập `REF-RUN_ID` và chọn kết quả thành công.
- [ ] Xác nhận.
  - Kỳ vọng:
  - Giao dịch `Successful` xuất hiện trong lịch sử.
  - BK-A chuyển sang `Paid`.
  - BK-A biến mất khỏi danh sách có thể thanh toán.
  - Popup thanh toán thành công xuất hiện.
- [ ] Làm mới Booking của tôi và xác nhận BK-A là `Paid`.

### 15.3 Thanh toán và yêu cầu hoàn tiền BK-G

- [ ] Thanh toán thành công BK-G để booking chuyển sang `Paid`.
- [ ] Trong `BOOKING CỦA TÔI`, để trống lý do và nhấn `Yêu cầu hoàn tiền`.
  - Kỳ vọng: bị từ chối vì lý do phải từ 10 đến 500 ký tự.
- [ ] Nhập lý do hợp lệ và gửi lại.
  - Kỳ vọng: trạng thái hoàn tiền là `Đang chờ duyệt`; booking vẫn `Paid`.
- [ ] Gửi lại khi request đang chờ.
  - Kỳ vọng: bị từ chối vì request chưa được Admin xử lý.

## 16. Guide cancellation request

### 16.1 Admin từ chối request của BK-D

Logout User, đăng nhập Guide, mở `LỊCH LÀM VIỆC`.

- [ ] Chọn BK-D.
- [ ] Nhập lý do dưới 10 ký tự.
  - Kỳ vọng: bị từ chối.
- [ ] Nhập `Guide bận đột xuất trong ngày kiểm thử RUN_ID`.
- [ ] Gửi yêu cầu hủy.
  - Kỳ vọng: tiến độ thành `Đang chờ duyệt hủy`.
- [ ] Không thể gửi request lần hai khi đang chờ.

Logout Guide, đăng nhập User:

- [ ] Mở Thanh toán.
- [ ] BK-D không xuất hiện trong danh sách có thể thanh toán khi request đang chờ.

Logout User, đăng nhập Admin, mở Content Management → Bookings:

- [ ] Chọn BK-D.
- [ ] Thấy `Cancel request = True`, lý do và thời gian gửi.
- [ ] Nhấn `Reject request`.
  - Kỳ vọng: request được giải quyết, BK-D vẫn `Accepted`.

Logout Admin, đăng nhập User:

- [ ] Làm mới Thanh toán.
- [ ] BK-D xuất hiện trở lại và có thể thanh toán.

### 16.2 Admin duyệt request của BK-E

Logout User, đăng nhập Guide:

- [ ] Chọn BK-E và gửi lý do hợp lệ.

Logout Guide, đăng nhập Admin:

- [ ] Chọn BK-E, kiểm tra đúng lý do.
- [ ] Nhấn `Approve cancel`.
  - Kỳ vọng: BK-E chuyển sang `Cancelled`.

Logout Admin, đăng nhập Guide:

- [ ] Làm mới lịch làm việc.
- [ ] BK-E không còn trong lịch.

Logout Guide, đăng nhập User:

- [ ] BK-E là `Cancelled`.
- [ ] BK-E không xuất hiện trong danh sách thanh toán.

## 17. Admin - quản lý trạng thái booking

Đăng nhập Admin, mở Content Management → Bookings.

- [ ] Chọn BK-G có yêu cầu hoàn tiền đang chờ.
- [ ] Kiểm tra lý do và thời gian gửi request hiển thị đúng.
- [ ] Nhấn `Approve refund`.
  - Kỳ vọng: BK-G chuyển sang `Cancelled`, trạng thái hoàn tiền là `Đã hoàn tiền`.
- [ ] Chọn BK-A đang `Paid` và chuyển thủ công sang `Completed` để tiếp tục
  regression mà không phải chờ ngày tour.
  - Kỳ vọng: cập nhật thành công; thao tác thủ công vẫn được hỗ trợ.
- [ ] Thử chuyển `Completed` về trạng thái khác.
  - Kỳ vọng: bị từ chối.
- [ ] Thử chuyển BK-B từ `Rejected` sang trạng thái khác.
  - Kỳ vọng: bị từ chối.
- [ ] Chọn booking có request hủy đang chờ, nếu còn.
- [ ] Thử chuyển sang `Paid` trước khi xử lý request.
  - Kỳ vọng: yêu cầu xử lý request hủy trước.

Ứng dụng tự quét booking sau khi database báo `Cơ sở dữ liệu sẵn sàng`. Booking `Paid`
được chuyển sang `Completed` khi ngày hiện tại đã qua ngày hoàn thành tour;
booking có yêu cầu hoàn tiền đang chờ sẽ được bỏ qua. Luồng trên vẫn chuyển
BK-A thủ công để regression có thể chạy trong một phiên.

Để kiểm tra riêng auto-complete trên database test:

- [ ] Tạo và thanh toán một booking test để có trạng thái `Paid`.
- [ ] Đóng ứng dụng, chỉnh `StartDate` của booking test thành ngày quá khứ sao
  cho ngày hoàn thành nhỏ hơn hôm nay.
- [ ] Mở lại ứng dụng.
  - Kỳ vọng: popup báo số tour đã cập nhật và booking tự thành `Completed`.
- [ ] Lặp lại với booking `Paid` có yêu cầu hoàn tiền đang chờ.
  - Kỳ vọng: booking vẫn `Paid` để Admin xử lý yêu cầu.

## 18. User - yêu thích và đánh giá

Logout Admin, đăng nhập User, mở `YÊU THÍCH & ĐÁNH GIÁ`.

- [ ] Mở tab `THANH TOÁN` sau khi đăng nhập lại.
  - Kỳ vọng: giao dịch của BK-G có trạng thái `Refunded`.
- [ ] Mở `BOOKING CỦA TÔI`.
  - Kỳ vọng: BK-G là `Cancelled` và hiển thị `Đã hoàn tiền`.

### 18.1 Yêu thích

- [ ] Chọn Hotel chính và nhấn Thêm yêu thích.
  - Kỳ vọng: xuất hiện trong danh sách yêu thích.
- [ ] Thêm lại cùng Hotel.
  - Kỳ vọng: bị từ chối vì đã tồn tại.
- [ ] Chọn Guide test và thêm yêu thích.
- [ ] Bỏ thích Hotel.
  - Kỳ vọng: Hotel biến mất khỏi danh sách.
- [ ] Nhấn Làm mới.
  - Kỳ vọng: dữ liệu đã lưu vẫn đúng.

### 18.2 Đánh giá

- [ ] Chọn Hotel chính, rating 5, nhập comment và lưu.
- [ ] Chọn Guide test, rating 4, nhập comment và lưu.
- [ ] Hai đánh giá xuất hiện trong danh sách.
- [ ] Lưu lại đánh giá cùng Hotel với rating/comment khác.
  - Kỳ vọng: cập nhật review cũ, không tạo bản trùng.
- [ ] Nhập rating 0 hoặc 6.
  - Kỳ vọng: validation từ chối.
- [ ] Xóa một review.
  - Kỳ vọng: review biến mất sau khi refresh.

Lưu ý: phiên bản hiện tại cho phép đánh giá nội dung đã duyệt dù chưa kiểm tra
booking `Completed`. Khi chức năng giới hạn đánh giá được triển khai, cập nhật
kỳ vọng của phần này.

## 19. Admin - ràng buộc xóa dữ liệu

Đăng nhập Admin.

- [ ] Thử xóa Hotel chính đã có booking.
  - Kỳ vọng: bị từ chối.
- [ ] Thử xóa Destination chính đã có Hotel/Booking.
  - Kỳ vọng: bị từ chối.
- [ ] Thử xóa User hoặc Guide đã có booking.
  - Kỳ vọng: bị từ chối do quan hệ database.
- [ ] Tạo một Hotel tạm không có booking rồi xóa.
  - Kỳ vọng: xóa thành công.
- [ ] Tạo một Destination tạm không có Hotel/Booking rồi xóa.
  - Kỳ vọng: xóa thành công.

## 20. Session, role và notification

- [ ] Khi đăng nhập User, chỉ navigation User hiển thị.
- [ ] Khi đăng nhập Guide, chỉ navigation Guide hiển thị.
- [ ] Khi đăng nhập Admin, chỉ navigation Admin hiển thị.
- [ ] Logout luôn quay về Login và ẩn menu role.
- [ ] Header hiển thị đúng `FullName (Role)`.
- [ ] Popup xuất hiện cho các thao tác chính:
  - Lưu lịch Guide.
  - Tạo booking.
  - Hủy booking.
  - Chấp nhận/từ chối booking.
  - Thanh toán thành công/thất bại.
  - Cập nhật hồ sơ.
  - Lưu đánh giá.
  - Gửi yêu cầu hủy.

## 21. Persistence

- [ ] Đóng hoàn toàn ứng dụng.
- [ ] Mở lại và đăng nhập từng role.
- [ ] Xác nhận các dữ liệu sau vẫn còn:
  - Account User và Guide.
  - Lịch trống Guide.
  - Destination và Hotel.
  - Booking cùng trạng thái cuối.
  - Payment history.
  - Favorites và reviews.
  - Hồ sơ User đã cập nhật.

## 22. Log và file ảnh

- [ ] Sau một lần đăng nhập sai hoặc booking bị từ chối, kiểm tra:

```text
TravelApp\bin\Debug\TravelApp_ErrorLog.txt
```

- [ ] Log có entry `LOGIN_FAILURE`, `BOOKING_FAILURE`, `WARNING` hoặc `ERROR`
  tương ứng với thao tác vừa test.
- [ ] Log không chứa password dạng rõ.
- [ ] Ảnh upload tồn tại tại:

```text
%LocalAppData%\TravelApp\Images\Destination
%LocalAppData%\TravelApp\Images\Hotel
```

- [ ] Upload lại cùng một file không tạo nhiều bản sao khác hash.

## 23. Cleanup sau test

Do nhiều entity có khóa ngoại, cleanup nên theo thứ tự:

1. Giữ lại booking/payment nếu cần làm bằng chứng test.
2. Chỉ xóa Hotel không có booking.
3. Chỉ xóa Destination không có Hotel/Booking.
4. Chỉ xóa User/Guide không có booking, favorite, review hoặc payment.

Nếu muốn chạy regression thường xuyên, nên dùng database test riêng và reset
database đó thay vì xóa thủ công trên database phát triển.

## 24. Các negative test nhanh

Chạy phần này khi sửa validation hoặc booking service:

- [ ] Đăng nhập sai password.
- [ ] Đăng ký trùng email/phone.
- [ ] User chọn Hotel không thuộc Destination.
- [ ] Booking 0 ngày, 31 ngày, ngày quá khứ, quá 365 ngày.
- [ ] Guide không rảnh đủ tất cả ngày của tour.
- [ ] Hai booking Accepted/Paid trùng lịch Guide.
- [ ] Hai booking Accepted/Paid trùng lịch User.
- [ ] User hủy booking Paid.
- [ ] Guide request hủy booking Paid.
- [ ] Guide request hủy với lý do dưới 10 hoặc trên 500 ký tự.
- [ ] User thanh toán khi request hủy đang chờ.
- [ ] User yêu cầu hoàn tiền booking `Paid` với lý do dưới 10 hoặc trên 500 ký tự.
- [ ] User yêu cầu hoàn tiền khi tour đã bắt đầu.
- [ ] Admin cập nhật trạng thái booking khi request hoàn tiền đang chờ.
- [ ] Admin cập nhật trạng thái không hợp lệ.
- [ ] Upload file không phải ảnh hoặc ảnh trên 5 MB.
- [ ] Rating Hotel ngoài 0-5.
- [ ] Rating review ngoài 1-5.
- [ ] Giá Hotel âm.
- [ ] Xóa entity đang được booking tham chiếu.

## 25. Báo cáo lỗi

Mỗi lỗi ghi theo mẫu:

```text
ID:
Ngày:
Phiên bản/commit:
Role:
Màn hình:
Dữ liệu sử dụng:
Các bước tái hiện:
Kết quả thực tế:
Kết quả mong đợi:
Ảnh/video:
Mã lỗi hoặc log:
Mức độ: Blocker / High / Medium / Low
```

## 26. Kết quả tổng hợp

| Khu vực | Pass | Fail | Skip | Ghi chú |
|---|---:|---:|---:|---|
| Startup/Navigation | | | | |
| Authentication | | | | |
| Admin Accounts | | | | |
| Guide Content/Schedule | | | | |
| Admin Content | | | | |
| User Search/Booking | | | | |
| Guide Booking/Work Schedule | | | | |
| Payment | | | | |
| Cancellation Request | | | | |
| Favorites/Reviews/Profile | | | | |
| Persistence/Logs/Images | | | | |

Điều kiện pass toàn bộ:

- Không có crash hoặc lỗi database ngoài dự kiến.
- Happy path kết thúc với BK-A ở trạng thái `Completed`.
- Các nhánh Rejected/Cancelled/request hủy đúng trạng thái.
- Dữ liệu vẫn tồn tại sau khi khởi động lại.
- Không có lỗi High hoặc Blocker chưa xử lý.
