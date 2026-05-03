# Sơ đồ Cơ sở Dữ liệu Microservices

Dưới đây là sơ đồ thực thể liên kết (ERD) cho toàn bộ hệ sinh thái dịch vụ, thể hiện sự phân chia dữ liệu theo từng Database riêng biệt (Database-per-service pattern) cũng như các bảng hỗ trợ kiến trúc Event-driven (Outbox, Idempotency).

> [!TIP]
> Các liên kết (relationships) trong sơ đồ là các liên kết mang tính logic (Logical Relationships). Trong môi trường Database-per-service thực tế, các khóa ngoại (Foreign Keys) sẽ không có constraints vật lý ở cấp độ database để đảm bảo tính độc lập.

```mermaid
erDiagram
    %% ==========================================
    %% DATABASE: userdb (Airbnb.UserService)
    %% ==========================================
    User {
        Guid Id PK
        string FullName
        string Email
        string HashedPassword
        DateTime CreatedAt
    }

    %% ==========================================
    %% DATABASE: propertydb (Airbnb.PropertyService)
    %% ==========================================
    Property {
        Guid Id PK
        Guid HostId "FK (Logical to User)"
        string Name
        string Description
        decimal PricePerNight
        string Address_CountryCode
        string Address_City
        string Address_StateProvince "Nullable"
        string Address_Ward "Nullable"
        string Address_StreetLine1
        string Address_StreetLine2 "Nullable"
        string Address_PostalCode "Nullable"
        double Address_Latitude
        double Address_Longitude
        DateTime CreatedAt
    }

    %% ==========================================
    %% DATABASE: bookdb (Airbnb.BookingService)
    %% ==========================================
    Booking {
        Guid Id PK
        Guid PropertyId "FK (Logical)"
        Guid UserId "FK (Logical)"
        DateTime CheckIn
        DateTime CheckOut
        decimal TotalPrice
        string Status "Pending | Confirmed | Cancelled"
        DateTime CreatedAt
    }
    
    ProcessedEvent {
        Guid EventId PK "Idempotency Key"
        DateTime ProcessedAt
        string EventType
    }

    %% ==========================================
    %% DATABASE: paydb (Airbnb.PaymentService)
    %% ==========================================
    Payment {
        Guid Id PK
        Guid BookingId "FK (Logical)"
        decimal Amount
        string Status "Pending | Success | Failed"
        string TransactionId
        DateTime CreatedAt
    }
    
    OutboxEvent {
        Guid Id PK "CDC Event ID"
        string EventType
        string Payload
        string TraceId "OpenTelemetry Context"
        DateTime CreatedAt
        bool Processed
    }

    %% ==========================================
    %% SEARCH INDEX (Airbnb.SearchService - Elasticsearch)
    %% ==========================================
    PropertyDoc {
        Guid Id PK
        Guid HostId "Indexed for filtering"
        string Name
        string Description
        decimal PricePerNight
        object AddressVO "Contains Geopoint and Locality"
        DateTime CreatedAt
    }

    %% ==========================================
    %% LOGICAL RELATIONSHIPS
    %% ==========================================
    User ||--o{ Property : "owns (as Host)"
    User ||--o{ Booking : "makes (as Guest)"
    Property ||--o{ Booking : "has"
    Booking ||--o| Payment : "requires"
    
    %% CDC & Event-Driven Flows
    Property ||--o| PropertyDoc : "synced via Debezium CDC"
    Payment ||--o| OutboxEvent : "generates (Tx Outbox)"
    OutboxEvent ||--o| ProcessedEvent : "consumed idempotently via Kafka"
```

## Chú giải kiến trúc

* **Độc lập Dữ liệu:** Mỗi Microservice tự quản lý schema của mình. Dữ liệu cross-service được tham chiếu thông qua ID (GUID).
* **Outbox Pattern:** Tại `paydb`, bảng `OutboxEvent` được lưu cùng transaction với `Payment` để đảm bảo Debezium chắc chắn sẽ pick up event và đẩy lên Kafka mà không lo lỗi Dual-write.
* **Idempotency:** Tại `bookdb`, bảng `ProcessedEvent` giữ vai trò rào chắn bảo vệ logic xử lý Booking (Consumer), giúp hệ thống an toàn chặn đứng mọi sự kiện bị lặp lại do retries hoặc lỗi mạng.
* **Search Sync:** `PropertyDoc` trong Elasticsearch là một "bản chiếu" (projection/read-model) từ bảng `Property` (Postgres) thông qua luồng CDC, đảm bảo Search Engine luôn đồng bộ với Source of Truth.
