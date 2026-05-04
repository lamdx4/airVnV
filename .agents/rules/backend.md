---
trigger: always_on
---


# 🧱 FINAL RULE – FASTENDPOINTS + MEDIATOR (TEAM-SAFE)

Dưới đây là tập hợp các quy tắc bắt buộc cho AI Agents và Developer khi tham gia phát triển dự án AirVnV.

---

## 🏛️ 1. Structure (bắt buộc, không tranh luận)

```text
/Features
  /<FeatureName>
    /<UseCase>
       Endpoint.cs
       Request.cs        (implements ICommand hoặc IQuery từ Mediator)
       Response.cs
       Handler.cs        (ICommandHandler hoặc IQueryHandler)
       Validator.cs      (optional – FastEndpoints.Validator<Request>)
```

### Rule

- 1 folder = 1 use case (GetProfile, UpdateProfile…)
- Không tạo "Services", "Utils" lung tung trong Feature
- Không share Handler giữa các UseCase
- Mục tiêu: **mở folder là thấy toàn bộ nghiệp vụ**

---

## ⚔️ 2. Command (WRITE) – BẮT BUỘC Mediator

- Tất cả logic thay đổi dữ liệu → bắt buộc dùng **Mediator.ICommand**
- Structure bắt buộc:
  - `Request.cs` implements `Mediator.ICommand<Response>`
  - `Handler.cs` implements `ICommandHandler<Request, Response>`
- Endpoint:
  - KHÔNG chứa business logic
  - Chỉ gọi: `await mediator.Send(req, ct)`
- Mọi side-effect (MassTransit Outbox, Domain Events) phải nằm trong Handler
- Không có exception cho quy tắc này.

```csharp
// Request.cs – Request IS the Command
public record CreatePropertyRequest(...) : Mediator.ICommand<CreatePropertyResponse>;

// Handler.cs
public sealed class CreatePropertyHandler(AppDbContext db, DomainEventPublisher publisher)
    : ICommandHandler<CreatePropertyRequest, CreatePropertyResponse>
{
    public async ValueTask<CreatePropertyResponse> Handle(
        CreatePropertyRequest req, CancellationToken ct)
    {
        // Business logic + domain events + SaveChanges
    }
}

// Endpoint.cs – thin, chỉ dispatch
public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<CreatePropertyRequest, ApiResponse<CreatePropertyResponse>>
{
    public override async Task HandleAsync(CreatePropertyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<CreatePropertyResponse>.SuccessResult(result), cancellation: ct);
    }
}
```

---

## 👀 3. Query (READ) – Mediator.IQuery

- Tất cả read operations → dùng **Mediator.IQuery**
- Structure bắt buộc:
  - `Request.cs` implements `Mediator.IQuery<Response>`
  - `Handler.cs` implements `IQueryHandler<Request, Response>`
- Endpoint:
  - KHÔNG chứa business logic hay LINQ
  - Chỉ gọi: `await mediator.Send(req, ct)`
- **Không còn phân biệt Simple/Complex Query** – mọi query đều có Handler.

```csharp
// Request.cs
public record GetPropertyRequest(Guid PropertyId) : Mediator.IQuery<GetPropertyResponse>;

// Handler.cs
public sealed class GetPropertyHandler(AppDbContext db)
    : IQueryHandler<GetPropertyRequest, GetPropertyResponse>
{
    public async ValueTask<GetPropertyResponse> Handle(
        GetPropertyRequest req, CancellationToken ct)
    {
        return await db.Properties
            .AsNoTracking()
            .Where(p => p.Id == req.PropertyId)
            .Select(p => new GetPropertyResponse(p.Id, p.Title, p.Status))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Property not found.");
    }
}

// Endpoint.cs – giống hệt Command endpoint
public class Endpoint(IMediator mediator)
    : FastEndpoints.Endpoint<GetPropertyRequest, ApiResponse<GetPropertyResponse>>
{
    public override async Task HandleAsync(GetPropertyRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await SendAsync(ApiResponse<GetPropertyResponse>.SuccessResult(result), cancellation: ct);
    }
}
```

---

## 🧠 4. Business Logic (rule sống còn)

- Business logic KHÔNG được nằm trong Endpoint
- Phải nằm ở:
  - **Handler** (Application logic – Orchestration)
  - hoặc **Domain Entity** (Core logic – Rich Domain Model)
- Endpoint chỉ:
  - nhận request
  - gọi `mediator.Send()`
  - trả response
- Nếu thấy Endpoint có "if business" hay LINQ → sai ngay.

---

## 🧩 5. Domain Boundary

- Logic nghiệp vụ quan trọng → nằm trong Domain
- Handler chỉ:
  - điều phối (orchestrate)
  - gọi Domain
  - save DB
- Không viết business rule lớn trong Handler để tránh "god class".

---

## 🚨 6. Anti-chaos (cực quan trọng)

- Không được tự tạo abstraction ngoài guideline
- Không được:
  - tạo Service layer tùy ý
  - inject DbContext lung tung nhiều nơi
  - viết logic trong Endpoint
- Nếu không chắc → dùng **Command + Handler**.

---

## ⚡ 7. Simplicity Guard

- Không tạo Handler nếu chỉ là health check hay static response (không có DB, không có domain).
- Không dùng `ICommand` cho Query (type safety: `IQuery` cho read, `ICommand` cho write).
- Ưu tiên code rõ ràng hơn là đúng pattern một cách máy móc.

---

## ⚠️ 8. Lưu ý namespace (quan trọng)

FastEndpoints cũng có `ICommand<>` – **conflict với Mediator**.
Luôn dùng fully-qualified name:

```csharp
// ✅ Đúng – tránh ambiguous reference
public record MyRequest(...) : Mediator.ICommand<MyResponse>;

// ❌ Sai – ambiguous giữa FastEndpoints.ICommand và Mediator.ICommand
public record MyRequest(...) : ICommand<MyResponse>;
```

---

## 🤖 9. AI Rule (Dành cho AI Agents)

- Luôn tuân thủ Command rule cho write: `Mediator.ICommand` + `ICommandHandler`.
- Luôn tuân thủ Query rule cho read: `Mediator.IQuery` + `IQueryHandler`.
- Endpoint luôn thin: chỉ `mediator.Send()`.
- Không tự tạo pattern mới.
- Không thêm abstraction nếu không cần.
- Mọi DTO phải khai báo `[JsonSerializable]` tại `JsonSerializerContext` (Native AOT Readiness).

---

## 💎 10. Unified Response Format (Bắt buộc)

Tất cả các API phản hồi thành công (200, 201) phải được bọc trong một Envelope chuẩn:

```csharp
public record ApiResponse<T>(
    T? Data,
    string? Message = null,
    bool Success = true,
    string? ErrorCode = null,
    List<string>? Errors = null)
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
```

### Quy tắc

- **Success:** Trả về `ApiResponse<T>` với `Success = true`.
- **Validation Error:** Framework tự động trả về `ErrorResponse` (400/422).
- **Business/Auth Error:** Sử dụng HTTP Status Code phù hợp (401, 403, 404) kèm theo **ErrorCode** định danh (ví dụ: `USER_NOT_FOUND`, `AUTH_INVALID_CREDENTIALS`).

---

## 🛡️ 11. Error Handling Policy (Bắt buộc)

- **Cấm xử lý lỗi thủ công:** Tuyệt đối không dùng `try-catch` trong Handler/Endpoint để đóng gói `ApiResponse` thất bại.
- **Tín hiệu lỗi:**
  - Sử dụng `NotFoundException` khi không tìm thấy tài nguyên.
  - **Bắt buộc:** Sử dụng `BusinessException(message, errorCode)` cho tất cả các vi phạm logic nghiệp vụ.
  - **Cấm:** Không sử dụng các Exception mặc định của hệ thống (như `InvalidOperationException`, `Exception`) để đại diện cho lỗi nghiệp vụ.
- **Global Middleware:** Mọi Exception sẽ được `ExceptionHandlingMiddleware` tự động bắt và đóng gói vào `ApiResponse<T>` với `Success = false`.
- **Swagger Documentation (Quan trọng):** Mỗi Endpoint phải mô tả danh sách các `ErrorCode` có thể trả về trong phần `Summary` bằng định dạng Markdown:
  - Sử dụng `s.Description` để liệt kê các `ErrorCode` (Dùng Bold và List).
  - Sử dụng `s.Responses[400]` hoặc `s.Responses[404]` để mô tả ý nghĩa tổng quát của nhóm lỗi đó.
  - Mục tiêu: Frontend nhìn vào Swagger là biết chính xác cần bắt (catch) những lỗi nghiệp vụ nào.
