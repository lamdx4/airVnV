# 🏡 AirVnV - Airbnb Clone 🚀

Chào mừng bạn đến với dự án **AirVnV**. Đây là hệ thống clone Airbnb hiện đại được thiết kế dựa trên kiến trúc **Microservices** hướng sự kiện (Event-Driven Architecture) kết hợp với sức mạnh điều phối của **.NET Aspire**.

*   **Repository chính thức:** [GitHub - airVnV](https://github.com/lamdx4/airVnV)

---

## 🏗️ Tổng quan Kiến trúc hệ thống

Hệ thống được phân rã thành 5 Service chính tương ứng với các nghiệp vụ cốt lõi:
*   **UserService**: Quản lý người dùng, xác thực, phân quyền.
*   **PropertyService**: Quản lý danh sách phòng, nhà ở, tiện ích.
*   **BookingService**: Xử lý đặt phòng, khóa lịch.
*   **PaymentService**: Xử lý giao dịch, cổng thanh toán an toàn.
*   **SearchService**: Tìm kiếm thông tin phòng theo thời gian thực bằng Elasticsearch.

Toàn bộ các dịch vụ giao tiếp thông qua **YARP API Gateway**, được kích hoạt luồng đồng bộ dữ liệu CDC qua **Kafka & Debezium**.

---

## ⚡ Hướng dẫn khởi động nhanh (Quick Start)

### 1. Yêu cầu cài đặt ban đầu
*   **.NET 9.0 SDK** trở lên.
*   **Node.js** v20+.
*   **Podman** hoặc **Docker Desktop** (Khuyên dùng Podman theo setup dự án).

### 2. Thao tác chạy local
Tại thư mục gốc của dự án, thực hiện chạy lệnh:
```bash
dotnet run --project Airbnb.AppHost
```
> [!TIP]
> Lệnh này sẽ tự động tải các Container Database, Kafka, ES và khởi động đồng loạt Microservices cùng Frontend Vite!

---

## 🤝 Quy trình Làm việc Nhóm (Team Collaboration)

Để giữ mã nguồn luôn sạch đẹp và tránh xung đột, toàn bộ team tuân thủ các quy tắc sau:
1. **Quy tắc đặt tên nhánh:** `feature/ten-tinh-nang` hoặc `bugfix/loi-can-sua`.
2. **Pull Request:** Bắt buộc phải Build pass locally trước khi mở PR vào `main`.

*👉 Xem tài liệu chi tiết cấu trúc Stack và luồng học tập tại [Team Onboarding Guide](./docs/Team_Onboarding.md)*
