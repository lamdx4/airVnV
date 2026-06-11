PUML_DATA_21_30 = {
    "UC-21": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Đặt phòng" as bookingUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

guest -> bookingUI: 1. Chọn ngày, số người và Đặt phòng
activate bookingUI
bookingUI -> bookingController: 2. Gửi yêu cầu đặt phòng
activate bookingController
bookingController -> booking: 3. Kiểm tra tính khả dụng (Lịch trùng)
activate booking
booking --> bookingController: 4. Phản hồi tính khả dụng
deactivate booking
alt Lịch còn trống
    bookingController -> booking: 5. Tạo đơn đặt phòng mới (Trạng thái: Pending)
    activate booking
    booking --> bookingController: 6. Khởi tạo thành công
    deactivate booking
    bookingController -> bookingController: 7. Bắn Event sang PaymentService
    bookingController --> bookingUI: 8. Phản hồi tạo đơn thành công
    bookingUI --> guest: 9. Chuyển hướng sang trang Thanh toán
else Ngày đã bị đặt
    bookingController --> bookingUI: 5a. Báo lỗi "These dates are already booked"
    bookingUI --> guest: 6a. Hiển thị thông báo yêu cầu chọn ngày khác
end
deactivate bookingController
deactivate bookingUI
@enduml""",

    "UC-22": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Đơn" as hostUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

host -> hostUI: 1. Xem đơn đặt phòng chờ duyệt
activate hostUI
hostUI -> bookingController: 2. Chọn Duyệt (Approve) đơn
activate bookingController
bookingController -> booking: 3. Lấy thông tin đơn đặt phòng
activate booking
booking --> bookingController: 4. Trả về thông tin đơn (Kiểm tra quyền Host)
deactivate booking
alt Đơn tồn tại và thuộc quyền Host
    bookingController -> booking: 5. Cập nhật trạng thái (Approved)
    activate booking
    booking --> bookingController: 6. Cập nhật thành công
    deactivate booking
    bookingController -> bookingController: 7. Gửi thông báo cho Khách
    bookingController --> hostUI: 8. Phản hồi xử lý thành công
    hostUI --> host: 9. Cập nhật trạng thái hiển thị
else Đơn không tồn tại hoặc không đủ quyền
    bookingController --> hostUI: 5a. Báo lỗi "Booking not found / Unauthorized"
    hostUI --> host: 6a. Hiển thị thông báo lỗi
end
deactivate bookingController
deactivate hostUI
@enduml""",

    "UC-23": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Lịch sử Đặt phòng" as tripsUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

guest -> tripsUI: 1. Nhấn nút Hủy phòng
activate tripsUI
tripsUI -> bookingController: 2. Gửi yêu cầu hủy đơn
activate bookingController
bookingController -> booking: 3. Lấy thông tin đơn và ngày Check-in
activate booking
booking --> bookingController: 4. Trả về thông tin đơn
deactivate booking
alt Trong thời hạn cho phép hủy
    bookingController -> bookingController: 5. Tính toán số tiền hoàn lại theo Policy
    bookingController -> booking: 6. Cập nhật trạng thái (Cancelled)
    activate booking
    booking --> bookingController: 7. Cập nhật thành công
    deactivate booking
    bookingController -> bookingController: 8. Bắn RefundEvent sang PaymentService
    bookingController --> tripsUI: 9. Phản hồi hủy thành công
    tripsUI --> guest: 10. Hiển thị thông báo hủy và tiền hoàn
else Quá thời hạn (Ví dụ: Sát giờ check-in)
    bookingController --> tripsUI: 5a. Báo lỗi vi phạm chính sách hủy
    tripsUI --> guest: 6a. Hiển thị thông báo từ chối hủy
end
deactivate bookingController
deactivate tripsUI
@enduml""",

    "UC-24": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Đơn" as hostUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

host -> hostUI: 1. Chọn hủy đơn khách đã đặt và nhập lý do
activate hostUI
hostUI -> bookingController: 2. Gửi yêu cầu hủy đơn từ phía Host
activate bookingController
bookingController -> booking: 3. Lấy thông tin đơn đặt phòng
activate booking
booking --> bookingController: 4. Trả về thông tin đơn
deactivate booking
alt Chưa qua ngày Check-in
    bookingController -> booking: 5. Cập nhật trạng thái (Cancelled by Host)
    activate booking
    booking --> bookingController: 6. Cập nhật thành công
    deactivate booking
    bookingController -> bookingController: 7. Bắn Event Phạt Host (Penalty)
    bookingController -> bookingController: 8. Bắn Event Hoàn tiền 100% cho Khách
    bookingController --> hostUI: 9. Phản hồi hủy thành công
    hostUI --> host: 10. Hiển thị cảnh báo đã bị trừ phí phạt
else Khách đã Check-in
    bookingController --> hostUI: 5a. Báo lỗi không thể hủy khi khách đã nhận phòng
    hostUI --> host: 6a. Hiển thị thông báo lỗi
end
deactivate bookingController
deactivate hostUI
@enduml""",

    "UC-25": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Lịch sử Chuyến đi" as tripsUI
control "Xử lý Đặt phòng" as bookingController
entity "Booking" as booking

guest -> tripsUI: 1. Truy cập mục Chuyến đi (Trips)
activate tripsUI
tripsUI -> bookingController: 2. Yêu cầu lấy danh sách lịch sử đặt phòng
activate bookingController
bookingController -> booking: 3. Truy vấn DB lấy lịch sử Booking của Khách
activate booking
booking --> bookingController: 4. Trả về danh sách đơn
deactivate booking
bookingController -> bookingController: 5. Phân loại đơn (Upcoming / Past)
bookingController --> tripsUI: 6. Phản hồi dữ liệu phân trang
tripsUI --> guest: 7. Hiển thị danh sách chuyến đi sắp tới và quá khứ
deactivate bookingController
deactivate tripsUI
@enduml""",

    "UC-26": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Thanh toán" as paymentUI
control "Xử lý Thanh toán" as paymentController
entity "Payment" as payment
participant "VNPay API" as vnpay

guest -> paymentUI: 1. Nhấn nút Xác nhận thanh toán
activate paymentUI
paymentUI -> paymentController: 2. Gửi yêu cầu khởi tạo thanh toán (Idempotency Key)
activate paymentController
paymentController -> payment: 3. Kiểm tra trạng thái hóa đơn
activate payment
payment --> paymentController: 4. Trả về trạng thái
deactivate payment
alt Chưa thanh toán hoặc Đang chờ
    paymentController -> paymentController: 5. Khởi tạo dữ liệu giao dịch
    paymentController -> vnpay: 6. Gọi API khởi tạo Session
    activate vnpay
    vnpay --> paymentController: 7. Trả về URL thanh toán VNPay
    deactivate vnpay
    paymentController --> paymentUI: 8. Phản hồi URL thanh toán
    paymentUI --> guest: 9. Chuyển hướng sang VNPay
else Đã thanh toán / Đang xử lý
    paymentController --> paymentUI: 5a. Báo lỗi "Payment is already processed"
    paymentUI --> guest: 6a. Thông báo lỗi chống trùng lặp
end
deactivate paymentController
deactivate paymentUI
@enduml""",

    "UC-27": """@startuml
skinparam maxMessageSize 150
actor "VNPay" as vnpay
boundary "Payment Webhook Endpoint" as webhookUI
control "Xử lý Thanh toán" as paymentController
entity "Payment" as payment

vnpay -> webhookUI: 1. Gửi kết quả giao dịch (IPN Payload)
activate webhookUI
webhookUI -> paymentController: 2. Chuyển tiếp payload xử lý
activate paymentController
paymentController -> paymentController: 3. Xác thực chữ ký số (Secure Hash)
alt Chữ ký hợp lệ
    paymentController -> payment: 4. Cập nhật trạng thái giao dịch (Success/Failed)
    activate payment
    payment --> paymentController: 5. Cập nhật DB thành công
    deactivate payment
    paymentController -> paymentController: 6. Phát sự kiện PaymentCompletedEvent sang BookingService
    paymentController --> webhookUI: 7. Phản hồi 200 OK cho VNPay
    webhookUI --> vnpay: 8. Trả về kết quả
else Sai chữ ký (Giả mạo)
    paymentController -> paymentController: 4a. Ghi Log cảnh báo bảo mật
    paymentController --> webhookUI: 5a. Bỏ qua Payload (400 Bad Request)
    webhookUI --> vnpay: 6a. Phản hồi lỗi
end
deactivate paymentController
deactivate webhookUI
@enduml""",

    "UC-28": """@startuml
skinparam maxMessageSize 150
actor "Hệ thống" as system
boundary "Job Hoàn tiền (Background)" as refundJob
control "Xử lý Hoàn tiền" as refundController
entity "RefundRecord" as refundRecord
participant "VNPay API" as vnpay

system -> refundJob: 1. Lắng nghe RefundEvent (Từ Booking bị hủy)
activate refundJob
refundJob -> refundController: 2. Khởi tạo tiến trình hoàn tiền
activate refundController
refundController -> refundRecord: 3. Ghi nhận giao dịch hoàn tiền (Pending)
activate refundRecord
refundRecord --> refundController: 4. Khởi tạo thành công
deactivate refundRecord
refundController -> vnpay: 5. Gọi API Hoàn tiền của cổng thanh toán
activate vnpay
alt VNPay phản hồi thành công
    vnpay --> refundController: 6. Trả về kết quả hoàn tiền thành công
    refundController -> refundRecord: 7. Cập nhật trạng thái Refunded (Lưu Transaction ID)
    activate refundRecord
    refundRecord --> refundController: 8. Cập nhật DB thành công
    deactivate refundRecord
else VNPay báo lỗi hoặc timeout
    vnpay --> refundController: 6a. Báo lỗi API
    refundController -> refundController: 7a. Lưu tác vụ vào Outbox để Retry sau
end
deactivate vnpay
refundController --> refundJob: 9. Hoàn thành tiến trình
deactivate refundController
deactivate refundJob
@enduml""",

    "UC-29": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Ví Điện tử" as walletUI
control "Xử lý Ví Host" as paymentController
entity "HostBalance" as balance

host -> walletUI: 1. Truy cập Ví thu nhập (Wallet)
activate walletUI
walletUI -> paymentController: 2. Yêu cầu lấy thông tin số dư
activate paymentController
paymentController -> balance: 3. Truy vấn bảng Balance theo Host ID
activate balance
balance --> paymentController: 4. Trả về dữ liệu ví
deactivate balance
paymentController -> paymentController: 5. Phân tách (Thu nhập tạm tính / Available Payout)
paymentController --> walletUI: 6. Phản hồi thông tin số dư
walletUI --> host: 7. Hiển thị doanh thu và số tiền có thể rút
deactivate paymentController
deactivate walletUI
@enduml""",

    "UC-30": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Ví Điện tử" as walletUI
control "Xử lý Ví Host" as paymentController
entity "HostBalance" as balance

host -> walletUI: 1. Nhập Số tài khoản và Ngân hàng
activate walletUI
walletUI -> paymentController: 2. Gửi yêu cầu lưu thông tin Payout
activate paymentController
paymentController -> paymentController: 3. Kiểm tra định dạng ngân hàng
alt Dữ liệu hợp lệ
    paymentController -> paymentController: 4. Mã hóa Số tài khoản (AES256)
    paymentController -> balance: 5. Lưu thông tin thanh toán vào DB
    activate balance
    balance --> paymentController: 6. Lưu thành công
    deactivate balance
    paymentController --> walletUI: 7. Phản hồi cập nhật thành công
    walletUI --> host: 8. Hiển thị thông báo thành công
else Số tài khoản sai định dạng
    paymentController --> walletUI: 4a. Báo lỗi "Invalid Bank Account Format"
    walletUI --> host: 5a. Yêu cầu nhập lại
end
deactivate paymentController
deactivate walletUI
@enduml"""
}
