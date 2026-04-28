# 📖 Cẩm nang Kỹ thuật & Gia nhập Dự án (Team Onboarding Guide)

Tài liệu này tổng hợp chuyên sâu về hệ sinh thái công nghệ áp dụng trong **AirVnV** để các thành viên mới nhanh chóng làm quen và nâng cao kiến thức.

---

## 🛠️ Khám phá Tech Stack

### 1. .NET Aspire (Orchestration)
*   **Nhiệm vụ:** Đóng vai trò điều phối cấu hình, gom nhóm microservices chạy đồng loạt chỉ với 1 nút bấm.
*   **Tại sao sử dụng?** Giúp tự động hóa Service Discovery, cấu hình Database Connect String mà không cần gán cứng thủ công.
*   **Tài liệu học thêm:** [Microsoft Aspire Docs](https://learn.microsoft.com/en-us/dotnet/aspire/)

### 2. Cơ chế CDC (Change Data Capture) & Kafka
*   **Mô hình:** Khi `PropertyService` cập nhật phòng -> `Debezium` tự động đọc Log từ PostgreSQL -> Đẩy tin nhắn vào `Kafka` -> `SearchService` lắng nghe để đồng bộ lên Elasticsearch.
*   **Lợi ích:** Đảm bảo dữ liệu tìm kiếm luôn khớp với database gốc mà không làm chậm Microservice cha.

### 3. API Gateway (YARP)
*   **Khái niệm:** Yet Another Reverse Proxy. Gom toàn bộ Microservices về một cổng bảo mật duy nhất.
*   **Frontend** chỉ cần gọi tới Gateway, Gateway sẽ tự định tuyến sang Service phù hợp.

### 4. Frontend (React + Vite + TailwindCSS + shadcn/ui)
*   **shadcn/ui**: Bộ UI cao cấp xây dựng trên nền Radix UI. 
*   **State Management**: Sử dụng Zustand tinh gọn thay thế Redux.

---

## 🧑‍💻 Quy chuẩn Viết Code cho Microservices

Dự án sử dụng lối thiết kế hiện đại để đạt hiệu năng tối đa. Khi tham gia viết code cho backend, toàn bộ team cần tuân thủ:

### 1. Kiến trúc Vertical Slice Architecture (VSA)
*   **Mô tả chuyên sâu:** Khác với mô hình N-Tier (phân tầng ngang) truyền thống làm phân tán code tính năng qua nhiều thư mục khác nhau, VSA đóng gói toàn bộ các khâu xử lý của một **Nghiệp vụ cụ thể (Use Case)** vào chung một nơi. Điều này tối đa hóa tính tự chủ (Cohesion), giảm thiểu phụ thuộc chéo (Coupling), và giúp việc mở rộng tính năng mới diễn ra độc lập, an toàn.
*   **Tài liệu khuyên đọc:** [Vertical Slice Architecture - Jimmy Bogard](https://jimmybogard.com/vertical-slice-architecture/)

### 2. Sử dụng FastEndpoints (Thay thế MVC)
*   **Framework chính thức:** [FastEndpoints Documentation](https://fast-endpoints.com/)
*   Áp dụng mô hình **REPR (Request-Endpoint-Response)**.
*   Kế thừa `Endpoint<TRequest, TResponse>` thay vì tạo Controller cồng kềnh.
*   *Mẫu Endpoint cơ bản:*
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
*   Mọi DTO mới tạo phải được khai báo trong `[JsonSerializable(typeof(MyDto))]` tại `JsonSerializerContext` của Microservice đó.
*   Tránh sử dụng Reflection/Dynamic code.

### 4. Quản lý Database & Di chuyển (Migration)
*   EF Core + PostgreSQL. Luôn tạo Migration qua CLI. Không sửa Schema tay.

---

## 🧑‍💻 Quy chuẩn Phân lớp Frontend (React)

Để tránh việc code giao diện chồng chéo lên logic xử lý, frontend bắt buộc chia thành 3 Layer rõ rệt:

### 1. API Layer (`src/features/{feature}/api/`)
*   **Trách nhiệm:** Chỉ chứa các hàm gọi Axios thuần túy. 
*   **Quy tắc:** *Tuyệt đối không* chứa state, logic điều hướng URL hay xử lý Token tại đây.

### 2. Hooks Layer (`src/features/{feature}/hooks/`)
*   **Trách nhiệm:** Bọc các API Fetcher bằng TanStack Query (`useQuery`, `useMutation`).
*   Được quyền can thiệp vào Navigation (`useNavigate`) hoặc cập nhật Store toàn cục (`useAuthStore`) khi API phản hồi.

### 3. UI Layer (`src/features/{feature}/components/` & `src/pages/`)
*   **Trách nhiệm:** Hiển thị giao diện trực quan cho người dùng.
*   **Quy tắc:** Sử dụng Custom Hook từ tầng 2. *Nghiêm cấm* gọi Axios trực tiếp trong UI!

---

## 🆘 Khắc phục sự cố thường gặp

*   **Lỗi OOM (Out of Memory):** Do Kafka/Elasticsearch ngốn RAM, đã được khống chế Heap tối đa 512MB tại `appsettings.json` của AppHost.
*   **Mất đồng bộ dữ liệu Tìm kiếm:** Kiểm tra xem container Debezium có kết nối được với Kafka và Postgres không.

Chúc các bạn Code vui vẻ! Mọi thắc mắc hãy tag Leader ngay trên PR.
