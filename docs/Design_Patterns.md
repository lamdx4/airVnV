# 📐 Các Design Patterns áp dụng trong Kiến trúc AirVnV

Tài liệu này tổng hợp các mẫu thiết kế (Design Patterns) cốt lõi được triển khai xuyên suốt từ Backend Microservices đến Frontend React UI.

---

## 🏢 Backend & Distributed Systems Patterns

### 1. Orchestrator Pattern (.NET Aspire)

- **Mô tả:** Quản lý tập trung vòng đời khởi động, cấu hình biến môi trường và phát hiện dịch vụ (Service Discovery) của toàn bộ các container/tiến trình con.
- **Áp dụng:** [AppHost.cs](file:///home/lamdx4/Projects/Airbnb/Airbnb.AppHost/AppHost.cs) – định nghĩa Postgres, RabbitMQ, Kafka, Elasticsearch, tất cả services.

### 2. Vertical Slice Architecture (VSA)

- **Mô tả:** Thay vì chia tầng ngang (Controllers → Services → Repositories), mã nguồn được chia dọc theo nghiệp vụ (Use Cases). Mỗi Use Case tự đóng gói Request, Handler, Response, Validator.
- **Structure:** `Features/<FeatureName>/<UseCase>/` – mở folder là thấy toàn bộ nghiệp vụ.

### 3. REPR Pattern (Request-Endpoint-Response)

- **Mô tả:** Hiện thực hóa qua **FastEndpoints**, loại bỏ sự cồng kềnh của MVC Controllers. Một Class = Một API Endpoint. Endpoint luôn thin: chỉ `mediator.Send()`.
- **Áp dụng:** [CreateProperty/Endpoint.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Features/CreateProperty/Endpoint.cs)

### 4. CQRS + Mediator (martinothamar/Mediator)

- **Mô tả:** Tách biệt Command (Write) và Query (Read) thông qua source-generated Mediator. Zero runtime reflection. Endpoint uniform: `mediator.Send(req, ct)` cho cả hai loại.
- **Command:** `Request : Mediator.ICommand<Response>` → `ICommandHandler<Request, Response>`
- **Query:** `Request : Mediator.IQuery<Response>` → `IQueryHandler<Request, Response>`
- **Áp dụng:** [CreateProperty/Handler.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Features/CreateProperty/Handler.cs)

> ⚠️ **Namespace conflict:** FastEndpoints cũng có `ICommand<>`. Luôn dùng `Mediator.ICommand<Response>` (fully-qualified).

### 5. Domain-Driven Design (DDD) – Rich Domain Model

- **Mô tả:** Tránh Anemic Model. Đóng gói business logic và invariants trực tiếp trong Domain Entity. Setter private, thay đổi state chỉ qua Domain Methods.
- **Áp dụng:** [Property.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Domain/Property.cs) – `Submit()`, `Approve()`, `Suspend()`, `Archive()` với guarded transitions.

### 6. Aggregate Root Pattern

- **Mô tả:** Aggregate Root là điểm entry duy nhất cho thao tác trên aggregate. Quản lý domain events nội bộ qua `Raise()` / `ClearDomainEvents()`. Satellite entities chỉ được tạo qua Root.
- **Shared via:** [Airbnb.SharedKernel](file:///home/lamdx4/Projects/Airbnb/Airbnb.SharedKernel/Domain/AggregateRoot.cs) – dùng chung `PropertyService`, `BookingService`, `PaymentService`.

### 7. Domain Events Pattern

- **Mô tả:** Aggregate phát ra immutable events (past tense) khi state thay đổi. Domain không biết về Infrastructure (không có Topic/Exchange). Mapping Domain → Topic nằm ở Infrastructure layer.
- **Interface:** [IDomainEvent](file:///home/lamdx4/Projects/Airbnb/Airbnb.SharedKernel/Domain/IDomainEvent.cs) trong SharedKernel.
- **Events:** [DomainEvents.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Domain/DomainEvents.cs) – `PropertyPublishedEvent`, `PropertySuspendedEvent`...

### 8. Transactional Outbox Pattern (MassTransit EF Core Outbox)

- **Mô tả:** Ghi Event vào bảng Outbox **trong cùng DB Transaction** với nghiệp vụ. MassTransit background service poll và dispatch sang RabbitMQ. Atomicity đảm bảo – không mất event dù crash.
- **Áp dụng:** `UseBusOutbox()` trong [Program.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Program.cs).
- **Publisher bridge:** [DomainEventPublisher.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Infrastructure/Messaging/DomainEventPublisher.cs) – map Domain Event → MassTransit `IPublishEndpoint`.

### 9. Idempotent Consumer Pattern

- **Mô tả:** Consumer kiểm tra xem Event đã được xử lý chưa qua unique constraint trên `EventId` (thay vì `AnyAsync` – tránh TOCTOU race condition). Dùng `try/catch` với Postgres SQLState `23505`.
- **Pattern:** Business logic + Insert `ProcessedEvent` trong cùng transaction → atomic, race-condition-free.
- **Áp dụng:** [Booking.cs](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.BookingService/Domain/Booking.cs) – `ProcessedEvent` entity.

### 10. Factory Method Pattern

- **Mô tả:** Thay constructor public bằng `static Create()` method có validation đầy đủ. EF Core dùng private constructor. Ngăn chặn object ở trạng thái invalid.
- **Áp dụng:** `Property.Create()`, `Booking.Create()`, `Payment.Create()`, `PropertyImage.Create()`.

### 11. Shared Kernel Pattern (DDD)

- **Mô tả:** Tách các building blocks dùng chung (IDomainEvent, AggregateRoot) ra project riêng. Services consume qua project reference, không duplicate code. Chỉ chứa infrastructure types – không có domain-specific logic.
- **Áp dụng:** [Airbnb.SharedKernel](file:///home/lamdx4/Projects/Airbnb/Airbnb.SharedKernel/) → referenced bởi `PropertyService`, `BookingService`, `PaymentService`.

### 12. Database-per-Microservice

- **Mô tả:** Tách biệt hoàn toàn DB cho mỗi service. Không cross-query giữa databases. Tham chiếu dữ liệu chỉ qua ID (Guid).
- **Áp dụng:** `propdb`, `bookdb`, `paydb`, `userdb` – mỗi service một database riêng.

### 13. Change Data Capture (CDC) – Debezium

- **Mô tả:** Debezium đọc WAL (Write-Ahead Logs) của Postgres và chuyển thành Event stream qua **Kafka** → ElasticSearch (Search Service). Kafka chỉ dùng cho CDC pipeline, không phải inter-service messaging.
- **Kafka role:** CDC only. Inter-service events dùng RabbitMQ + MassTransit.

### 14. API Gateway Pattern (YARP)

- **Mô tả:** Yet Another Reverse Proxy làm điểm vào duy nhất. Authentication tại Gateway, forward request với `X-User-Id` header vào internal services. Frontend không biết service nào tồn tại.

### 15. EF Core Configuration Pattern (Fluent API)

- **Mô tả:** Tách EF Core entity configuration ra `IEntityTypeConfiguration<T>` riêng từng file, đăng ký qua `ApplyConfiguration()` trong DbContext. DbContext chỉ chứa DbSets và `OnModelCreating`.
- **Áp dụng:** [Infrastructure/Configurations/](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Infrastructure/Configurations/) – `PropertyConfiguration`, `AdminDivisionConfiguration`...

### 16. Value Object Pattern (EF Core JSONB)

- **Mô tả:** Immutable record types (`Pricing`, `PropertyCapacity`, `HouseRules`, `AddressRaw`) lưu dưới dạng JSONB column. Validation nằm trong constructor, không thể tạo object invalid.
- **Áp dụng:** [Domain/ValueObjects/](file:///home/lamdx4/Projects/Airbnb/src/Airbnb.PropertyService/Domain/ValueObjects/)

### 17. Options Pattern

- **Mô tả:** Chuyển đổi cấu hình từ `appsettings.json` / environment variables sang Class C# tường minh, type-safe. Validate tại startup.

### 18. Dependency Injection (DI)

- **Mô tả:** .NET built-in IoC container. Vòng đời Scoped cho Handlers, DbContext. Transient cho lightweight services. Singleton tránh dùng với mutable state.

---

## 🎨 Frontend UI Patterns

### 1. Feature-Sliced Architecture

- **Mô tả:** Code tổ chức theo feature (`features/auth/`, `features/property/`...), mỗi feature có `api/`, `types/`, `utils/`, `hooks/`, `components/` riêng.
- **Rule:** Không share Hook giữa features. Logic dùng chung → `src/hooks/` (Shared layer).

### 2. Custom Hook Pattern

- **Mô tả:** Tách biệt Logic gọi API & State (`useAuth`, `useProfile`) khỏi UI Component. Hook chỉ dùng TanStack Query – không chứa `useNavigate` hay business logic phức tạp.
- **Áp dụng:** [features/auth/hooks/useAuth.ts](file:///home/lamdx4/Projects/Airbnb/airbnb-web/src/features/auth/hooks/useAuth.ts)

### 3. DTO → Model Mapper Pattern

- **Mô tả:** UI không dùng trực tiếp schema từ API (DTO). Tất cả transformation nằm trong Pure Functions ở `utils/mapper.ts`. UI chỉ biết đến `Model`.
- **Rule:** Cấm dùng `any`. DTO từ API, Model cho UI – tách biệt hoàn toàn.

### 4. Server State vs. Client State

- **Mô tả:** Dữ liệu từ API → TanStack Query (server state, không vào Zustand). State "xuyên trang" → Zustand (`AuthSession`, `Theme`). State nội bộ Component → `useState`.
- **Áp dụng:** [store/authStore.ts](file:///home/lamdx4/Projects/Airbnb/airbnb-web/src/store/authStore.ts)

### 5. Flux Pattern (Zustand)

- **Mô tả:** Quản lý Global State theo luồng một chiều. Store chỉ chứa `AuthSession`, `Theme`, `GlobalSettings` – không lưu server data.

### 6. Proxy Pattern (Axios Interceptors)

- **Mô tả:** Tự động đính kèm Bearer JWT Token cho mọi HTTP request. Intercept response để xử lý 401 (token refresh). Không hardcode URL – dùng `VITE_API_URL`.
- **Áp dụng:** [lib/api.ts](file:///home/lamdx4/Projects/Airbnb/airbnb-web/src/lib/api.ts)

### 7. Atomic Design Pattern (shadcn/ui)

- **Mô tả:** UI phân rã thành Atoms (`Button`, `Input`, `Card`) → Molecules → Organisms. shadcn/ui cung cấp unstyled primitives, project customize theo design system.

### 8. Provider Pattern (React Context)

- **Mô tả:** `QueryClientProvider`, `BrowserRouter` wrap toàn bộ app. Distribute State/Logic xuống cây component mà không prop-drilling.

---

## 🔁 Cross-Cutting Patterns

### 1. Exhaustiveness Check (CI Guard)

- **Mô tả:** Unit test kiểm tra `TopicMap` trong `DomainEventPublisher` phải cover **tất cả** `IDomainEvent` implementations trong assembly. Test fail ngay nếu thêm event mới mà quên map.
- **Áp dụng:** [DomainEventPublisherTests.cs](file:///home/lamdx4/Projects/Airbnb/tests/Airbnb.PropertyService.Tests/Infrastructure/DomainEventPublisherTests.cs)

### 2. Unified Response Envelope

- **Mô tả:** Mọi API response thành công bọc trong `ApiResponse<T>` với `Data`, `Success`, `ErrorCode`, `Timestamp`. Client chỉ cần parse một format duy nhất.
- **Áp dụng:** `ApiResponse<T>` trong `Airbnb.ServiceDefaults`.

### 3. Global Using (C# 10+)

- **Mô tả:** Khai báo `global using` cho namespace dùng xuyên suốt service (SharedKernel, common libraries) trong file `GlobalUsings.cs`, tránh lặp lại trong mọi file.
- **Áp dụng:** `GlobalUsings.cs` trong mỗi service.
