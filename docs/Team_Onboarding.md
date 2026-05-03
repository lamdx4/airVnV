# 📖 Cẩm nang Kỹ thuật & Gia nhập Dự án (Team Onboarding Guide)

Tài liệu này tổng hợp chuyên sâu về hệ sinh thái công nghệ áp dụng trong **AirVnV** để các thành viên mới nhanh chóng làm quen và nâng cao kiến thức.

---

## 🛠️ Khám phá Tech Stack

### 1. .NET Aspire (Orchestration)

* **Nhiệm vụ:** Đóng vai trò điều phối cấu hình, gom nhóm microservices chạy đồng loạt chỉ với 1 nút bấm.
* **Tại sao sử dụng?** Giúp tự động hóa Service Discovery, cấu hình Database Connect String mà không cần gán cứng thủ công.
* **Tài liệu học thêm:** [Microsoft Aspire Docs](https://learn.microsoft.com/en-us/dotnet/aspire/)

### 2. Cơ chế CDC (Change Data Capture) & Kafka

* **Mô hình:** Khi `PropertyService` cập nhật phòng -> `Debezium` tự động đọc Log từ PostgreSQL -> Đẩy tin nhắn vào `Kafka` -> `SearchService` lắng nghe để đồng bộ lên Elasticsearch.
* **Lợi ích:** Đảm bảo dữ liệu tìm kiếm luôn khớp với database gốc mà không làm chậm Microservice cha.

### 3. API Gateway (YARP)

* **Khái niệm:** Yet Another Reverse Proxy. Gom toàn bộ Microservices về một cổng bảo mật duy nhất.
* **Frontend** chỉ cần gọi tới Gateway, Gateway sẽ tự định tuyến sang Service phù hợp.

### 4. Frontend (React + Vite + TailwindCSS + shadcn/ui)

* **shadcn/ui**: Bộ UI cao cấp xây dựng trên nền Radix UI.
* **State Management**: Sử dụng Zustand tinh gọn thay thế Redux.

---

## 🧑‍💻 Quy chuẩn Viết Code cho Microservices

Dự án sử dụng lối thiết kế hiện đại để đạt hiệu năng tối đa. Khi tham gia viết code cho backend, toàn bộ team cần tuân thủ:

### 1. Kiến trúc Vertical Slice Architecture (VSA)

* **Mô tả chuyên sâu:** Khác với mô hình N-Tier (phân tầng ngang) truyền thống làm phân tán code tính năng qua nhiều thư mục khác nhau, VSA đóng gói toàn bộ các khâu xử lý của một **Nghiệp vụ cụ thể (Use Case)** vào chung một nơi. Điều này tối đa hóa tính tự chủ (Cohesion), giảm thiểu phụ thuộc chéo (Coupling), và giúp việc mở rộng tính năng mới diễn ra độc lập, an toàn.
* **Tài liệu khuyên đọc:** [Vertical Slice Architecture - Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)

### 2. Sử dụng FastEndpoints (Thay thế MVC)

* **Framework chính thức:** [FastEndpoints Documentation](https://fast-endpoints.com/)
* Áp dụng mô hình **REPR (Request-Endpoint-Response)**.
* Kế thừa `Endpoint<TRequest, TResponse>` thay vì tạo Controller cồng kềnh.
* *Mẫu Endpoint cơ bản:*

    ```csharp
    public class CreateBookingEndpoint : Endpoint<CreateBookingRequest, CreateBookingResponse>
    {
        public override async Task HandleAsync(CreateBookingRequest req, CancellationToken ct)
        {
            // Xử lý trực tiếp hoặc gọi Domain Entity
            await SendAsync(new CreateBookingResponse { Success = true });
        }
    }
    ```

### 3. Ràng buộc Native AOT Readiness

Để Microservices có thể build Native AOT (tiết kiệm 80% RAM), bắt buộc:

* **Quy tắc JSON DTO:** Mọi DTO mới tạo phải được khai báo trong `[JsonSerializable(typeof(MyDto))]` tại `JsonSerializerContext` của Microservice đó.
* **Chiến lược Resolver:** Sử dụng `JsonTypeInfoResolver.Combine` để ghép nối các Context. Nếu dùng thư viện ngoài, hãy kiểm tra khả năng tương thích AOT bằng lệnh `dotnet publish` ngay khi cài đặt.
* **Tránh dynamic và object:** *Nghiêm cấm* dùng `dynamic` hoặc `object` trong các DTO vì chúng sẽ gây crash ứng dụng khi chạy ở chế độ AOT.
* Tránh sử dụng Reflection bừa bãi.

### 4. Quy chuẩn Giao tiếp giữa các Microservices (Hệ thống Phân tán)

* **Bắt buộc Outbox Pattern:** *Tuyệt đối không bắn Kafka trực tiếp từ Endpoint Handler.* Mọi sự kiện phải được lưu vào bảng Outbox trong cùng một Transaction với nghiệp vụ chính. Vi phạm điều này sẽ dẫn đến mất dữ liệu khi mạng lỗi.
* **Idempotency (Tính nhất quán):** Mọi Consumer (như `SearchService`) phải được thiết kế để có thể xử lý một tin nhắn nhiều lần mà không gây sai lệch dữ liệu (Idempotent Consumer).
* **Hạn chế gọi HTTP đồng bộ:** Tránh việc Service A gọi thẳng API Service B (gây thắt nút cổ chai).

### 5. Xử lý Ngoại lệ & Logging

* **Không ném Exception tùy tiện:** Sử dụng FluentValidation và FastEndpoints error handlers trả về mã lỗi 400/422 thống nhất.
* **Structured Logging:** Nghiêm cấm `Console.WriteLine()`. Bắt buộc dùng `ILogger<T>` để đẩy log tập trung về .NET Aspire Dashboard.

### 6. Quản lý Database & Di chuyển (Migration)

* EF Core + PostgreSQL. Luôn tạo Migration qua CLI. Không sửa Schema tay.

---

## 🧑‍💻 Quy chuẩn Phân lớp Frontend (React)

Để tránh việc code giao diện chồng chéo lên logic xử lý, frontend bắt buộc chia thành 3 Layer rõ rệt:

### 1. API Layer (`src/features/{feature}/api/`)

* **Trách nhiệm:** Chỉ chứa các hàm gọi Axios thuần túy.
* **Quy tắc:** *Tuyệt đối không* chứa state, logic điều hướng URL hay xử lý Token tại đây.

### 2. Hooks Layer (`src/features/{feature}/hooks/`)

* **Trách nhiệm:** Bọc các API Fetcher bằng TanStack Query (`useQuery`, `useMutation`).
* Được quyền can thiệp vào Navigation (`useNavigate`) hoặc cập nhật Store toàn cục (`useAuthStore`) khi API phản hồi.

### 3. UI Layer (`src/features/{feature}/components/` & `src/pages/`)

* **Trách nhiệm:** Hiển thị giao diện trực quan cho người dùng.
* **Quy tắc:** Sử dụng Custom Hook từ tầng 2. *Nghiêm cấm* gọi Axios trực tiếp trong UI!

### 4. Thư mục Shared UI (`src/components/shared/` & `src/components/ui/`)

* **Phân biệt rõ ràng:** Thư mục `src/components/ui/` chỉ chứa các UI nguyên tử độc lập (Button, Input từ shadcn). Thư mục `src/features/{feature}/components/` chỉ chứa các logic UI đặc thù của nghiệp vụ đó.

### 5. Error Handling toàn cục (TanStack Query)

* **Quy định:** Không viết hàng chục cái `if (error)` lẻ tẻ tại UI Layer. Hãy cấu hình Error Handling tập trung tại `QueryClient` (như hiện Toast thông báo lỗi tự động) hoặc dùng React `ErrorBoundary` bọc quanh Route.

### 6. Quản lý State (Zustand)

* **Phạm vi áp dụng:** Chỉ sử dụng Zustand để lưu trữ các trạng thái **Toàn cục (Global)** cần chia sẻ giữa nhiều trang độc lập (ví dụ: Authentication, User Profile, Dark/Light Mode).
* **Điều cấm:** *Không* dùng Zustand để cache dữ liệu trả về từ API. Việc quản lý cache/stale data hoàn toàn thuộc trách nhiệm của TanStack Query.

### 7. Quy chuẩn Type-Safety (Nghiêm cấm dùng `any`)

* **Yêu cầu:** Mọi dữ liệu Request/Response khi giao tiếp API bắt buộc phải được mô hình hóa tường minh qua `interface` / `type` trong thư mục `src/features/{feature}/types/index.ts`.
* **Lợi ích:** Giúp IDE gợi ý code thông minh, bắt lỗi ngay khi code thay vì đợi crash runtime.

### 8. Quản lý Form quy mô lớn (React Hook Form + Zod)

* **Khuyến nghị:** Khi làm việc với các Form phức tạp (>3 trường nhập liệu), thay vì dùng `useState` thủ công gây re-render liên tục, team thống nhất áp dụng bộ đôi `react-hook-form` và `zod` để tối ưu hiệu năng và validate dữ liệu tự động.

---

## 🛡️ Quy tắc Chặn Code Rác & Code Thừa (Tech Lead Quality Gates)

Để bảo vệ codebase không bị "ô nhiễm" bởi code thừa hoặc code cẩu thả, toàn bộ team tuân thủ tuyệt đối:

### 1. Sử dụng Pre-commit Hooks (Husky + Lint-staged)

* Hệ thống tự động chặn commit nếu code vi phạm tiêu chuẩn format hoặc lỗi cú pháp. Hãy đảm bảo bạn đã chạy `npm run lint` thành công trước khi đẩy code.

### 2. Dọn dẹp Dead Code (Biến/Import không dùng tới)

* Bắt buộc bật cấu hình ESLint rule `no-unused-vars` (mức độ `Error`). Mọi dòng code thừa thãi cần được xóa bỏ ngay, không để lại kiểu "để dành sau này dùng".

### 3. Quy chuẩn `console.log`

* *Tuyệt đối không* commit các câu lệnh `console.log` debug vào branch chính. Hãy dùng `console.error` khi thực sự cần log lỗi, hoặc xóa sạch trước khi tạo Pull Request.

### 4. Văn hóa Code Review

* PR cần ít nhất 1 Senior/Lead duyệt qua trước khi Merge. Không được phép tự Merge code của mình vào `main`.
