PUML_DATA_31_45 = {
    "UC-31": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Thu nhập" as payoutUI
control "Xử lý Rút tiền" as payoutController
entity "Payout" as payout

host -> payoutUI: 1. Gửi yêu cầu rút tiền
activate payoutUI
payoutUI -> payoutController: 2. Gọi API tạo lệnh Payout
activate payoutController
payoutController -> payoutController: 3. Kiểm tra số dư khả dụng (Available Balance)
alt Số dư hợp lệ và đủ mức tối thiểu
    payoutController -> payout: 4. Tạo lệnh Payout (Trạng thái: Pending)
    activate payout
    payout --> payoutController: 5. Sinh mã PayoutReference thành công
    deactivate payout
    payoutController --> payoutUI: 6. Phản hồi tạo lệnh thành công
    payoutUI --> host: 7. Hiển thị thông báo chờ Admin duyệt
else Số dư không đủ
    payoutController --> payoutUI: 4a. Báo lỗi "Insufficient funds"
    payoutUI --> host: 5a. Hiển thị thông báo từ chối
end
deactivate payoutController
deactivate payoutUI
@enduml""",

    "UC-32": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Lịch sử Giao dịch" as transactionUI
control "Xử lý Giao dịch" as paymentController
entity "Payment" as payment

host -> transactionUI: 1. Mở trang Lịch sử Giao dịch
activate transactionUI
transactionUI -> paymentController: 2. Yêu cầu lấy danh sách giao dịch
activate paymentController
paymentController -> payment: 3. Truy vấn DB Payment Service theo HostId
activate payment
payment --> paymentController: 4. Trả về danh sách (Immutable)
deactivate payment
paymentController --> transactionUI: 5. Phản hồi danh sách phân trang
transactionUI --> host: 6. Hiển thị lịch sử cộng/trừ tiền
deactivate paymentController
deactivate transactionUI
@enduml""",

    "UC-33": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Chi tiết Phòng" as propertyUI
control "Xử lý Chat" as chatController
entity "Conversation" as conversation

guest -> propertyUI: 1. Nhấn nút "Liên hệ Chủ nhà"
activate propertyUI
propertyUI -> chatController: 2. Yêu cầu khởi tạo hội thoại
activate chatController
chatController -> chatController: 3. Kiểm tra tính hợp lệ (Khách không phải là Chủ nhà)
alt Khách hợp lệ
    chatController -> conversation: 4. Kiểm tra Conversation_Id hiện có
    activate conversation
    conversation --> chatController: 5. Trả về Conversation_Id (Cũ hoặc Tạo mới)
    deactivate conversation
    chatController --> propertyUI: 6. Phản hồi Id hội thoại
    propertyUI --> guest: 7. Chuyển hướng sang Màn hình Chat
else Là Chủ nhà tự chat với chính mình
    chatController --> propertyUI: 4a. Báo lỗi "Host cannot start conversation"
    propertyUI --> guest: 5a. Hiển thị thông báo từ chối
end
deactivate chatController
deactivate propertyUI
@enduml""",

    "UC-34": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Chat" as chatUI
control "Xử lý Chat (WebSocket)" as chatController
entity "Message" as message
participant "SignalR / Socket.IO" as socket

user -> chatUI: 1. Nhập tin nhắn và Nhấn gửi
activate chatUI
chatUI -> chatController: 2. Gửi tin nhắn qua WebSocket
activate chatController
chatController -> chatController: 3. Kiểm tra quyền (Là người tham gia hội thoại)
alt Có quyền gửi
    chatController -> message: 4. Lưu tin nhắn vào DB
    activate message
    message --> chatController: 5. Lưu thành công
    deactivate message
    chatController -> socket: 6. Broadcast tin nhắn tới đối phương
    chatController --> chatUI: 7. Xác nhận đã gửi
    chatUI --> user: 8. Hiển thị tin nhắn lên giao diện
else Không có quyền gửi
    chatController --> chatUI: 4a. Báo lỗi "Not a participant"
    chatUI --> user: 5a. Hiển thị thông báo lỗi
end
deactivate chatController
deactivate chatUI
@enduml""",

    "UC-35": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Chat" as chatUI
control "Xử lý Chat" as chatController
entity "ConversationParticipant" as participant

user -> chatUI: 1. Mở cửa sổ hội thoại (Đang có tin nhắn mới)
activate chatUI
chatUI -> chatController: 2. Gửi API MarkAsRead
activate chatController
chatController -> participant: 3. Cập nhật LastReadTimestamp
activate participant
participant --> chatController: 4. Cập nhật DB thành công
deactivate participant
chatController --> chatUI: 5. Phản hồi xử lý thành công
chatUI --> user: 6. Xóa huy hiệu Unread (Chấm đỏ)
deactivate chatController
deactivate chatUI
@enduml""",

    "UC-36": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Chat" as chatUI
control "Xử lý Chat" as chatController
entity "Message" as message

user -> chatUI: 1. Cuộn lên trên để xem tin cũ
activate chatUI
chatUI -> chatController: 2. Yêu cầu lấy tin nhắn (LastMessageId)
activate chatController
chatController -> message: 3. Truy vấn DB (Keyset Pagination / Cursor)
activate message
message --> chatController: 4. Trả về 20 tin nhắn tiếp theo
deactivate message
chatController --> chatUI: 5. Phản hồi mảng dữ liệu tin nhắn
chatUI --> user: 6. Hiển thị thêm tin nhắn lên lịch sử trò chuyện
deactivate chatController
deactivate chatUI
@enduml""",

    "UC-37": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Đánh giá" as reviewUI
control "Xử lý Đánh giá" as reviewController
entity "Review" as review

guest -> reviewUI: 1. Nhập Số sao và Nội dung bình luận
activate reviewUI
reviewUI -> reviewController: 2. Gửi yêu cầu lưu Đánh giá
activate reviewController
reviewController -> reviewController: 3. Kiểm tra trạng thái chuyến đi (Completed)
alt Chuyến đi đã hoàn tất
    reviewController -> review: 4. Tạo mới Review (Gắn với BookingId)
    activate review
    review --> reviewController: 5. Lưu thành công
    deactivate review
    reviewController -> reviewController: 6. Phát Event tính toán lại Rating cho Property
    reviewController --> reviewUI: 7. Phản hồi xử lý thành công
    reviewUI --> guest: 8. Hiển thị đánh giá công khai
else Chuyến đi chưa hoàn tất hoặc Đã đánh giá
    reviewController --> reviewUI: 4a. Báo lỗi "Booking must be Completed"
    reviewUI --> guest: 5a. Hiển thị thông báo lỗi
end
deactivate reviewController
deactivate reviewUI
@enduml""",

    "UC-38": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Quản lý Đánh giá" as reviewUI
control "Xử lý Đánh giá" as reviewController
entity "Review" as review

guest -> reviewUI: 1. Chọn Sửa / Xóa bài đánh giá
activate reviewUI
reviewUI -> reviewController: 2. Gửi yêu cầu cập nhật/xóa Đánh giá
activate reviewController
reviewController -> review: 3. Kiểm tra quyền sở hữu Review
activate review
review --> reviewController: 4. Trả về dữ liệu Review
deactivate review
alt Hợp lệ và Phòng đang tồn tại
    reviewController -> review: 5. Thực thi cập nhật / Xóa mềm
    activate review
    review --> reviewController: 6. Xử lý thành công
    deactivate review
    reviewController -> reviewController: 7. Phát Event tính toán lại Rating cho Property
    reviewController --> reviewUI: 8. Phản hồi thành công
    reviewUI --> guest: 9. Hiển thị cập nhật trên màn hình
else Phòng không tồn tại
    reviewController --> reviewUI: 5a. Báo lỗi "Property not found"
    reviewUI --> guest: 6a. Hiển thị thông báo lỗi
end
deactivate reviewController
deactivate reviewUI
@enduml""",

    "UC-39": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Trang Đăng nhập Quản trị" as loginUI
control "Xử lý Đăng nhập" as authController
entity "User" as account

admin -> loginUI: 1. Nhập Email và Mật khẩu (bcrypt)
activate loginUI
loginUI -> authController: 2. Gửi thông tin chứng thực Admin
activate authController
authController -> account: 3. Lấy thông tin tài khoản và Role
activate account
account --> authController: 4. Trả về thông tin Admin
deactivate account
alt Đúng mật khẩu và Có quyền Admin/Moderator
    authController -> authController: 5. Sinh JWT Token (Admin Privilege)
    authController --> loginUI: 6. Phản hồi Đăng nhập thành công
    loginUI --> admin: 7. Chuyển hướng vào Dashboard Quản trị
else Thiếu quyền truy cập
    authController --> loginUI: 5a. Báo lỗi "Access denied. Admin role required"
    loginUI --> admin: 6a. Hiển thị thông báo từ chối
end
deactivate authController
deactivate loginUI
@enduml""",

    "UC-40": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Màn hình Duyệt Bài" as adminUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property

admin -> adminUI: 1. Xem xét phòng (Pending Approval) và nhấn Approve
activate adminUI
adminUI -> propertyController: 2. Gửi yêu cầu phê duyệt Bất động sản
activate propertyController
propertyController -> propertyController: 3. Xác thực quyền Admin
propertyController -> property: 4. Cập nhật trạng thái thành Published
activate property
property --> propertyController: 5. Cập nhật thành công
deactivate property
propertyController -> propertyController: 6. Phát Event sang Kafka (Kích hoạt luồng CDC sang Elasticsearch)
propertyController --> adminUI: 7. Phản hồi duyệt thành công
adminUI --> admin: 8. Hiển thị tin đăng đã công khai
deactivate propertyController
deactivate adminUI
@enduml""",

    "UC-41": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Quản lý Người dùng" as adminUI
control "Xử lý Tài khoản" as userController
entity "User" as account

admin -> adminUI: 1. Chọn User và nhấn Khóa (Suspend)
activate adminUI
adminUI -> userController: 2. Gửi yêu cầu khóa tài khoản
activate userController
userController -> account: 3. Lấy thông tin User
activate account
account --> userController: 4. Trả về thông tin
deactivate account
alt User tồn tại trong hệ thống
    userController -> account: 5. Đổi trạng thái thành Suspended
    activate account
    account --> userController: 6. Lưu thay đổi
    deactivate account
    userController -> userController: 7. Tự động hủy mọi phiên đăng nhập (RevokeSessions)
    userController --> adminUI: 8. Phản hồi khóa thành công
    adminUI --> admin: 9. Hiển thị trạng thái Suspended
else User không tồn tại
    userController --> adminUI: 5a. Báo lỗi "User not found"
    adminUI --> admin: 6a. Hiển thị thông báo lỗi
end
deactivate userController
deactivate adminUI
@enduml""",

    "UC-42": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Quản lý Tiện ích" as adminUI
control "Xử lý Tiện ích" as amenityController
entity "Amenity" as amenity

admin -> adminUI: 1. Nhập Tên tiện ích mới và Icon
activate adminUI
adminUI -> amenityController: 2. Yêu cầu thêm Tiện ích
activate amenityController
amenityController -> amenity: 3. Kiểm tra tính trùng lặp tên Tiện ích
activate amenity
amenity --> amenityController: 4. Phản hồi kết quả kiểm tra
deactivate amenity
alt Tên tiện ích chưa tồn tại
    amenityController -> amenity: 5. Lưu Tiện ích mới vào DB
    activate amenity
    amenity --> amenityController: 6. Lưu thành công
    deactivate amenity
    amenityController --> adminUI: 7. Phản hồi tạo thành công
    adminUI --> admin: 8. Hiển thị tiện ích mới trong danh sách
else Trùng tên
    amenityController --> adminUI: 5a. Báo lỗi "Amenity already exists"
    adminUI --> admin: 6a. Yêu cầu chọn tên khác
end
deactivate amenityController
deactivate adminUI
@enduml""",

    "UC-43": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Xử lý Khiếu nại" as adminUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

admin -> adminUI: 1. Xem xét chứng cứ 2 bên và ra Quyết định
activate adminUI
adminUI -> bookingController: 2. Gửi Quyết định giải quyết khiếu nại (Phạt/Hoàn tiền)
activate bookingController
bookingController -> booking: 3. Truy cập Đơn đặt phòng có tranh chấp
activate booking
booking --> bookingController: 4. Trả về dữ liệu Đơn
deactivate booking
bookingController -> booking: 5. Đóng băng Booking (Giải quyết dứt điểm)
activate booking
booking --> bookingController: 6. Cập nhật thành công
deactivate booking
bookingController -> bookingController: 7. Bắn Event Refund hoặc Phạt Host tương ứng
bookingController --> adminUI: 8. Phản hồi xử lý thành công
adminUI --> admin: 9. Hiển thị Dispute đã đóng
deactivate bookingController
deactivate adminUI
@enduml""",

    "UC-44": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Báo cáo Thống kê" as adminUI
control "Xử lý Thống kê" as analyticsController
entity "Payment" as payment

admin -> adminUI: 1. Chọn khoảng thời gian xem báo cáo
activate adminUI
adminUI -> analyticsController: 2. Yêu cầu xuất dữ liệu doanh thu
activate analyticsController
analyticsController -> payment: 3. Truy vấn Analytics DB (Group By Ngày/Tháng)
activate payment
payment --> analyticsController: 4. Trả về tổng hợp doanh thu
deactivate payment
analyticsController --> adminUI: 5. Phản hồi JSON dữ liệu Chart
adminUI --> admin: 6. Render biểu đồ tăng trưởng
deactivate analyticsController
deactivate adminUI
@enduml""",

    "UC-45": """@startuml
skinparam maxMessageSize 150
actor "Admin" as admin
boundary "Cấu hình Nền tảng" as adminUI
control "Xử lý Cấu hình" as platformController
entity "PlatformSetting" as settings

admin -> adminUI: 1. Nhập phần trăm phí Dịch vụ / Hoa hồng mới
activate adminUI
adminUI -> platformController: 2. Yêu cầu cập nhật cấu hình
activate platformController
platformController -> platformController: 3. Xác thực quyền Root Admin
platformController -> settings: 4. Lưu cấu hình mới (Chỉ áp dụng tương lai)
activate settings
settings --> platformController: 5. Cập nhật DB thành công
deactivate settings
platformController -> platformController: 6. Xóa Cache cấu hình cũ
platformController --> adminUI: 7. Phản hồi cập nhật thành công
adminUI --> admin: 8. Hiển thị thông báo lưu thành công
deactivate platformController
deactivate adminUI
@enduml"""
}
