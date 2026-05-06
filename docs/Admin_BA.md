# Tài liệu Phân tích Nghiệp vụ (BA) - Phân hệ Administrator

Phân hệ Administrator (Quản trị viên) đóng vai trò là "người gác cổng" và "trọng tài" của nền tảng Airbnb. Mục tiêu cốt lõi của Admin là duy trì chất lượng nguồn cung (Property), đảm bảo an toàn cho người dùng (User Verification), và điều phối dòng tiền (Finance & Payout).

Dưới đây là phân tích chi tiết scope công việc để team của anh có thể dựa vào đây lên kế hoạch phát triển.

---

## 1. Hành trình Người dùng của Admin (Admin Journey)

Không giống như Guest hay Host (hướng tới conversion), hành trình của Admin mang tính chất **Vận hành (Operations)** và **Xử lý sự cố (Resolution)**:

1. **Daily Monitoring (Theo dõi hàng ngày):** Đăng nhập -> Xem Dashboard tổng quan (Doanh thu, số lượng booking mới, cảnh báo bất thường).
2. **Moderation (Kiểm duyệt):** Mở hàng đợi (Queue) kiểm duyệt -> Xem xét các chỗ ở mới được Host submit -> Check hình ảnh, giấy tờ -> Approve (Duyệt) hoặc Reject (Từ chối kèm lý do).
3. **Dispute & Support (Hỗ trợ & Tranh chấp):** Tiếp nhận Ticket khiếu nại từ Guest/Host -> Yêu cầu cung cấp bằng chứng (ảnh chụp, tin nhắn) -> Ra quyết định hoàn tiền hoặc bồi thường.
4. **Finance (Kế toán):** Cuối tuần/cuối tháng -> Rà soát các Booking đã hoàn thành -> Chốt đối soát doanh thu -> Chuyển tiền (Payout) cho Host sau khi trừ phí nền tảng (Platform Fee).

---

## 2. Các Usecase & Yêu cầu Chức năng (Functional Epics)

Để team dễ chia task, phân hệ Admin được chia làm 5 Epic chính:

### Epic A: Quản trị Tài khoản & Danh tính (User & Identity Management)
- **UC-A1: Danh sách Người dùng:** Xem danh sách, tìm kiếm, lọc theo Role (Host/Guest), Trạng thái (Active, Suspended).
- **UC-A2: Khóa/Mở khóa Tài khoản:** Đình chỉ tài khoản (Ban/Suspend) nếu có hành vi vi phạm tiêu chuẩn cộng đồng, kèm theo gửi email thông báo.
- **UC-A3: Xét duyệt KYC (Identity Verification):** Cấp dấu tích xanh (Verified) cho Host sau khi nhân viên Admin kiểm tra ảnh chụp CCCD/Passport.

### Epic B: Quản lý & Kiểm duyệt Chỗ ở (Property Moderation)
*(Ghi chú: Backend PropertyService đã có sẵn một số API hỗ trợ phần này)*
- **UC-B1: Hàng đợi Duyệt Chỗ ở (Approval Queue):** Danh sách các Listing đang ở trạng thái `Pending`.
- **UC-B2: Chi tiết Kiểm duyệt:** Xem toàn bộ thông tin Listing (Vị trí, mô tả, hình ảnh, tiện ích).
- **UC-B3: Cấp phép/Từ chối:** Đổi trạng thái Listing sang `Active` (cho phép hiện trên trang tìm kiếm) hoặc `Rejected`.
- **UC-B4: Gỡ bỏ khẩn cấp:** Xóa (Delete) hoặc Tạm đình chỉ (Suspend) chỗ ở đang hoạt động nếu có report rủi ro cao từ Guest.

### Epic C: Tài chính & Đối soát (Finance & Payout)
- **UC-C1: Lịch sử Giao dịch:** Xem luồng tiền vào (Pay-in) từ Guest cho các Booking.
- **UC-C2: Quản lý Payout (Thanh toán cho Host):** Hiển thị danh sách các khoản tiền chờ thanh toán cho Host (Dựa trên Booking đã Check-out thành công).
- **UC-C3: Cấu hình Phí Nền tảng (Platform Fee):** Cho phép Admin cấp cao thay đổi % hoa hồng thu của Host/Guest.

### Epic D: Trung tâm Giải quyết Tranh chấp (Resolution Center)
- **UC-D1: Quản lý Support Tickets:** Tạo, xem, gán (assign) ticket khiếu nại cho nhân viên Admin cụ thể.
- **UC-D2: Refund Management:** Tính năng buộc hoàn tiền (Force Refund) một phần hoặc toàn bộ cho Guest thông qua Cổng thanh toán, bypass chính sách hủy phòng của Host nếu Host vi phạm nghiêm trọng (ví dụ: chỗ ở không đúng thực tế).

### Epic E: Dashboard & Báo cáo (Analytics)
- **UC-E1: Tổng quan (Overview):** Biểu đồ doanh thu ròng (Net Revenue), Tổng giao dịch (GMV).
- **UC-E2: Chỉ số Hệ thống:** Số lượng người dùng mới, tỷ lệ lấp phòng trung bình (Occupancy Rate).

---

## 3. Đề xuất luồng làm việc cho Team (Execution Plan)

Vì hệ thống đang sử dụng Microservices, team làm Admin cần giao tiếp chéo với nhiều service. Em đề xuất thứ tự triển khai cho team anh như sau:

- **Phase 1 (Dễ nhất):** Dựng Layout Dashboard (có thể dùng Template Ant Design, Shadcn) + Làm **Epic B (Kiểm duyệt Property)** vì API backend gần như đã đầy đủ.
- **Phase 2:** Làm **Epic A (User Management)**.
- **Phase 3:** Làm **Epic E (Dashboard)** kết hợp lấy data từ Elasticsearch / Redis.
- **Phase 4 (Khó nhất):** Làm **Epic C & D (Tài chính và Tranh chấp)**. Bắt buộc đợi team Core (em và anh) hoàn thiện Booking Engine và Payment Gateway thì mới có số liệu thực tế để đối soát và Refund.
