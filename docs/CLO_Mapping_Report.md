# BÁO CÁO ĐÁP ỨNG CHUẨN ĐẦU RA HỌC PHẦN (CLOs)
**Dự án:** AirVnV - Hệ thống Đặt phòng Lưu trú theo Kiến trúc Hướng dịch vụ (Microservices)

Tài liệu này đối chiếu trực tiếp các kết quả đạt được của dự án với các Chuẩn đầu ra học phần (Course Learning Outcomes - CLOs) theo quy định chấm điểm.

---

## 1. Chuẩn đầu ra học phần (CLOs) áp dụng cho Đồ án

| STT | Chuẩn đầu ra học phần (CLOs) | Kiến thức (Cognitive) | Kỹ năng (Psychomotor) | Thái độ (Affective) | Chỉ báo PI |
| :--- | :--- | :---: | :---: | :---: | :---: |
| **CLO1** | Hiểu quá trình phân tích, thiết kế dịch vụ sử dụng SOAP, REST và Microservice | C2 | | | PLO1/ X E |
| **CLO2** | Áp dụng được hoạt động phát triển phần mềm hướng dịch vụ cho một bài toán cụ thể | C3 | | | PLO3/ X E |
| **CLO3** | Trình bày thành thạo được kết quả dự án cá nhân của mình | | P3 | | PLO4/ X E |

---

## 2. Hoạt động kiểm tra và hoạt động dạy học theo chuẩn đầu ra

| CLOs | Hình thức kiểm tra theo chuẩn đầu ra | | | | | Hình thức dạy học theo chuẩn đầu ra | | | | |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| | **Báo cáo** | **Bảo vệ dự án** | **Thực hành** | **Vấn đáp** | **Tự luận / Trắc nghiệm** | **Bài giảng** | **Làm việc nhóm** | **Thảo luận nhóm** | **HD thực hành** | **HD thực tập** |
| **CLO1** | X | X | | | | X | X | X | | |
| **CLO2** | X | X | | | | X | X | X | | |
| **CLO3** | X | X | | | | X | X | X | | |

---

## 3. Kế hoạch kiểm tra theo chuẩn đầu ra (Thang điểm 10)

| Thành phần kiểm tra | Hoạt động Kiểm tra | Hình thức kiểm tra | Trọng số (%) | Thời điểm kiểm tra (tuần) | CĐR HP (CLOs) |
| :--- | :--- | :--- | :---: | :--- | :--- |
| **Chuyên cần** (formative assessment) | Điểm danh | Điểm danh | 10% | Hàng tuần | (Không dùng KT CLO) |
| **Trung bình kiểm tra** (formative assessment) | Bài tập | Bài tập kỹ năng | 20% | 5, 10 | (Không dùng KT CLO) |
| **Bài tập lớn và bảo vệ dự án kết thúc học phần** (summative assessment) | **Báo cáo cuối kỳ** | Báo cáo | **40%** | Cuối kỳ | **CLO 1, 2, 3** |
| | **Trình bày mã nguồn và demo kết quả** | Bảo vệ dự án | **30%** | Cuối kỳ | **CLO 2, 3** |

---

## 4. Bảng Đối chiếu Kết quả Dự án AirVnV với CLOs (Dùng để Giảng viên chấm điểm)

Phần này mapping trực tiếp các thành phần kỹ thuật của Đồ án vào tiêu chí chấm điểm của học phần.

| CLOs | Tiêu chí đánh giá | Minh chứng từ Đồ án AirVnV (Rules Mapping) |
| :--- | :--- | :--- |
| **CLO1** | Hiểu quá trình phân tích, thiết kế dịch vụ sử dụng REST và Microservice (C2) | **Thiết kế Kiến trúc:** Phân rã thành công thành 6 Bounded Contexts độc lập (Identity, Property, Search, Booking, Payment, Chat).<br><br>**Thiết kế Dữ liệu:** Áp dụng triệt để nguyên tắc Database-per-service (Mỗi service có 1 DB PostgreSQL riêng biệt, không query chéo).<br><br>**Phân tích Tích hợp:** Thiết kế các luồng giao tiếp đồng bộ (YARP API Gateway, REST HTTPClient) và bất đồng bộ (Saga Orchestration, Event-Driven). |
| **CLO2** | Áp dụng hoạt động phát triển phần mềm hướng dịch vụ cho bài toán cụ thể (C3) | **Áp dụng REST & CQRS:** Sử dụng `FastEndpoints` và `MediatR` để xây dựng API chuẩn RESTful, tách biệt hoàn toàn luồng Đọc/Ghi dữ liệu.<br><br>**Áp dụng Microservices Patterns:** Lập trình thành công các mẫu thiết kế mức độ Enterprise:<br>- *Saga Orchestration:* Sử dụng `MassTransit StateMachine` điều phối luồng Đặt phòng & Hoàn tiền.<br>- *Transactional Outbox:* Chống mất dữ liệu qua mạng.<br>- *Change Data Capture (CDC):* Sử dụng `Debezium` và `Kafka` đồng bộ dữ liệu vào `Elasticsearch`.<br><br>**Công nghệ:** .NET 8/9, React/Zustand, Docker Compose, RabbitMQ. |
| **CLO3** | Trình bày thành thạo kết quả dự án cá nhân (P3) | **Báo cáo (40%):** Hoàn thiện Slide báo cáo mạch lạc, thể hiện rõ các sơ đồ Sequence Diagram (Luồng Booking) và Architecture Diagram (Luồng CDC).<br><br>**Bảo vệ & Demo (30%):**<br>- Trình bày trực quan luồng hệ thống qua **.NET Aspire Dashboard** (Distributed Tracing).<br>- Demo giao diện người dùng (React UI).<br>- Trả lời vấn đáp thông qua việc giải thích trực tiếp luồng code Event-Driven trên Visual Studio/Rider. |
