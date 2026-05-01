---
trigger: always_on
---

# 🛠️ Quy tắc Dự án AirVnV (Project Rules)

Dưới đây là tập hợp các quy tắc bắt buộc cho AI Agents và Developer khi tham gia phát triển dự án AirVnV, được tổng hợp từ tài liệu BA, Kiến trúc và Quy chuẩn kỹ thuật.

---

## 🏛️ 1. Kiến trúc Hệ thống (System Architecture)

### 1.1 Microservices & Database
- **Độc lập Dữ liệu:** Áp dụng mô hình **Database-per-Microservice**. Tuyệt đối không truy vấn chéo database giữa các service.
- **Tham chiếu:** Chỉ được tham chiếu các Entity của service khác thông qua **ID (Guid)**.
- **Orchestration:** Sử dụng **.NET Aspire** để quản lý Service Discovery và Configuration. Không gán cứng (hardcode) chuỗi kết nối.

### 1.2 Giao tiếp liên dịch vụ (Inter-service Communication)
- **CDC (Change Data Capture):** Sử dụng **Debezium** để đồng bộ dữ liệu từ `PropertyService` (Postgres) sang `SearchService` (Elasticsearch). Không gọi API đồng bộ để cập nhật Search Index.
- **Event-Driven:** Sử dụng **Kafka** cho các logic nghiệp vụ chéo (ví dụ: Thanh toán thành công -> Cập nhật trạng thái Đặt phòng).
- **Transactional Outbox:** Bắt buộc lưu Event vào bảng `Outbox` trong cùng một Transaction với logic nghiệp vụ chính. KHÔNG bắn Kafka trực tiếp từ Handler.
- **Idempotency:** Mọi Consumer phải có cơ chế chống trùng lặp (ví dụ: bảng `ProcessedEvent`).

---

## 💻 2. Quy chuẩn Backend (.NET)

### 2.1 Cấu trúc mã nguồn (Vertical Slice Architecture)
- **Vertical Slices:** Đóng gói toàn bộ DTO, Endpoint, và Logic của một nghiệp vụ (Use Case) vào chung một thư mục/class. Loại bỏ Controllers kiểu MVC truyền thống.
- **REPR Pattern:** Sử dụng **FastEndpoints**. Mỗi Class kế thừa `Endpoint<TRequest, TResponse>` đại diện cho một API.

### 2.2 Native AOT Readiness (Bắt buộc)
- **JSON DTOs:** Mọi DTO phải được khai báo trong `[JsonSerializable]` tại `JsonSerializerContext` của service đó.
- **Nghiêm cấm:** Không sử dụng `dynamic`, `object`, hoặc `Reflection` bừa bãi trong các luồng xử lý DTO.

### 2.3 Domain & Logic
- **Rich Domain Model:** Logic nghiệp vụ và các quy tắc thay đổi trạng thái phải nằm trong Domain Entity (ví dụ: `User.cs`, `Booking.cs`). Tránh Anemic Domain Model.
- **Validation:** Sử dụng **FluentValidation** cho dữ liệu đầu vào. Trả về mã lỗi 400/422 thống nhất qua FastEndpoints.
- **Database:** Sử dụng **EF Core**. Mọi thay đổi schema phải thực hiện qua **Migrations CLI**.

---

## 🎨 3. Quy chuẩn Frontend (React)

### 3.1 Phân lớp Layer
- **Layer 1 (API):** Chỉ chứa các hàm Axios thuần túy. Không chứa state.
- **Layer 2 (Hooks):** Bọc API bằng **TanStack Query** (`useQuery`, `useMutation`). Quản lý cache và logic điều hướng.
- **Layer 3 (UI):** Chỉ hiển thị giao diện. Gọi Logic qua Custom Hooks. **Nghiêm cấm gọi Axios trực tiếp trong UI.**

### 3.2 State Management
- **Zustand:** Chỉ dùng cho Global State (Auth, Theme). Không dùng để cache dữ liệu API.
- **TanStack Query:** Chịu trách nhiệm hoàn toàn việc quản lý dữ liệu từ server.

### 3.3 UI Patterns
- **Atomic Design:** Sử dụng bộ UI **shadcn/ui** (Atoms) để lắp ghép giao diện.
- **Type-Safety:** Nghiêm cấm sử dụng `any`. Mọi Request/Response phải có `interface` rõ ràng.
- **Forms:** Sử dụng `react-hook-form` + `zod` cho các form phức tạp.

---

## 🛡️ 4. Quy tắc Chung & Chất lượng Code

- **Clean Code:** Xóa bỏ hoàn toàn Dead code, biến/import không sử dụng.
- **Logging:** Sử dụng **ILogger<T>** (Structured Logging). Nghiêm cấm `Console.WriteLine()` (Backend) và `console.log()` (Frontend).
- **Security:** Mọi API phải đi qua **YARP Gateway**. Authentication thực hiện bằng **JWT Bearer**.
- **Git:** Không tự ý merge vào `main`. Tuân thủ workflow của dự án.
