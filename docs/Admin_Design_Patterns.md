# Design Patterns áp dụng trong module Admin

> Tài liệu này tổng hợp **2 Design Pattern** đã được refactor vào module Admin của hệ thống airVnV, kèm lý do, code smell trước khi refactor, và lợi ích sau khi refactor — phục vụ buổi báo cáo với giảng viên.

---

## Tổng quan

| # | Pattern | Loại (GoF) | Vùng áp dụng | Vấn đề giải quyết |
|---|---------|------------|--------------|-------------------|
| 1 | **Strategy** | Behavioral | Báo cáo admin theo ngày/tuần/tháng (`ReportBucketing`) | Loại bỏ `switch` lặp lại 4 lần × 3 service |
| 2 | **State** | Behavioral | Vòng đời trạng thái User (`Active / Suspended / Banned`) | Loại bỏ chuỗi `if Status is not …` rải rác trong domain |

Cả hai đều thuộc nhóm **Behavioral Pattern** — tập trung vào việc tổ chức hành vi và sự thay đổi hành vi tuỳ theo dữ liệu, đúng với bản chất của module Admin: nhiều thao tác đổi trạng thái + nhiều cách tổng hợp dữ liệu.

---

## 🧩 Pattern 1 — Strategy Pattern cho `ReportBucketing`

### 1.1. Bối cảnh

Module Admin có nhiều báo cáo time-series cho phép admin chọn nhóm dữ liệu theo `day | week | month`:
- `GET /api/admin/reports/revenue-series` (PaymentService)
- `GET /api/admin/reports/new-listings` (PropertyService)
- `GET /api/admin/reports/user-growth` (UserService)

Mỗi endpoint cần 4 phép tính phụ thuộc vào `groupBy`:
1. **NormalizeStart** — đưa ngày bắt đầu về đầu kỳ (đầu tuần / đầu tháng).
2. **Advance** — tiến tới mốc kế tiếp.
3. **Key** — sinh khoá nhóm để `GroupBy` (ví dụ `2026-W23`).
4. **Label** — sinh nhãn hiển thị (ví dụ `W23 2026`).

### 1.2. Vấn đề trước khi refactor (code smell)

Trong code cũ, một class `static ReportBucketing` lặp lại 4 hàm `switch` trên chuỗi `"day"/"week"/"month"`:

```csharp
public static string BucketKey(DateOnly d, string groupBy) => groupBy switch
{
    "month" => $"{d.Year:D4}-{d.Month:D2}",
    "week"  => $"{ISOWeek.GetYear(...):D4}-W{ISOWeek.GetWeekOfYear(...):D2}",
    _       => d.ToString("yyyy-MM-dd")
};

private static DateOnly NormalizeStart(DateOnly d, string groupBy) => groupBy switch { ... };
private static DateOnly Advance(DateOnly d, string groupBy) => groupBy switch { ... };
private static string BucketLabel(DateOnly d, string groupBy) => groupBy switch { ... };
```

Tệ hơn, class này được **copy-paste 3 lần** ở 3 service: `Airbnb.PaymentService`, `Airbnb.PropertyService`, `Airbnb.UserService`. Mỗi lần thêm option mới (ví dụ `"quarter"`) phải sửa **4 hàm × 3 file = 12 chỗ** — đúng định nghĩa của *Shotgun Surgery smell*.

Các vấn đề cụ thể:
- ❌ **Vi phạm OCP (Open/Closed Principle)**: muốn thêm "quarter" phải mở sửa 4 hàm.
- ❌ **Vi phạm DRY**: 3 bản copy gần như y hệt.
- ❌ **Nguy cơ sai sót**: nếu sửa quy tắc "tuần" ở 1 file mà quên 2 file kia → báo cáo không nhất quán.
- ❌ **Khó test**: không thể test riêng từng cách tính.

### 1.3. Tại sao chọn Strategy Pattern?

Strategy phù hợp khi:
1. Có **một họ thuật toán** cùng giải quyết một bài toán (ở đây: cách "gom" thời gian).
2. Cần **chọn thuật toán tại runtime** dựa trên input của user (`groupBy` từ query string).
3. Muốn **đóng kín** mỗi thuật toán, dễ thay/thêm mà không sửa code cũ.

Đây chính xác là tình huống đang gặp.

### 1.4. Thiết kế sau refactor

```
Airbnb.SharedKernel/Reports/
├── IBucketStrategy.cs          ← Interface (Strategy)
├── DayBucketStrategy.cs        ← Concrete strategy
├── WeekBucketStrategy.cs       ← Concrete strategy
├── MonthBucketStrategy.cs      ← Concrete strategy
└── BucketStrategyFactory.cs    ← Factory chọn strategy theo string
```

**Interface** (gom 4 phép tính + 1 default method `GenerateBuckets` dùng chung):

```csharp
public interface IBucketStrategy
{
    string Key(DateOnly date);
    string Label(DateOnly date);
    DateOnly NormalizeStart(DateOnly date);
    DateOnly Advance(DateOnly date);

    // Default interface method — dùng được cho mọi strategy
    List<(string Key, string Label)> GenerateBuckets(DateOnly from, DateOnly to)
    {
        var list = new List<(string, string)>();
        var cursor = NormalizeStart(from);
        while (cursor <= to)
        {
            list.Add((Key(cursor), Label(cursor)));
            cursor = Advance(cursor);
        }
        return list;
    }
}
```

**Concrete strategy** (ví dụ `MonthBucketStrategy`):

```csharp
public sealed class MonthBucketStrategy : IBucketStrategy
{
    public string Key(DateOnly d) => $"{d.Year:D4}-{d.Month:D2}";
    public string Label(DateOnly d) => d.ToString("yyyy-MM");
    public DateOnly NormalizeStart(DateOnly d) => new(d.Year, d.Month, 1);
    public DateOnly Advance(DateOnly d) => d.AddMonths(1);
}
```

**Factory** (đóng vai trò chọn strategy):

```csharp
public static class BucketStrategyFactory
{
    public static IBucketStrategy For(string? groupBy) => (groupBy ?? "day").ToLowerInvariant() switch
    {
        "month" => new MonthBucketStrategy(),
        "week"  => new WeekBucketStrategy(),
        _       => new DayBucketStrategy(),
    };
}
```

**Cách dùng trong endpoint** (ngắn gọn, không còn `switch`):

```csharp
var bucket = BucketStrategyFactory.For(req.GroupBy);
var buckets = bucket.GenerateBuckets(from, to);

var grouped = raw
    .GroupBy(d => bucket.Key(DateOnly.FromDateTime(d)))
    .ToDictionary(g => g.Key, g => g.Count());
```

### 1.5. Sơ đồ UML

```
        ┌──────────────────────────┐
        │  <<interface>>           │
        │  IBucketStrategy         │
        ├──────────────────────────┤
        │ + Key(d): string         │
        │ + Label(d): string       │
        │ + NormalizeStart(d)      │
        │ + Advance(d)             │
        │ + GenerateBuckets(...)   │
        └────────────△─────────────┘
                     │
        ┌────────────┼──────────────┐
        │            │              │
 ┌──────┴────┐ ┌─────┴────┐ ┌──────┴─────┐
 │ DayBucket │ │ WeekBucket│ │ MonthBucket│
 │ Strategy  │ │ Strategy  │ │ Strategy   │
 └───────────┘ └───────────┘ └────────────┘
        △
        │ creates
 ┌──────┴────────────────┐
 │ BucketStrategyFactory │
 └───────────────────────┘
        △
        │ uses
 ┌──────┴─────────────────────┐
 │ GetRevenueSeriesEndpoint    │
 │ GetNewListingsEndpoint      │
 │ GetUserGrowthReportEndpoint │
 └─────────────────────────────┘
```

### 1.6. Lợi ích đạt được

| Trước | Sau |
|-------|-----|
| 4 `switch` × 3 file = 12 nhánh | Mỗi strategy = 1 class độc lập |
| Thêm "quarter" → sửa 12 chỗ | Thêm `QuarterBucketStrategy` + 1 dòng trong Factory |
| Test gián tiếp qua endpoint | Test trực tiếp từng strategy bằng unit test |
| Code copy-paste 3 service | Dùng chung trong `Airbnb.SharedKernel` |

✅ Đáp ứng **Open/Closed Principle**: mở rộng (thêm strategy mới) không cần sửa code cũ.
✅ Đáp ứng **Single Responsibility**: mỗi strategy chỉ làm một việc.
✅ **DRY**: một nguồn sự thật duy nhất cho logic gom thời gian.

---

## 🧩 Pattern 2 — State Pattern cho `User Status`

### 2.1. Bối cảnh

Một User trong hệ thống có 3 trạng thái:

```
              Suspend                Ban
   [Active] ──────────► [Suspended] ─────► [Banned]
      │                       │              │
      │      Ban              │  Activate    │  Activate
      └───────────────────────┴──────────────┘
                              │
                              ▼
                          [Active]
```

Module Admin cung cấp 3 endpoint thay đổi trạng thái:
- `POST /api/admin/users/{id}/suspend`
- `POST /api/admin/users/{id}/ban`
- `POST /api/admin/users/{id}/activate`

Mỗi chuyển đổi (transition) có quy tắc riêng — ví dụ chỉ User `Active` mới được Suspend, chỉ User `Suspended/Banned` mới được Activate, …

### 2.2. Vấn đề trước khi refactor

Code cũ trong `User.cs` rải rác các câu `if Status is not …`:

```csharp
public void Suspend(string reason)
{
    if (Status is not UserStatus.Active)
        throw new BusinessException("Only active users can be suspended.", "INVALID_STATUS_TRANSITION");
    if (string.IsNullOrWhiteSpace(reason))
        throw new BusinessException("Suspension reason is required.", "REASON_REQUIRED");
    Status = UserStatus.Suspended;
    SuspensionReason = reason;
}

public void Ban(string reason)
{
    if (Status is UserStatus.Banned)
        throw new BusinessException("User is already banned.", "USER_ALREADY_BANNED");
    if (Status is not (UserStatus.Active or UserStatus.Suspended))
        throw new BusinessException("Cannot ban a user with current status.", "INVALID_STATUS_TRANSITION");
    if (string.IsNullOrWhiteSpace(reason))
        throw new BusinessException("Ban reason is required.", "REASON_REQUIRED");
    Status = UserStatus.Banned;
    BanReason = reason;
}

public void Activate()
{
    if (Status is not (UserStatus.Suspended or UserStatus.Banned))
        throw new BusinessException("Only suspended or banned users can be activated.", "INVALID_STATUS_TRANSITION");
    ...
}
```

Các vấn đề cụ thể:
- ❌ **State-checking conditionals** — một anti-pattern OOP kinh điển: mỗi hành vi phải tự kiểm tra trạng thái hiện tại trước khi thực thi.
- ❌ **Logic phân tán**: muốn biết "User đang Suspended thì làm được gì?" → phải đọc cả 3 phương thức.
- ❌ **Khó mở rộng**: nếu thêm trạng thái `PendingVerification`, phải sửa cả 3 phương thức + thêm phương thức mới.
- ❌ **Vi phạm Tell, Don't Ask**: code liên tục hỏi "bạn đang trạng thái nào?" thay vì để trạng thái tự quyết định hành vi.

### 2.3. Tại sao chọn State Pattern?

State Pattern phù hợp khi:
1. Đối tượng có **vòng đời trạng thái rõ ràng** (state machine).
2. Hành vi của đối tượng **thay đổi theo trạng thái**.
3. Bạn thấy code có nhiều `if/switch` trên một field "status/state".

`User` thoả đủ 3 điều kiện. Đây là ví dụ "kinh điển" của State Pattern.

### 2.4. Thiết kế sau refactor

```
src/Airbnb.UserService/Domain/States/
├── IUserStatusState.cs             ← Interface (State)
├── ActiveState.cs                  ← Concrete state
├── SuspendedState.cs               ← Concrete state
├── BannedState.cs                  ← Concrete state
└── UserStatusStateFactory.cs       ← Map UserStatus enum → State instance
```

**Interface** — mỗi state biết phản ứng với 3 hành động:

```csharp
internal interface IUserStatusState
{
    UserStatus Status { get; }
    void Suspend(User user, string reason);
    void Ban(User user, string reason);
    void Activate(User user);
}
```

**Concrete state** — mỗi class tự đóng gói các transition hợp lệ của riêng nó:

```csharp
internal sealed class ActiveState : IUserStatusState
{
    public UserStatus Status => UserStatus.Active;

    public void Suspend(User user, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Suspension reason is required.", "REASON_REQUIRED");
        user.MarkSuspended(reason);          // Active → Suspended (hợp lệ)
    }

    public void Ban(User user, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessException("Ban reason is required.", "REASON_REQUIRED");
        user.MarkBanned(reason);             // Active → Banned (hợp lệ)
    }

    public void Activate(User _) =>           // Active → Active (KHÔNG hợp lệ)
        throw new BusinessException("Only suspended or banned users can be activated.",
                                    "INVALID_STATUS_TRANSITION");
}
```

`BannedState` thì ngược lại: Suspend/Ban đều ném exception, chỉ `Activate` được phép.

**Aggregate `User`** chỉ còn **3 dòng delegation** + setter nội bộ:

```csharp
public void Suspend(string reason) => UserStatusStateFactory.For(Status).Suspend(this, reason);
public void Ban(string reason)     => UserStatusStateFactory.For(Status).Ban(this, reason);
public void Activate()             => UserStatusStateFactory.For(Status).Activate(this);

internal void MarkSuspended(string reason) { Status = UserStatus.Suspended; SuspensionReason = reason; }
internal void MarkBanned(string reason)    { Status = UserStatus.Banned;    BanReason = reason; }
internal void MarkActive()                 { Status = UserStatus.Active;    SuspensionReason = null; BanReason = null; }
```

> **Ghi chú thiết kế:** Vì `User` được EF Core map vào DB qua enum `Status`, ta **giữ lại** field `Status` (cho persistence) và **tính state instance khi cần** (không lưu state object). Đây là biến thể "stateless state object" — phổ biến khi kết hợp State Pattern với EF/ORM.

### 2.5. Sơ đồ UML

```
       ┌──────────────────────────┐
       │ <<interface>>            │
       │ IUserStatusState         │
       ├──────────────────────────┤
       │ + Suspend(user, reason)  │
       │ + Ban(user, reason)      │
       │ + Activate(user)         │
       └────────────△─────────────┘
                    │
       ┌────────────┼─────────────┐
       │            │             │
┌──────┴───┐ ┌──────┴────┐ ┌──────┴───┐
│ Active   │ │ Suspended │ │ Banned   │
│ State    │ │ State     │ │ State    │
└──────────┘ └───────────┘ └──────────┘
                    △
                    │ creates
       ┌────────────┴──────────────┐
       │ UserStatusStateFactory     │
       └───────────────────────────┘
                    △
                    │ uses
       ┌────────────┴──────────────┐
       │           User             │  ←── delegates Suspend/Ban/Activate
       │ (Aggregate Root)           │     to current state object
       └────────────────────────────┘
```

### 2.6. Lợi ích đạt được

| Trước | Sau |
|-------|-----|
| Mỗi method có 2–3 `if` kiểm tra Status | Method aggregate chỉ còn 1 dòng delegation |
| Logic transition rải khắp `User.cs` | Logic gom theo state — đọc 1 class hiểu hết hành vi của trạng thái đó |
| Thêm `PendingVerification` → sửa 3 method cũ | Thêm `PendingVerificationState` + 1 dòng Factory, **0 sửa file cũ** |
| Khó test combinatorial | Test mỗi state class riêng (3 × 3 = 9 case rõ ràng) |

✅ Đáp ứng **Open/Closed Principle**: thêm trạng thái không sửa code cũ.
✅ Đáp ứng **Tell, Don't Ask**: caller chỉ gọi `user.Suspend(reason)`, không cần biết User đang ở trạng thái nào.
✅ **Behavior locality**: muốn biết SuspendedState làm được gì → đọc đúng 1 file.
✅ Giữ nguyên **API public** của `User` → 3 handler admin (`SuspendUserHandler`, `BanUserHandler`, `ActivateUserHandler`) **không cần sửa một dòng**.

---

## 📌 So sánh hai pattern

| Tiêu chí | Strategy | State |
|----------|----------|-------|
| Mục đích chính | Hoán đổi **thuật toán** theo input | Hoán đổi **hành vi** theo trạng thái nội tại |
| Ai chọn? | Client/Factory chọn dựa trên dữ liệu ngoài (`groupBy`) | Object tự chọn dựa trên trạng thái bên trong (`Status`) |
| Strategy có biết các strategy khác? | Không | State có thể chuyển sang state khác (transition) |
| Cấu trúc UML | Gần như giống nhau (1 interface + N concrete + Context) | Gần như giống nhau |

> **Điểm thường bị hỏi:** "Strategy và State có UML giống nhau, khác chỗ nào?"
> **Trả lời:** Khác ở **ý đồ (intent)**. Strategy quan tâm "chọn cách làm", State quan tâm "tôi đang là gì → tôi cư xử ra sao".

---

## 🧪 Verify

Sau khi refactor, đã verify:

- ✅ Build thành công cả 3 service: `Airbnb.PaymentService`, `Airbnb.PropertyService`, `Airbnb.UserService` (0 error).
- ✅ Test có sẵn `Airbnb.PropertyService.Tests` pass 3/3.
- ✅ Hành vi domain giữ nguyên 100%: cùng `BusinessException` code (`INVALID_STATUS_TRANSITION`, `REASON_REQUIRED`, `USER_ALREADY_BANNED`).
- ✅ API public của `User` không đổi → các handler admin không phải sửa.

---

## 📎 File liên quan

**Strategy Pattern:**
- `Airbnb.SharedKernel/Reports/IBucketStrategy.cs`
- `Airbnb.SharedKernel/Reports/DayBucketStrategy.cs`
- `Airbnb.SharedKernel/Reports/WeekBucketStrategy.cs`
- `Airbnb.SharedKernel/Reports/MonthBucketStrategy.cs`
- `Airbnb.SharedKernel/Reports/BucketStrategyFactory.cs`
- Call sites: `GetRevenueSeries`, `GetNewListings`, `GetUserGrowthReport`

**State Pattern:**
- `src/Airbnb.UserService/Domain/States/IUserStatusState.cs`
- `src/Airbnb.UserService/Domain/States/ActiveState.cs`
- `src/Airbnb.UserService/Domain/States/SuspendedState.cs`
- `src/Airbnb.UserService/Domain/States/BannedState.cs`
- `src/Airbnb.UserService/Domain/States/UserStatusStateFactory.cs`
- Aggregate đã refactor: `src/Airbnb.UserService/Domain/User.cs`
