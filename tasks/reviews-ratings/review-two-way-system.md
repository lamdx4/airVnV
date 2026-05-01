# Task: Hệ thống Đánh giá 2 chiều (Two-way Review System)

## Mô tả
Xây dựng module đánh giá cho phép Guest và Host nhận xét về nhau sau khi chuyến đi kết thúc.

## Mục tiêu
- Lưu trữ đánh giá (Rating & Comment).
- Chỉ cho phép đánh giá khi Booking ở trạng thái `Completed`.
- Ngăn chặn đánh giá trùng lặp cho cùng một Booking.

## Acceptance Criteria
- [ ] Thiết kế Schema cho bảng `Reviews` (có thể đặt trong `PropertyService` hoặc Service mới).
- [ ] API `POST /api/reviews` kiểm tra trạng thái `Booking` từ `BookingService`.
- [ ] Logic: Guest chỉ đánh giá sau ngày Check-out.

## Đầu vào
- `booking_id`, `rating` (1-5), `comment`.

## Đầu ra
- Bản ghi đánh giá mới.
- Cập nhật điểm rating trung bình của Property/User.

## Ưu tiên
Medium

## Ước lượng
3 days
