PUML_DATA_01_10 = {
    "UC-01": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Đăng ký" as registerUI
control "Xử lý Đăng ký" as registerController
entity "User" as account

user -> registerUI: 1. Nhập thông tin đăng ký
activate registerUI
registerUI -> registerController: 2. Gửi yêu cầu đăng ký
activate registerController
registerController -> registerController: 3. Xác thực định dạng dữ liệu
registerController -> account: 4. Kiểm tra email tồn tại
activate account
account --> registerController: 5. Trả về kết quả kiểm tra
deactivate account
alt Email chưa tồn tại
    registerController -> account: 6. Tạo mới tài khoản (Inactive)
    activate account
    account --> registerController: 7. Tạo thành công
    deactivate account
    registerController -> registerController: 8. Tạo mã OTP và Gửi Email
    registerController --> registerUI: 9. Phản hồi yêu cầu nhập OTP
    registerUI --> user: 10. Hiển thị màn hình xác thực OTP
else Email đã tồn tại
    registerController --> registerUI: 6a. Báo lỗi "Email already exists"
    registerUI --> user: 7a. Hiển thị thông báo lỗi
end
deactivate registerController
deactivate registerUI
@enduml""",

    "UC-02": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Xác thực" as verifyUI
control "Xử lý Xác thực" as verifyController
entity "User" as account

user -> verifyUI: 1. Nhập mã OTP
activate verifyUI
verifyUI -> verifyController: 2. Gửi mã OTP để xác thực
activate verifyController
verifyController -> verifyController: 3. Kiểm tra tính hợp lệ của mã OTP
alt OTP hợp lệ và chưa hết hạn
    verifyController -> account: 4. Cập nhật trạng thái tài khoản (Active)
    activate account
    account --> verifyController: 5. Cập nhật thành công
    deactivate account
    verifyController --> verifyUI: 6. Phản hồi xác thực thành công
    verifyUI --> user: 7. Chuyển hướng đến màn hình Đăng nhập
else OTP hết hạn hoặc sai
    verifyController --> verifyUI: 4a. Báo lỗi "Verification code expired / invalid"
    verifyUI --> user: 5a. Hiển thị thông báo lỗi
end
deactivate verifyController
deactivate verifyUI
@enduml""",

    "UC-03": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Đăng nhập" as loginUI
control "Xử lý Đăng nhập" as loginController
entity "User" as account

user -> loginUI: 1. Nhập email và mật khẩu
activate loginUI
loginUI -> loginController: 2. Gửi thông tin đăng nhập
activate loginController
loginController -> account: 3. Lấy thông tin tài khoản theo email
activate account
account --> loginController: 4. Trả về thông tin tài khoản
deactivate account
alt Tài khoản tồn tại và Mật khẩu hợp lệ
    loginController -> loginController: 5. Xác thực mật khẩu
    loginController -> loginController: 6. Tạo phiên đăng nhập (Cấp phát Token)
    loginController --> loginUI: 7. Phản hồi Đăng nhập thành công
    loginUI --> user: 8. Chuyển hướng sang Màn hình chính
else Sai email hoặc mật khẩu
    loginController --> loginUI: 5a. Báo lỗi "Invalid email or password"
    loginUI --> user: 6a. Hiển thị thông báo lỗi
end
deactivate loginController
deactivate loginUI
@enduml""",

    "UC-04": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Hồ sơ" as profileUI
control "Xử lý Hồ sơ" as profileController
entity "User" as account

user -> profileUI: 1. Cập nhật thông tin cá nhân
activate profileUI
profileUI -> profileController: 2. Gửi yêu cầu cập nhật hồ sơ
activate profileController
profileController -> profileController: 3. Xác thực định dạng dữ liệu (Validate)
alt Dữ liệu hợp lệ
    profileController -> account: 4. Cập nhật thông tin vào CSDL
    activate account
    account --> profileController: 5. Cập nhật thành công
    deactivate account
    profileController --> profileUI: 6. Phản hồi Cập nhật thành công
    profileUI --> user: 7. Hiển thị thông báo thành công
else Lỗi định dạng (Validation Failed)
    profileController --> profileUI: 4a. Phản hồi lỗi định dạng
    profileUI --> user: 5a. Hiển thị thông báo lỗi
end
deactivate profileController
deactivate profileUI
@enduml""",

    "UC-05": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Đổi mật khẩu" as passwordUI
control "Xử lý Đổi mật khẩu" as passwordController
entity "User" as account

user -> passwordUI: 1. Nhập mật khẩu cũ và mật khẩu mới
activate passwordUI
passwordUI -> passwordController: 2. Gửi yêu cầu đổi mật khẩu
activate passwordController
passwordController -> account: 3. Lấy thông tin tài khoản hiện tại
activate account
account --> passwordController: 4. Trả về thông tin tài khoản
deactivate account
alt Mật khẩu cũ chính xác và Mật khẩu mới hợp lệ
    passwordController -> passwordController: 5. Xác thực mật khẩu cũ
    passwordController -> account: 6. Cập nhật mật khẩu mới
    activate account
    account --> passwordController: 7. Cập nhật thành công
    deactivate account
    passwordController -> passwordController: 8. Hủy các phiên đăng nhập khác
    passwordController --> passwordUI: 9. Phản hồi Đổi mật khẩu thành công
    passwordUI --> user: 10. Hiển thị thông báo thành công
else Sai mật khẩu cũ hoặc không đạt chuẩn
    passwordController --> passwordUI: 5a. Báo lỗi sai mật khẩu hoặc mật khẩu yếu
    passwordUI --> user: 6a. Hiển thị thông báo lỗi
end
deactivate passwordController
deactivate passwordUI
@enduml""",

    "UC-06": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Màn hình Quên mật khẩu" as forgotPwdUI
control "Xử lý Quên mật khẩu" as forgotPwdController
entity "User" as account

user -> forgotPwdUI: 1. Nhập email yêu cầu khôi phục
activate forgotPwdUI
forgotPwdUI -> forgotPwdController: 2. Gửi yêu cầu quên mật khẩu
activate forgotPwdController
forgotPwdController -> account: 3. Tìm tài khoản theo email
activate account
account --> forgotPwdController: 4. Trả về kết quả tìm kiếm
deactivate account
alt Email có tồn tại trong hệ thống
    forgotPwdController -> forgotPwdController: 5. Tạo Reset Token
    forgotPwdController -> account: 6. Lưu Token vào tài khoản
    activate account
    account --> forgotPwdController: 7. Lưu thành công
    deactivate account
    forgotPwdController -> forgotPwdController: 8. Gửi email chứa link khôi phục
    forgotPwdController --> forgotPwdUI: 9. Phản hồi xử lý thành công (200 OK)
    forgotPwdUI --> user: 10. Thông báo kiểm tra email
else Email không tồn tại
    forgotPwdController --> forgotPwdUI: 5a. Phản hồi xử lý thành công (200 OK)
    forgotPwdUI --> user: 6a. Thông báo kiểm tra email (Ẩn danh)
end
deactivate forgotPwdController
deactivate forgotPwdUI
@enduml""",

    "UC-07": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Hệ thống ngầm (Client)" as clientUI
control "Xử lý Xác thực" as authController
entity "UserSession" as session

user -> clientUI: 1. Truy cập tính năng khi Token hết hạn
activate clientUI
clientUI -> authController: 2. Gửi Refresh Token yêu cầu làm mới
activate authController
authController -> session: 3. Kiểm tra tính hợp lệ của Refresh Token
activate session
session --> authController: 4. Trả về trạng thái Token
deactivate session
alt Token hợp lệ và chưa bị thu hồi
    authController -> session: 5. Thu hồi Token cũ và Tạo Token mới (Rotation)
    activate session
    session --> authController: 6. Cập nhật phiên đăng nhập thành công
    deactivate session
    authController --> clientUI: 7. Trả về bộ Token mới
    clientUI --> user: 8. Tự động tiếp tục tính năng đang dùng
else Token không hợp lệ hoặc bị thu hồi
    authController --> clientUI: 5a. Báo lỗi "Token is revoked / invalid"
    clientUI --> user: 6a. Chuyển hướng người dùng về trang Đăng nhập
end
deactivate authController
deactivate clientUI
@enduml""",

    "UC-08": """@startuml
skinparam maxMessageSize 150
actor "Người dùng" as user
boundary "Giao diện Thông báo" as notificationUI
control "Xử lý Thông báo" as notificationController
entity "Notification" as notification

user -> notificationUI: 1. Bấm mở chuông thông báo
activate notificationUI
notificationUI -> notificationController: 2. Yêu cầu lấy danh sách thông báo
activate notificationController
notificationController -> notification: 3. Truy vấn danh sách thông báo chưa đọc
activate notification
notification --> notificationController: 4. Trả về danh sách thông báo
deactivate notification
notificationController --> notificationUI: 5. Phản hồi danh sách thông báo
notificationUI --> user: 6. Hiển thị danh sách lên màn hình

user -> notificationUI: 7. Nhấn xem một thông báo cụ thể
notificationUI -> notificationController: 8. Gửi yêu cầu đánh dấu đã đọc
notificationController -> notification: 9. Cập nhật trạng thái (IsRead = true)
activate notification
notification --> notificationController: 10. Cập nhật thành công
deactivate notification
notificationController --> notificationUI: 11. Phản hồi xử lý thành công
notificationUI --> user: 12. Ẩn dấu chấm đỏ (Unread badge)

deactivate notificationController
deactivate notificationUI
@enduml""",

    "UC-09": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Host" as hostUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property

host -> hostUI: 1. Nhập thông tin cơ bản (Tiêu đề, mô tả)
activate hostUI
hostUI -> propertyController: 2. Yêu cầu khởi tạo Bất động sản mới
activate propertyController
propertyController -> propertyController: 3. Xác thực định dạng dữ liệu (Validate)
alt Dữ liệu đầy đủ và hợp lệ
    propertyController -> property: 4. Tạo mới bản ghi Bất động sản (Trạng thái: Draft)
    activate property
    property --> propertyController: 5. Khởi tạo thành công (Trả về Property ID)
    deactivate property
    propertyController --> hostUI: 6. Phản hồi tạo thành công
    hostUI --> host: 7. Chuyển sang bước Tải hình ảnh
else Dữ liệu thiếu hoặc sai định dạng
    propertyController --> hostUI: 4a. Báo lỗi "Validation Failed"
    hostUI --> host: 5a. Hiển thị lỗi yêu cầu nhập lại
end
deactivate propertyController
deactivate hostUI
@enduml""",

    "UC-10": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Host" as hostUI
control "Xử lý Bất động sản" as propertyController
entity "PropertyImage" as property

host -> hostUI: 1. Tải lên hình ảnh và chọn Tiện ích
activate hostUI
hostUI -> propertyController: 2. Gửi yêu cầu cập nhật hình ảnh và tiện ích
activate propertyController
propertyController -> propertyController: 3. Kiểm tra định dạng và số lượng hình ảnh
alt Hình ảnh hợp lệ (JPG/PNG và tối thiểu 5 ảnh)
    propertyController -> propertyController: 4. Xử lý tải hình ảnh lên Cloud
    propertyController -> property: 5. Cập nhật danh sách URL hình ảnh và tiện ích
    activate property
    property --> propertyController: 6. Cập nhật thành công
    deactivate property
    propertyController --> hostUI: 7. Phản hồi cập nhật thành công
    hostUI --> host: 8. Hiển thị thông báo thành công
else Sai định dạng hoặc thiếu số lượng
    propertyController --> hostUI: 4a. Từ chối upload và báo lỗi
    hostUI --> host: 5a. Yêu cầu tải lại ảnh hợp lệ
end
deactivate propertyController
deactivate hostUI
@enduml"""
}
