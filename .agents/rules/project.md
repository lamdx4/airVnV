---
trigger: always_on
---

# 🌐 PROJECT RULES – ARCHITECTURE & DEVOPS

Quy chuẩn cấp cao về kiến trúc hệ thống và luồng dữ liệu phân tán.

---

## 🏛️ 1. Microservices Boundary

- **Database-per-Microservice:** Mỗi service quản lý DB riêng. Tuyệt đối không query chéo database.
- **Data Reference:** Chỉ tham chiếu dữ liệu qua ID (Guid).
- **Service Discovery:** Sử dụng **.NET Aspire**. Không gán cứng (hardcode) chuỗi kết nối hoặc URL service.

---

## ⚔️ 2. Distributed Data Flow

- **CDC (Change Data Capture):** Sử dụng **Debezium** để đồng bộ dữ liệu từ Postgres sang Elasticsearch (Search Service).
- **Event-Driven Architecture:** Giao tiếp bất đồng bộ qua **Kafka**.
- **Transactional Outbox:** Bắt buộc lưu Event vào bảng Outbox cùng Transaction với nghiệp vụ chính. 
- **Idempotency:** Mọi Consumer phải có cơ chế chống trùng lặp (ví dụ: bảng `ProcessedEvent`).

---

## 🛡️ 3. Security & Infrastructure

- **API Gateway:** Mọi traffic từ Frontend phải đi qua **YARP Gateway**.
- **Authentication:** Sử dụng **JWT Bearer**. 
- **Logging:** Sử dụng **Structured Logging** (ILogger). Cấm dùng `Console.WriteLine()`.
- **Database Migrations:** Mọi thay đổi schema phải thực hiện qua **EF Core Migrations**. Không sửa DB bằng tay.

---

## 🚀 4. Workflow

- **CI/CD Readiness:** Luôn đảm bảo dự án build được ở chế độ **Native AOT**.
- **Team Workflow:** Tuân thủ quy trình Review PR. Không tự ý merge vào `main`.
