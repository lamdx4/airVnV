# Task: Tìm kiếm Nâng cao và Địa lý (Geo-search & Filters)

## Mô tả
Nâng cấp `SearchService` để hỗ trợ tìm kiếm theo tọa độ địa lý (Geopoint) và các bộ lọc nâng cao từ Elasticsearch.

## Mục tiêu
- Tích hợp Elasticsearch Geopoint để tìm kiếm Property trong bán kính X km.
- Triển khai các bộ lọc: Khoảng giá (Price range), Loại phòng, Tiện ích.
- Trả về danh sách kết quả có kèm thông tin khoảng cách.

## Acceptance Criteria
- [ ] Cấu hình Mapping Elasticsearch cho `PropertyDoc` với field `location` kiểu `geo_point`.
- [ ] API `GET /api/search` hỗ trợ tham số `lat`, `lon`, `radius`, `minPrice`, `maxPrice`.
- [ ] Thực hiện truy vấn `geo_distance` kết hợp `bool query` (filter) trong Elasticsearch.

## Đầu vào
- `lat`, `lon` (double)
- `radius` (km)
- `filters` (JSON object)

## Đầu ra
- Danh sách `PropertyDoc` thỏa mãn điều kiện.

## Ưu tiên
High

## Ước lượng
4 days
