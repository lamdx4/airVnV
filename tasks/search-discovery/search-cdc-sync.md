# Task: Đồng bộ dữ liệu tự động (CDC Sync via Debezium)

## Mô tả
Thiết lập luồng tự động đồng bộ dữ liệu từ Postgres (`propertydb`) sang Elasticsearch (`SearchService`) sử dụng Debezium và Kafka.

## Mục tiêu
- Đảm bảo khi Host tạo/sửa Property, dữ liệu tìm kiếm được cập nhật ngay lập tức (Near real-time).
- Tránh việc gọi API trực tiếp giữa các service (Loose coupling).

## Acceptance Criteria
- [ ] Cấu hình Debezium Postgres Connector để lắng nghe bảng `Properties`.
- [ ] Code Kafka Consumer trong `SearchService` để nhận Event và index vào Elasticsearch.
- [ ] Xử lý Idempotency (chống trùng lặp event) dựa trên `PropertyId` và `Version/Timestamp`.

## Đầu vào
- Binlog từ Postgres (via Debezium).
- Kafka Topic: `airbnb.public.properties` (Lưu ý: Không subscribe topic `property_images` nữa).

## Đầu ra
- Dữ liệu text/geo-spatial trong Elasticsearch luôn khớp với Postgres.
- (MỚI) Frontend sẽ tự hydrate hình ảnh qua `/bulk` API thay vì bắt ES lưu hình ảnh.

## Ưu tiên
Medium (Đã hoàn thành và refactor Hybrid Hydration)

## Ước lượng
3 days
