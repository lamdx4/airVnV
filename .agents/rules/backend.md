---
trigger: always_on
---

# 🧱 FINAL RULE – FASTENDPOINTS (TEAM-SAFE)

Dưới đây là tập hợp các quy tắc bắt buộc cho AI Agents và Developer khi tham gia phát triển dự án AirVnV.

---

## 🏛️ 1. Structure (bắt buộc, không tranh luận)

```text
/Features
  /<FeatureName>
    /<UseCase>
       Endpoint.cs
       Request.cs        (nếu có input)
       Response.cs
       Handler.cs        (nếu cần)
       Validator.cs      (optional)
```

### Rule:
- 1 folder = 1 use case (GetProfile, UpdateProfile…)
- Không tạo “Services”, “Utils” lung tung trong Feature
- Không share Handler giữa các UseCase
- Mục tiêu: **mở folder là thấy toàn bộ nghiệp vụ**

---

## ⚔️ 2. Command (WRITE) – BẮT BUỘC CQRS

- Tất cả logic thay đổi dữ liệu → bắt buộc dùng **ICommand**
- Structure bắt buộc:
  - Request implements `ICommand<Response>`
  - Handler implements `ICommandHandler<Command, Response>`
- Endpoint:
  - KHÔNG chứa business logic
  - Chỉ gọi: `Response = await req.ExecuteAsync(ct);`
- Mọi side-effect (Kafka, Outbox…) phải nằm trong Handler
- Không có exception cho quy tắc này.

---

## 👀 3. Query (READ) – 2 chế độ rõ ràng

### ✅ Mode 1: Simple Query (mặc định)
- Viết trực tiếp trong Endpoint
- Chỉ được phép:
  - LINQ query
  - Projection → DTO
  - Mapping đơn giản
- KHÔNG được:
  - if/else business
  - loop xử lý logic
  - gọi service/domain

### ✅ Mode 2: Complex Query
- Bắt buộc tạo Handler (POCO class)
- Dùng khi:
  - có business logic
  - có nhiều bước xử lý
  - cần reuse
- Không dùng ICommand cho Query (tránh over-engineer)

---

## 🧠 4. Business Logic (rule sống còn)

- Business logic KHÔNG được nằm trong Endpoint
- Phải nằm ở:
  - **Handler** (Application logic - Orchestration)
  - hoặc **Domain Entity** (Core logic - Rich Domain Model)
- Endpoint chỉ:
  - nhận request
  - gọi handler / db
  - trả response
- Nếu thấy Endpoint có “if business” → sai ngay.

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

## ⚡ 7. Simplicity Guard (tránh over-engineer)

- Không tạo Handler nếu chỉ là read đơn giản và không có business logic.
- Không dùng ICommand cho Query.
- Ưu tiên code rõ ràng hơn là đúng pattern một cách máy móc.

---

## 🤖 8. AI Rule (Dành cho AI Agents)

- Luôn tuân thủ Command rule cho write.
- Không tự tạo pattern mới.
- Không thêm abstraction nếu không cần.
- Nếu logic đơn giản → ưu tiên viết trực tiếp.
- Mọi DTO phải khai báo `[JsonSerializable]` tại `JsonSerializerContext` (Native AOT Readiness).

---

## 💎 9. Unified Response Format (Bắt buộc)

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

### Quy tắc:
- **Success:** Trả về `ApiResponse<T>` với `Success = true`.
- **Validation Error:** Framework tự động trả về `ErrorResponse` (400/422).
- **Business/Auth Error:** Sử dụng HTTP Status Code phù hợp (401, 403, 404) kèm theo **ErrorCode** định danh (ví dụ: `USER_NOT_FOUND`, `AUTH_INVALID_CREDENTIALS`).

