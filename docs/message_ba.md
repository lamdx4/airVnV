# Tài Liệu Đặc Tả Nghiệp Vụ (BA) & Hướng Dẫn Phát Triển - Airbnb Chat Service

Tài liệu này giải thích chi tiết logic nghiệp vụ của hệ thống nhắn tin (Chat Service) theo chuẩn mô hình Airbnb, đồng thời chỉ ra rõ những hạng mục kỹ thuật mà một lập trình viên mới (New Developer) cần phải nắm bắt và thực hiện.

---

## 1. TƯ TƯỞNG CỐT LÕI (CORE CONCEPTS)

Hệ thống nhắn tin của Airbnb **không phải là hệ thống chat P2P (người với người) đơn thuần như Messenger hay Zalo**. Nó là hệ thống **Nhắn tin dựa trên Ngữ Cảnh (Context-based Messaging)**.

### Ngữ cảnh ở đây là gì?

Mọi cuộc hội thoại (`Conversation` / Thread) đều phải xoay quanh một **căn nhà (`Property`)** hoặc một **chuyến đi (`Booking`)**.

- Không có chuyện 2 người dùng tự nhiên tìm thấy nhau và chat với nhau vô cớ.
- Người thuê (`Guest`) chủ động nhắn tin cho Chủ nhà (`Host`) để hỏi về phòng (gọi là **Inquiry**).
- Khi Guest thanh toán đặt phòng thành công, một cuộc hội thoại mới (hoặc cuộc hội thoại cũ được cập nhật) sẽ gắn thêm mã **BookingId**.

### System Message (Tin nhắn hệ thống)

Hệ thống Airbnb rất hay có những tin nhắn tự động chèn vào giữa cuộc hội thoại (Ví dụ: *"Booking confirmed: 3 nights"*, *"Payment Failed"*).

- Các tin nhắn này không do User tự gõ, mà do hệ thống tự sinh ra (Auto-generated) dựa trên các sự kiện (Domain Events) từ các dịch vụ khác (ví dụ: BookingService báo về).

---

## 2. LUỒNG NGHIỆP VỤ & VIỆC DEVELOPER CẦN LÀM

### 🛠️ Workflow 1: Khách hàng (Guest) hỏi thăm Chủ nhà (Host)

- **Nghiệp vụ:** Guest vào trang chi tiết phòng, bấm nút "Liên hệ chủ nhà" và gửi tin nhắn đầu tiên. Hệ thống tạo ra một cuộc hội thoại mới gắn với `PropertyId` đó.
- **Developer cần làm:**
  - Viết Endpoint `POST /api/chat/conversations` (StartConversation).
  - Logic: Kiểm tra xem giữa `GuestId`, `HostId` và `PropertyId` đã có Conversation nào chưa. Nếu có -> Dùng lại. Nếu chưa -> Tạo mới bảng `Conversation` + Insert tin nhắn vào bảng `Message`.
  - Push tin nhắn qua SignalR cho Host (nếu Host đang online).

### 🛠️ Workflow 2: Khách hàng Đặt phòng thành công (Booking Creation)

- **Nghiệp vụ:** Guest đặt phòng và thanh toán xong. Host và Guest cần có một luồng chat chung để trao đổi về chuyến đi (hướng dẫn check-in, vị trí chìa khóa). Ngay khi đặt thành công, hệ thống tự động bắn một tin nhắn *"Booking đã được xác nhận"* vào lịch sử chat.
- **Developer cần làm:**
  - Viết một **Consumer** lắng nghe event `BookingCreatedEvent` từ RabbitMQ (do BookingService bắn ra).
  - Logic trong Consumer: Tìm hoặc tạo `Conversation` tương ứng, update cột `BookingId` cho Conversation đó.
  - Tự động insert một record vào bảng `Message` với `Type = System` và `Content = "Booking Confirmed"`.
  - Push notification/SignalR cho cả Guest và Host.

### 🛠️ Workflow 3: Quản lý Hộp thư đến (Inbox)

- **Nghiệp vụ:** Host có rất nhiều nhà, Guest có rất nhiều chuyến đi. Khi mở Inbox, họ cần thấy danh sách các cuộc hội thoại được sắp xếp theo thời gian tin nhắn mới nhất, kèm theo trích dẫn tin nhắn cuối cùng (Last Message Preview) và đếm số tin chưa đọc.
- **Developer cần làm:**
  - Viết Endpoint `GET /api/chat/inbox`.
  - Logic query: Filter bảng `Conversation` theo `UserId` (người dùng đang gọi API có thể là Guest hoặc Host).
  - Sort theo trường `LastMessageAt` giảm dần (DESC).
  - Trả về danh sách kèm thông tin cơ bản: Tên người chat cùng, Ảnh avatar, `LastMessageContent`, và trạng thái Unread.

### 🛠️ Workflow 4: Gửi tin nhắn và Trạng thái Đã đọc (Read Receipts)

- **Nghiệp vụ:** Khi đang mở ứng dụng, tin nhắn nhảy real-time không cần f5. Khi user bấm vào một Conversation, đánh dấu đã đọc các tin nhắn.
- **Developer cần làm:**
  - Xây dựng **SignalR Hub** (`ChatHub`) để đẩy dữ liệu real-time. Bắt buộc cắm **Redis Backplane**.
  - Viết Endpoint `PATCH /api/chat/conversations/{id}/read` để update `LastReadMessageId` cho Participant (kỹ thuật đối chiếu UUIDv7 để biết tin nào đã đọc mà không cần update từng dòng Message).

### 🛠️ Workflow 5: Đồng bộ thông tin người dùng (Data Replication)
- **Nghiệp vụ:** Khi User đổi tên/avatar bên `UserService`, `ChatService` cần phải tự động cập nhật lại các thông tin này trong tất cả các cuộc hội thoại mà User đó tham gia để hiển thị Inbox với tốc độ nhanh nhất.
- **Developer cần làm:**
  - **Code hiện tại:** Database của `ChatService` ĐÃ ĐƯỢC DỰNG SẴN. Thực thể `ConversationParticipant` lưu trữ trực tiếp các cột snapshot `DisplayName` và `AvatarUrl`.
  - **Nhiệm vụ:** Viết **Consumer** lắng nghe `UserProfileUpdatedEvent` từ `Airbnb.SharedKernel`.
  - Logic trong Consumer: Tìm TẤT CẢ các record trong bảng `ConversationParticipants` có `UserId` trùng khớp và thực hiện Update `DisplayName`, `AvatarUrl`.
  - Không cần đồng bộ lúc tạo tài khoản vì User mới tạo chưa tham gia bất kỳ cuộc hội thoại nào.

---

## 3. THIẾT KẾ CƠ SỞ DỮ LIỆU THỰC TẾ (DATABASE SCHEMA)

Kiến trúc DB đã được thiết kế tối ưu, **KHÔNG SỬ DỤNG bảng trung gian ChatUsers**, mà gộp trực tiếp thông tin Snapshot vào Participant và dùng kỹ thuật Read Receipt qua UUIDv7.

**Table `Conversations`**
- `Id` (PK - Guid)
- `PropertyId` (Guid) -> Căn nhà mà họ đang chat về
- `PropertyTitle` (String) -> Snapshot tên căn nhà
- `ReservationId` (Guid?) -> Null nếu chỉ là hỏi han, có giá trị nếu đã thành chuyến đi.
- `LastMessageAt` (DateTimeOffset) -> Để sort Inbox (cập nhật liên tục).
- `CreatedAt` (DateTimeOffset)
- Quan hệ `1-N` với `ConversationParticipants` và `Messages`.

**Table `ConversationParticipants` (Thay thế cho ChatUser)**
- `ConversationId` (FK)
- `UserId` (Guid)
- `Role` (Enum: Guest, Host)
- `DisplayName` (String) -> Snapshot Data Replication từ UserService.
- `AvatarUrl` (String?) -> Snapshot Data Replication.
- `LastReadMessageId` (Guid?) -> Mốc tin nhắn cuối cùng đã đọc (So sánh Version 7 UUID).
- `IsArchived` (Bool) -> Lưu trữ trạng thái Ẩn/Lưu trữ chat cho riêng cá nhân này.

**Table `Messages`**
- `Id` (PK - Guid, UUIDv7)
- `ConversationId` (FK - Guid)
- `SenderId` (Guid?) -> ID người gửi. Là Null nếu hệ thống tự gửi.
- `Content` (String) -> Nội dung tin nhắn
- `MessageType` (Enum: Text, Image, System) -> Loại tin nhắn
- `CreatedAt` (DateTimeOffset)

---

## 4. TÓM TẮT CHECKLIST CÔNG VIỆC CHO DEV

1. Setup Entity Framework (`DbContext`, `Entities`, Migration).
2. Cấu hình Mediator pattern (Quy tắc chung của team: Các command/query handler đảm nhận 100% business logic, Endpoint chỉ làm nhiệm vụ dispatch).
3. Code REST API:
   - `StartConversation`
   - `GetInbox`
   - `GetMessages` (Lịch sử chat)
   - `SendMessage` (API dự phòng/Upload hình ảnh)
4. Setup SignalR Hub & Redis Backplane.
5. Setup MassTransit Consumer lắng nghe `BookingCreatedEvent`.

> **Lưu ý cực kỳ quan trọng cho Dev mới:**
> Hệ thống áp dụng triệt để nguyên tắc Database-per-Microservice. Không được phép Query chéo từ bảng Users hay Properties của Service khác. Trong Inbox, nếu cần hiện tên User hay Ảnh phòng, Frontend sẽ tự đảm nhận việc lấy dữ liệu qua ID hoặc phải tổ chức cơ chế Data Replication (Lưu bản sao tên User trong ChatDB). Ở MVP, chúng ta chỉ cần trả về `PropertyId` và `ParticipantId`, việc map dữ liệu giao cho UI (Frontend) hoặc API Gateway.

---

## 5. TUÂN THỦ QUY TẮC CỦA DỰ ÁN (MANDATORY RULES)

Tất cả lập trình viên khi tham gia phát triển Chat Service **BẮT BUỘC** phải tuân thủ nghiêm ngặt các bộ quy tắc (Rules) đã được định nghĩa trong thư mục `.agents/rules/` của dự án:

### 🚀 5.1. Quy Tắc Backend
Chi tiết tại: [backend.md](../.agents/rules/backend.md)

### 🎨 5.2. Quy Tắc Frontend
Chi tiết tại: [frontend.md](../.agents/rules/frontend.md)

### 🌐 5.3. Quy Tắc Kiến Trúc
Chi tiết tại: [project.md](../.agents/rules/project.md)
