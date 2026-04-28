# 📐 Các Design Patterns áp dụng trong Kiến trúc AirVnV

Tài liệu này tổng hợp các mẫu thiết kế (Design Patterns) cốt lõi được triển khai xuyên suốt từ Backend Microservices đến Frontend React UI.

---

## 🏢 Backend & Distributed Systems Patterns

### 1. Orchestrator Pattern (.NET Aspire)
*   **Mô tả:** Quản lý tập trung vòng đời khởi động, cấu hình biến môi trường và phát hiện dịch vụ (Service Discovery) của toàn bộ các container/tiến trình con.

### 2. Vertical Slice Architecture (VSA)
*   **Mô tả:** Thay vì chia tầng ngang (Controllers -> Services -> Repositories), mã nguồn được chia dọc theo nghiệp vụ (Use Cases). Mỗi Use Case tự đóng gói DTO, Endpoint và Logic riêng.

### 3. REPR Pattern (Request-Endpoint-Response)
*   **Mô tả:** Hiện thực hóa qua **FastEndpoints**, loại bỏ sự cồng kềnh của MVC Controllers. Một Class = Một API Endpoint.

### 4. Database-per-Microservice
*   **Mô tả:** Tách biệt hoàn toàn DB cho `UserService`, `PropertyService`, `BookingService`. Không chia sẻ dữ liệu trực tiếp ở tầng DB để tránh thắt nút cổ chai.

### 5. API Gateway Pattern (YARP)
*   **Mô tả:** Sử dụng Yet Another Reverse Proxy làm chốt chặn duy nhất tiếp nhận traffic từ Frontend và chuyển tiếp an toàn vào cụm microservices nội bộ.

### 6. Transactional Outbox Pattern
*   **Mô tả:** Ghi bản ghi Event trực tiếp vào bảng Outbox cùng Transaction với logic nghiệp vụ chính, ngăn chặn mất mát tin nhắn Kafka khi hệ thống gặp sự cố.

### 7. Change Data Capture (CDC)
*   **Mô tả:** Sử dụng **Debezium** đọc WAL (Write-Ahead Logs) của Postgres và chuyển đổi thành Event stream.

### 8. Rich Domain Model (Domain-Driven Design)
*   **Mô tả:** Tránh mô hình Anemic (chỉ có Entity rỗng chứa Get/Set). Đóng gói toàn bộ thuộc tính dữ liệu cùng các quy tắc nghiệp vụ, phương thức thay đổi trạng thái trực tiếp bên trong Domain Entity (ví dụ: [User.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.UserService/Domain/User.cs)).

### 9. Event-Driven Architecture (EDA)
*   **Mô tả:** Thiết lập luồng trao đổi dữ liệu bất đồng bộ qua Kafka. Các service không cần biết mặt nhau mà chỉ cần tương tác qua các Topic sự kiện.

### 10. Dependency Injection (DI)
*   **Mô tả:** Tách rời việc khởi tạo và quản lý vòng đời của các Object phụ thuộc ra khỏi class nghiệp vụ chính, được .NET hỗ trợ sẵn thông qua IServiceCollection.

### 11. Options Pattern
*   **Mô tả:** Chuyển đổi và kiểm tra dữ liệu cấu hình từ `appsettings.json` sang các Class C# tường minh có hỗ trợ Type-safe mạnh mẽ.

### 12. CQRS (Command Query Responsibility Segregation)
*   **Mô tả:** Áp dụng ngầm định tại tầng Use Cases của VSA. Tách biệt rõ ràng giữa luồng ghi dữ liệu (Commands như `RegisterUser`) và luồng đọc/truy vấn dữ liệu (Queries như `Search`).

---

## 🎨 Frontend UI Patterns

### 1. Custom Hook Pattern
*   **Mô tả:** Tách biệt hoàn toàn Logic gọi API & State (`useAuth`, `useLogin`) khỏi mã hiển thị UI của React Component.

### 2. Flux Pattern (Zustand)
*   **Mô tả:** Quản lý State toàn cục theo một luồng dữ liệu một chiều an toàn, dễ đoán định.

### 3. Proxy Pattern (Axios Interceptors)
*   **Mô tả:** Tự động can thiệp vào Request để đính kèm Bearer JWT Token cho mọi yêu cầu HTTP mà không cần code lặp lại.

### 4. Atomic Design Pattern (shadcn/ui)
*   **Mô tả:** Phân rã giao diện thành các thành phần nguyên tử nhỏ nhất (Atoms như Button, Input) và lắp ghép thành các khối giao diện nghiệp vụ phức tạp hơn (Organisms).

### 5. Provider Pattern (React Context)
*   **Mô tả:** Quấn các Component con vào các Provider cấp cao (`QueryClientProvider`, `BrowserRouter`) để phân phối State/Logic xuyên suốt cây giao diện mà không cần truyền Props thủ công.
