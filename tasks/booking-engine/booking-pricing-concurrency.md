# Task: Logic Tính giá và Chống Overbooking (Booking Logic & Concurrency)

## Mô tả
Xử lý logic đặt phòng phức tạp: tính toán tổng tiền dựa trên giá ngày và triển khai cơ chế khóa chỗ tạm thời để tránh tình trạng nhiều người đặt cùng 1 phòng.

## Mục tiêu
- Tính toán chính xác `TotalPrice` dựa trên dữ liệu từ `PropertyAvailability`.
- Triển khai "Pending Booking" và cơ chế Lock inventory trong Redis (hoặc Database lock) trong 10-15 phút khi Guest đang thanh toán.

## Acceptance Criteria
- [ ] API `POST /api/bookings` thực hiện check lịch trống trước khi tạo.
- [ ] Logic tính giá: Sum(daily_price) + cleaning_fee + service_fee.
- [ ] Sử dụng Distributed Lock (Redis) hoặc Pessimistic Lock trên Postgres để chặn các request trùng lặp ngày.

## Đầu vào
- `property_id`, `check_in`, `check_out`.

## Đầu ra
- `Booking` ở trạng thái `Pending`.
- Tổng tiền cần thanh toán.

## Ưu tiên
High

## Ước lượng
5 days
