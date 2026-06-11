PUML_DATA_11_20 = {
    "UC-11": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Giá" as pricingUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property

host -> pricingUI: 1. Chọn ngày và nhập giá tùy chỉnh
activate pricingUI
pricingUI -> propertyController: 2. Gửi yêu cầu cập nhật giá
activate propertyController
propertyController -> propertyController: 3. Kiểm tra tính hợp lệ của giá
alt Giá hợp lệ (Lớn hơn 0)
    propertyController -> property: 4. Cập nhật bảng giá tùy chỉnh (Smart Pricing)
    activate property
    property --> propertyController: 5. Cập nhật thành công
    deactivate property
    propertyController --> pricingUI: 6. Phản hồi thiết lập thành công
    pricingUI --> host: 7. Hiển thị thông báo thành công
else Giá không hợp lệ
    propertyController --> pricingUI: 4a. Báo lỗi giá không hợp lệ
    pricingUI --> host: 5a. Hiển thị cảnh báo lỗi
end
deactivate propertyController
deactivate pricingUI
@enduml""",

    "UC-12": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Lịch" as calendarUI
control "Xử lý Bất động sản" as propertyController
entity "Booking" as booking
entity "PropertyAvailability" as availability

host -> calendarUI: 1. Chọn khoảng thời gian cần khóa
activate calendarUI
calendarUI -> propertyController: 2. Gửi yêu cầu khóa lịch rảnh
activate propertyController
propertyController -> booking: 3. Kiểm tra các đơn đặt phòng (Pending/Confirmed) trong khoảng ngày
activate booking
booking --> propertyController: 4. Trả về danh sách đơn đặt phòng
deactivate booking
alt Không có đơn đặt phòng trùng lịch
    propertyController -> availability: 5. Tạo mới BlockAvailability
    activate availability
    availability --> propertyController: 6. Khóa lịch thành công
    deactivate availability
    propertyController --> calendarUI: 7. Phản hồi khóa lịch thành công
    calendarUI --> host: 8. Cập nhật giao diện lịch (Màu đỏ/Khóa)
else Có đơn đặt phòng trùng lịch
    propertyController --> calendarUI: 5a. Báo lỗi không thể khóa ngày đã có khách đặt
    calendarUI --> host: 6a. Hiển thị thông báo lỗi
end
deactivate propertyController
deactivate calendarUI
@enduml""",

    "UC-13": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Host" as hostUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property
entity "PropertyImage" as propertyImage

host -> hostUI: 1. Chọn xóa phòng đang nháp
activate hostUI
hostUI -> propertyController: 2. Gửi yêu cầu xóa Bất động sản
activate propertyController
propertyController -> property: 3. Lấy thông tin trạng thái phòng
activate property
property --> propertyController: 4. Trả về trạng thái hiện tại
deactivate property
alt Trạng thái là Draft
    propertyController -> property: 5. Cập nhật trạng thái (IsDeleted = true)
    activate property
    property --> propertyController: 6. Soft Delete thành công
    deactivate property
    propertyController -> propertyImage: 7. Gửi yêu cầu xóa hình ảnh (Cloud)
    propertyController --> hostUI: 8. Phản hồi xóa thành công
    hostUI --> host: 9. Xóa phòng khỏi danh sách hiển thị
else Trạng thái đã Publish
    propertyController --> hostUI: 5a. Báo lỗi "Cannot delete published property"
    hostUI --> host: 6a. Hiển thị thông báo yêu cầu Suspend trước
end
deactivate propertyController
deactivate hostUI
@enduml""",

    "UC-14": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Host" as hostUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property

host -> hostUI: 1. Chọn phòng nháp và nhấn Submit
activate hostUI
hostUI -> propertyController: 2. Gửi yêu cầu duyệt Bất động sản
activate propertyController
propertyController -> property: 3. Lấy toàn bộ thông tin phòng
activate property
property --> propertyController: 4. Trả về thông tin phòng
deactivate property
propertyController -> propertyController: 5. Validate dữ liệu toàn diện (Giá, Địa chỉ, Ảnh)
alt Dữ liệu đầy đủ và trạng thái Draft
    propertyController -> property: 6. Chuyển trạng thái (Pending Approval)
    activate property
    property --> propertyController: 7. Cập nhật thành công
    deactivate property
    propertyController -> propertyController: 8. Gửi thông báo cho Admin
    propertyController --> hostUI: 9. Phản hồi gửi duyệt thành công
    hostUI --> host: 10. Cập nhật trạng thái hiển thị
else Thiếu dữ liệu hoặc sai trạng thái
    propertyController --> hostUI: 6a. Báo lỗi thiếu thông tin bắt buộc
    hostUI --> host: 7a. Hiển thị thông báo yêu cầu bổ sung
end
deactivate propertyController
deactivate hostUI
@enduml""",

    "UC-15": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Quản lý Host" as hostUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property

host -> hostUI: 1. Chọn tạm ngưng (Suspend) hoặc Mở lại (Reinstate)
activate hostUI
hostUI -> propertyController: 2. Gửi yêu cầu cập nhật trạng thái phòng
activate propertyController
propertyController -> property: 3. Lấy trạng thái hiện tại
activate property
property --> propertyController: 4. Trả về trạng thái
deactivate property
alt Trạng thái hợp lệ (Published hoặc Suspended)
    propertyController -> property: 5. Cập nhật trạng thái mới
    activate property
    property --> propertyController: 6. Cập nhật DB thành công
    deactivate property
    propertyController -> propertyController: 7. Bắn Event Kafka đồng bộ sang SearchService
    propertyController --> hostUI: 8. Phản hồi cập nhật thành công
    hostUI --> host: 9. Hiển thị trạng thái mới
else Trạng thái không hợp lệ (vd: Draft)
    propertyController --> hostUI: 5a. Báo lỗi "Cannot revert to Draft"
    hostUI --> host: 6a. Hiển thị thông báo lỗi
end
deactivate propertyController
deactivate hostUI
@enduml""",

    "UC-16": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Dashboard Chủ nhà" as dashboardUI
control "Xử lý Thống kê" as analyticsController
entity "Booking" as booking
entity "PropertyDoc" as propertyDoc

host -> dashboardUI: 1. Truy cập trang Thống kê
activate dashboardUI
dashboardUI -> analyticsController: 2. Yêu cầu lấy dữ liệu Dashboard (Tháng này)
activate analyticsController
analyticsController -> booking: 3. Truy vấn tổng số đơn đặt phòng và doanh thu
activate booking
booking --> analyticsController: 4. Trả về số liệu thống kê Booking
deactivate booking
analyticsController -> propertyDoc: 5. Truy vấn lượt xem từ SearchService (Elasticsearch)
activate propertyDoc
propertyDoc --> analyticsController: 6. Trả về số liệu lượt xem
deactivate propertyDoc
analyticsController -> analyticsController: 7. Tổng hợp và lưu Cache (Redis)
analyticsController --> dashboardUI: 8. Trả về dữ liệu Analytics
dashboardUI --> host: 9. Hiển thị biểu đồ thu nhập và lượt xem
deactivate analyticsController
deactivate dashboardUI
@enduml""",

    "UC-17": """@startuml
skinparam maxMessageSize 150
actor "Chủ nhà" as host
boundary "Màn hình Đánh giá" as reviewUI
control "Xử lý Đánh giá" as reviewController
entity "Review" as review

host -> reviewUI: 1. Nhập nội dung phản hồi đánh giá
activate reviewUI
reviewUI -> reviewController: 2. Gửi nội dung phản hồi
activate reviewController
reviewController -> review: 3. Kiểm tra đánh giá hiện tại
activate review
review --> reviewController: 4. Trả về dữ liệu đánh giá
deactivate review
alt Đánh giá chưa có phản hồi
    reviewController -> reviewController: 5. Kiểm tra từ ngữ thô tục
    reviewController -> review: 6. Cập nhật nội dung phản hồi (HostReply)
    activate review
    review --> reviewController: 7. Lưu DB thành công
    deactivate review
    reviewController -> reviewController: 8. Bắn sự kiện gửi thông báo cho Khách
    reviewController --> reviewUI: 9. Phản hồi xử lý thành công
    reviewUI --> host: 10. Hiển thị phản hồi công khai
else Đã phản hồi trước đó
    reviewController --> reviewUI: 5a. Báo lỗi "Host already replied to this review"
    reviewUI --> host: 6a. Hiển thị thông báo lỗi
end
deactivate reviewController
deactivate reviewUI
@enduml""",

    "UC-18": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Tìm kiếm" as searchUI
control "Xử lý Tìm kiếm" as searchController
entity "PropertyDoc" as propertyDoc

guest -> searchUI: 1. Nhập tiêu chí (Địa điểm, giá, ngày) và nhấn Tìm
activate searchUI
searchUI -> searchController: 2. Gửi yêu cầu tìm kiếm Bất động sản
activate searchController
searchController -> searchController: 3. Khởi tạo truy vấn tìm kiếm
searchController -> propertyDoc: 4. Truy vấn Elasticsearch (Chỉ phòng Published)
activate propertyDoc
propertyDoc --> searchController: 5. Trả về danh sách kết quả phù hợp
deactivate propertyDoc
alt Truy vấn thành công
    searchController --> searchUI: 6. Phản hồi dữ liệu phân trang
    searchUI --> guest: 7. Hiển thị danh sách phòng trống
else Lỗi Elasticsearch
    searchController -> searchController: 6a. Chuyển hướng truy vấn Fallback (PostgreSQL)
    searchController --> searchUI: 7a. Phản hồi kết quả Fallback hoặc Lỗi 500
    searchUI --> guest: 8a. Hiển thị kết quả hoặc thông báo lỗi
end
deactivate searchController
deactivate searchUI
@enduml""",

    "UC-19": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Thanh Tìm kiếm" as searchBarUI
control "Xử lý Tìm kiếm" as searchController
entity "PropertyDoc" as propertyDoc

guest -> searchBarUI: 1. Gõ chữ cái đầu vào ô Tìm kiếm
activate searchBarUI
searchBarUI -> searchController: 2. Gửi từ khóa gợi ý (Autocomplete)
activate searchController
searchController -> propertyDoc: 3. Tìm kiếm gần đúng (Fuzzy Match) trên Elasticsearch
activate propertyDoc
propertyDoc --> searchController: 4. Trả về Top 5 địa danh / tên phòng
deactivate propertyDoc
searchController --> searchBarUI: 5. Phản hồi danh sách gợi ý (< 50ms)
searchBarUI --> guest: 6. Hiển thị danh sách Autocomplete thả xuống
deactivate searchController
deactivate searchBarUI
@enduml""",

    "UC-20": """@startuml
skinparam maxMessageSize 150
actor "Khách" as guest
boundary "Màn hình Chi tiết Phòng" as propertyDetailUI
control "Xử lý Bất động sản" as propertyController
entity "Property" as property
entity "Review" as review

guest -> propertyDetailUI: 1. Chọn xem một phòng cụ thể
activate propertyDetailUI
propertyDetailUI -> propertyController: 2. Yêu cầu lấy thông tin chi tiết phòng
activate propertyController
propertyController -> property: 3. Lấy thông tin phòng, giá, tiện ích
activate property
property --> propertyController: 4. Trả về thông tin
deactivate property
alt Phòng tồn tại và trạng thái Published
    propertyController -> review: 5. Lấy danh sách đánh giá của phòng
    activate review
    review --> propertyController: 6. Trả về danh sách đánh giá
    deactivate review
    propertyController --> propertyDetailUI: 7. Phản hồi dữ liệu chi tiết
    propertyDetailUI --> guest: 8. Hiển thị toàn bộ thông tin
else Phòng không tồn tại hoặc bị ẩn
    propertyController --> propertyDetailUI: 5a. Báo lỗi "Property not found (404)"
    propertyDetailUI --> guest: 6a. Chuyển hướng về trang Lỗi 404
end
deactivate propertyController
deactivate propertyDetailUI
@enduml"""
}
