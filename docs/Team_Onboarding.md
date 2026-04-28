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

## 🧑‍💻 Quy chuẩn Viết Code & Review

1. **Nguyên lý Clean Code**:
   * Giữ các Method/Component ngắn gọn, tập trung vào 1 nhiệm vụ.
   * Tách biệt Logic UI và Logic xử lý trạng thái.
2. **Database Migration**:
   * Không can thiệp tay vào cấu trúc Table. Sử dụng hoàn toàn EF Core Migrations.

---

## 🆘 Khắc phục sự cố thường gặp

*   **Lỗi OOM (Out of Memory) do Kafka/Elasticsearch nuốt RAM:**
    * Đã được khống chế Heap Size an toàn tại cấu hình AppHost.
*   **Aspire không tìm thấy Runtime Container:**
    * Đảm bảo Podman/Docker Engine của bạn đang ở chế độ hoạt động (Running).

Hãy chủ động trao đổi qua Discord chung của team nếu gặp bất cứ khó khăn nào!
