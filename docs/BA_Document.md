# Tài liệu Phân tích Nghiệp vụ (Business Analysis) - Dự án Airbnb Clone

## 1. Tầm nhìn & Mục tiêu Kinh doanh (Vision & Business Objectives)
- **Tầm nhìn:** Xây dựng một nền tảng kết nối trực tiếp giữa người có không gian cho thuê (Host) và người có nhu cầu thuê chỗ ở (Guest) một cách minh bạch, an toàn và tiện lợi.
- **Mục tiêu:** Cung cấp trải nghiệm tìm kiếm, đặt phòng và thanh toán liền mạch; đảm bảo độ tin cậy thông qua hệ thống đánh giá hai chiều; kiến trúc hệ thống sẵn sàng mở rộng (Microservices) để đáp ứng lượng truy cập lớn.

## 2. Các Bên Liên Quan (Stakeholders & Actor Profiles)
- **Guest (Khách thuê):** Người dùng tìm kiếm chỗ ở, thực hiện đặt phòng, thanh toán và để lại đánh giá.
- **Host (Chủ nhà):** Người dùng đăng tải chỗ ở, quản lý lịch trống, quản lý giá, và tương tác với Guest.
- **System Admin (Quản trị viên):** Quản lý hệ thống, xử lý tranh chấp, duyệt các listing vi phạm, và theo dõi dòng tiền/doanh thu.

## 3. Bản đồ Hành trình Người dùng (User Journeys)
### 3.1 Hành trình của Guest (Guest Journey)
1. **Khám phá:** Truy cập trang chủ -> Tìm kiếm theo địa điểm, thời gian, số khách -> Lọc kết quả (giá, tiện ích, loại phòng).
2. **Quyết định:** Xem chi tiết chỗ ở (hình ảnh, mô tả, đánh giá) -> Chọn ngày -> Xem tổng tiền.
3. **Giao dịch:** Đăng nhập/Đăng ký -> Thanh toán an toàn -> Nhận xác nhận đặt phòng (Email/In-app).
4. **Trải nghiệm & Hậu mãi:** Nhận phòng -> Trả phòng -> Để lại đánh giá (Review) cho Host.

### 3.2 Hành trình của Host (Host Journey)
1. **Gia nhập:** Đăng ký tài khoản Host -> Tạo Listing mới (nhập địa chỉ, up ảnh, tiện ích, quy định).
2. **Quản lý:** Cập nhật lịch trống (Calendar) -> Thiết lập giá linh hoạt.
3. **Vận hành:** Nhận yêu cầu đặt phòng -> Chấp nhận/Từ chối (hoặc Auto-book) -> Giao tiếp với Guest.
4. **Doanh thu:** Nhận thanh toán sau khi Guest check-in -> Rút tiền về tài khoản ngân hàng.

## 4. Yêu cầu Chức năng (Functional Requirements - Epics)

### Epic 1: Identity & Access Management (Quản lý Định danh)
- Đăng ký/Đăng nhập (Email, Phone).
- Quản lý hồ sơ cá nhân, đổi mật khẩu.
- Phân quyền theo Role (Guest, Host, Admin).

### Epic 2: Property Management (Quản lý Chỗ ở)
- Host tạo/sửa/xóa Listing (Thông tin cơ bản, Vị trí tọa độ, Hình ảnh, Tiện ích).
- Quản lý Calendar (Block ngày nghỉ, cập nhật giá theo ngày).

### Epic 3: Search & Discovery (Tìm kiếm & Khám phá)
- Tìm kiếm Text và Địa lý (Geo-search) kết hợp ngày tháng.
- Bộ lọc nâng cao (Khoảng giá, Loại phòng, Tiện ích).
- Hiển thị danh sách kết hợp bản đồ.

### Epic 4: Booking Engine (Hệ thống Đặt phòng)
- Tính toán giá tiền động (Thuế, Phí nền tảng, Phí dọn dẹp, Giảm giá).
- Cơ chế khóa chỗ (Lock inventory) khi đang trong phiên thanh toán (Concurrency Control chống Overbooking).
- Quản lý trạng thái Booking (Pending, Confirmed, Cancelled, Completed).

### Epic 5: Payment (Thanh toán)
- Tích hợp thanh toán an toàn qua cổng VNPay.
- Lưu trữ lịch sử giao dịch.

### Epic 6: Review & Rating (Đánh giá & Xếp hạng)
- Đánh giá 2 chiều (Guest đánh giá Host, Host đánh giá Guest).
- Chỉ cho phép đánh giá khi Booking đã ở trạng thái `Completed`.

## 5. Yêu cầu Phi chức năng (Non-Functional Requirements)
- **Scalability:** Hệ thống áp dụng Microservices để có thể scale độc lập dịch vụ Search (đọc nhiều) và Booking (ghi/xử lý logic phức tạp).
- **Performance:** Caching dữ liệu tĩnh bằng Redis để tăng tốc độ phản hồi.
- **Consistency:** Đảm bảo tính toàn vẹn dữ liệu cực cao khi đặt phòng (tránh Overbooking) bằng Transaction/Saga Pattern khi thanh toán.
- **Security:** API được bảo vệ bằng JWT Gateway Authentication.

## 6. Sơ lược Mô hình Thực thể Miền (Domain Model Overview)
Để đảm bảo tính Traceability và thiết kế CSDL (Prisma) sau này, chúng ta định nghĩa sớm các Core Entities:
- **`User`**: `id`, `email`, `role`, `profile_info`
- **`Property`**: `id`, `host_id`, `title`, `address`, `base_price`
- **`PropertyAvailability`**: `id`, `property_id`, `date`, `price`, `status` (available/booked)
- **`Booking`**: `id`, `guest_id`, `property_id`, `start_date`, `end_date`, `status`
- **`Review`**: `id`, `booking_id`, `reviewer_id`, `rating`, `comment`

---
> Bước tiếp theo: Dựa trên tài liệu BA này, chúng ta sẽ thiết kế System Architecture Diagram và lên Prisma Schema chuẩn chỉnh.
