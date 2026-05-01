# Task: Quản lý Lịch và Giá theo ngày (Property Calendar & Pricing)

## Mô tả
Triển khai hệ thống quản lý lịch trống và cấu hình giá linh hoạt cho Host. Host có thể chọn các ngày cụ thể để chặn (Block) hoặc thay đổi giá thuê khác với giá cơ bản (Base Price).

## Mục tiêu
- Cho phép Host cập nhật trạng thái phòng (Available/Blocked) theo ngày.
- Cho phép Host thiết lập giá (Custom Price) cho các ngày đặc biệt (Lễ, cuối tuần).
- Cung cấp API trả về lịch trạng thái trong một khoảng thời gian.

## Acceptance Criteria
- [ ] Tạo bảng `PropertyAvailability` trong `propertydb`.
- [ ] API `PUT /api/properties/{id}/calendar` nhận danh sách ngày và giá/trạng thái.
- [ ] Logic kiểm tra: Chỉ Host của Property mới có quyền cập nhật.
- [ ] API `GET /api/properties/{id}/calendar?start=...&end=...` trả về mảng dữ liệu lịch.

## Đầu vào
- `property_id` (Guid)
- Mảng `CalendarEntries` { `Date`, `Price`, `IsBlocked` }

## Đầu ra
- Trạng thái thành công/thất bại.
- Dữ liệu lịch đã được cập nhật.

## Ưu tiên
High

## Ước lượng
3 days
