# Task: Tích hợp Cổng thanh toán VNPay (VNPay Integration)

## Mô tả
Kết nối `PaymentService` với cổng thanh toán VNPay để thực hiện thanh toán thực tế cho các đơn đặt phòng.

## Mục tiêu
- Tạo URL thanh toán VNPay dựa trên thông tin đơn hàng.
- Xử lý IPN (Instant Payment Notification) từ VNPay để cập nhật trạng thái đơn hàng.

## Acceptance Criteria
- [ ] Implement VNPay Hash logic (HmacSHA512).
- [ ] API `POST /api/payments/create-url` trả về link redirect sang VNPay.
- [ ] API `GET /api/payments/vnpay-return` xử lý callback từ trình duyệt.
- [ ] API `POST /api/payments/vnpay-ipn` cập nhật Database an toàn (kiểm tra chữ ký, kiểm tra số tiền).

## Đầu vào
- `booking_id`, `amount`, `order_info`.

## Đầu ra
- URL redirect VNPay.
- Trạng thái thanh toán cập nhật vào `paydb`.

## Ưu tiên
High

## Ước lượng
4 days
