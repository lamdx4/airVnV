# Task: Thông báo thời gian thực (Real-time Firebase Notifications)

## Mô tả
Sử dụng Firebase Cloud Messaging (FCM) để gửi thông báo đẩy đến Guest và Host khi có sự kiện quan trọng (Đặt phòng thành công, Thanh toán thất bại, v.v.).

## Mục tiêu
- Tích hợp Firebase Admin SDK đã cấu hình trong `UserService`.
- Gửi thông báo tự động dựa trên các Event từ Kafka.

## Acceptance Criteria
- [ ] Code Service xử lý gửi Push Message qua FCM.
- [ ] Lắng nghe Kafka các sự kiện: `BookingConfirmed`, `PaymentFailed`.
- [ ] Lưu trữ và quản lý `FCM Token` của User trong `userdb`.

## Đầu vào
- `user_id`, `title`, `body`.
- FCM Token hợp lệ.

## Đầu ra
- Thông báo hiển thị trên thiết bị người dùng.

## Ưu tiên
Low

## Ước lượng
2 days
