# Property Service

**Property Service** is the core bounded context responsible for managing the lifecycle, pricing, and configuration of all rental properties (listings) on the AirVnV platform. It acts as the primary "Write Model" for property data.

## 🧠 Domain Concepts

* **Property Lifecycle (State Machine):** Properties move through strict states: `Draft` (incomplete setup) -> `PendingReview` (awaiting admin approval) -> `Published` (visible to guests) -> `Suspended` / `Archived`.
* **Dynamic Pricing Engine:** A property doesn't just have a single price. It contains a `BasePrice`, `CleaningFee`, `ServiceFee`, and a dynamic `WeekendPremiumPercentage`.
* **Availability Management:** Hosts can block out specific date ranges manually, or they are automatically blocked when a Booking is confirmed.
* **Amenities & Rules:** Structured metadata describing what the property offers and rules guests must follow (e.g., `AllowPets`, `CheckInTime`).

## 🗄️ Database Schema (PostgreSQL)

The primary tables in this microservice:

| Table Name | Description |
|------------|-------------|
| `InboxState` | Core metadata and storage for InboxState. |
| `OutboxMessage` | Core metadata and storage for OutboxMessage. |
| `OutboxState` | Core metadata and storage for OutboxState. |
| `amenities` | Core metadata and storage for Amenities. |
| `o` | Core metadata and storage for O. |
| `properties` | Core metadata and storage for Properties. |
| `property_amenities` | Core metadata and storage for Property Amenities. |
| `property_availabilities` | Core metadata and storage for Property Availabilities. |
| `property_images` | Core metadata and storage for Property Images. |
| `reviews` | Core metadata and storage for Reviews. |

### Entity Relationship Diagram (ERD)
```mermaid
erDiagram

    InboxState {
        Id bigint PK "not null"
        ReceiveCount integer "not null"
        Received timestamp_with_time_zone "not null"
        ConsumerId uuid "not null"
        LockId uuid "not null"
        MessageId uuid "not null"
        LastSequenceNumber bigint "null"
        RowVersion bytea "null"
        Consumed timestamp_with_time_zone "null"
        Delivered timestamp_with_time_zone "null"
        ExpirationTime timestamp_with_time_zone "null"
    }

    OutboxMessage {
        SequenceNumber bigint PK "not null"
        ContentType character_varying "not null"
        Body text "not null"
        MessageType text "not null"
        SentTime timestamp_with_time_zone "not null"
        MessageId uuid "not null"
        DestinationAddress character_varying "null"
        FaultAddress character_varying "null"
        ResponseAddress character_varying "null"
        SourceAddress character_varying "null"
        Headers text "null"
        Properties text "null"
        EnqueueTime timestamp_with_time_zone "null"
        ExpirationTime timestamp_with_time_zone "null"
        ConversationId uuid "null"
        CorrelationId uuid "null"
        InboxConsumerId uuid "null"
        InboxMessageId uuid "null"
        InitiatorId uuid "null"
        OutboxId uuid "null"
        RequestId uuid "null"
    }

    OutboxState {
        OutboxId uuid PK "not null"
        Created timestamp_with_time_zone "not null"
        LockId uuid "not null"
        LastSequenceNumber bigint "null"
        RowVersion bytea "null"
        Delivered timestamp_with_time_zone "null"
    }

    amenities {
        Id uuid PK "not null"
        Category character_varying "not null"
        Name character_varying "not null"
        IconCode character_varying "null"
    }

    properties {
        Id uuid PK "not null"
        Version bigint "not null"
        CountryCode character_varying "not null"
        Description character_varying "not null"
        DisplayAddress character_varying "not null"
        Slug character_varying "not null"
        Title character_varying "not null"
        pricing_currency_code character_varying "not null"
        Latitude double_precision "not null"
        Longitude double_precision "not null"
        ReviewCount integer "not null"
        Status integer "not null"
        Type integer "not null"
        capacity_bathroom_count integer "not null"
        capacity_bed_count integer "not null"
        capacity_bedroom_count integer "not null"
        capacity_guest_count integer "not null"
        AddressRaw jsonb "not null"
        HouseRules jsonb "not null"
        AverageRating numeric "not null"
        pricing_base_price numeric "not null"
        pricing_cleaning_fee numeric "not null"
        pricing_service_fee numeric "not null"
        pricing_weekend_premium_percent numeric "not null"
        BookingMode text "not null"
        CreatedAt timestamp_with_time_zone "not null"
        HostId uuid "not null"
        Admin1Code character_varying "null"
        Admin2Code character_varying "null"
        SuspensionReason character_varying "null"
        UpdatedAt timestamp_with_time_zone "null"
    }

    property_amenities {
        AmenityId uuid PK "not null"
        PropertyId uuid PK "not null"
        PropertyId uuid FK "not null"
        AdditionalInfo character_varying "null"
    }

    property_availabilities {
        Id uuid PK "not null"
        PropertyId uuid FK "not null"
        EndDate date "not null"
        StartDate date "not null"
        Type integer "not null"
        Note character_varying "null"
    }

    property_images {
        Id uuid PK "not null"
        PropertyId uuid FK "not null"
        Type character_varying "not null"
        Url character_varying "not null"
        DisplayOrder integer "not null"
        PublicId text "not null"
        UploadedBy uuid "not null"
    }

    reviews {
        Id uuid PK "not null"
        PropertyId uuid FK "not null"
        Comment character_varying "not null"
        Rating integer "not null"
        CreatedAt timestamp_with_time_zone "not null"
        BookingId uuid "not null"
        GuestId uuid "not null"
    }

    properties ||--o{ property_amenities : "property_amenities(PropertyId) -> properties(Id)"
    properties ||--o{ property_availabilities : "property_availabilities(PropertyId) -> properties(Id)"
    properties ||--o{ property_images : "property_images(PropertyId) -> properties(Id)"
    properties ||--o{ reviews : "reviews(PropertyId) -> properties(Id)"
```

## Indexes

### `InboxState`

- `AK_InboxState_MessageId_ConsumerId`
- `IX_InboxState_Delivered`
- `PK_InboxState`

### `OutboxMessage`

- `IX_OutboxMessage_EnqueueTime`
- `IX_OutboxMessage_ExpirationTime`
- `IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber`
- `IX_OutboxMessage_OutboxId_SequenceNumber`
- `PK_OutboxMessage`

### `OutboxState`

- `IX_OutboxState_Created`
- `PK_OutboxState`

### `amenities`

- `PK_amenities`

### `properties`

- `IX_properties_CountryCode_Admin1Code_Admin2Code`
- `IX_properties_Slug`
- `PK_properties`

### `property_amenities`

- `PK_property_amenities`

### `property_availabilities`

- `IX_property_availabilities_PropertyId`
- `PK_property_availabilities`

### `property_images`

- `IX_property_images_PropertyId_Type`
- `PK_property_images`

### `reviews`

- `IX_reviews_BookingId`
- `IX_reviews_PropertyId`
- `PK_reviews`

## Indexes

### `InboxState`

- `AK_InboxState_MessageId_ConsumerId`
- `IX_InboxState_Delivered`
- `PK_InboxState`

### `OutboxMessage`

- `IX_OutboxMessage_EnqueueTime`
- `IX_OutboxMessage_ExpirationTime`
- `IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber`
- `IX_OutboxMessage_OutboxId_SequenceNumber`
- `PK_OutboxMessage`

### `OutboxState`

- `IX_OutboxState_Created`
- `PK_OutboxState`

### `amenities`

- `PK_amenities`

### `properties`

- `IX_properties_CountryCode_Admin1Code_Admin2Code`
- `IX_properties_Slug`
- `PK_properties`

### `property_amenities`

- `PK_property_amenities`

### `property_availabilities`

- `IX_property_availabilities_PropertyId`
- `PK_property_availabilities`

### `property_images`

- `IX_property_images_PropertyId_Type`
- `PK_property_images`

### `reviews`

- `IX_reviews_BookingId`
- `IX_reviews_PropertyId`
- `PK_reviews`

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **PATCH** | `/api/properties/{propertyId}/status` | Update property status (Publish/Unpublish/Archive) |
| **PUT** | `/api/properties/{propertyId}/reviews/{reviewId}` |  |
| **DELETE** | `/api/properties/{propertyId}/reviews/{reviewId}` |  |
| **PATCH** | `/api/properties/{propertyId}` | Update property information (Host only, partial update) |
| **GET** | `/api/properties/{propertyId}` |  |
| **DELETE** | `/api/properties/{propertyId}` | Delete property (Host only, Draft or Archived only) |
| **PUT** | `/api/properties/{propertyId}/location` | Update property location coordinates and address |
| **PATCH** | `/api/properties/{propertyId}/amenities/{amenityId}` | Update amenity additional information/notes |
| **DELETE** | `/api/properties/{propertyId}/amenities/{amenityId}` | Remove amenity from property (Host only) |
| **POST** | `/api/properties/{propertyId}/amenities/{amenityId}` | Add amenity to property (Host only) |
| **POST** | `/api/properties/{propertyId}/suspend` | Admin suspends property (Published → Suspended) |
| **POST** | `/api/properties/{propertyId}/submit` | Host submits property for review (Draft → PendingReview) |
| **POST** | `/api/properties/{propertyId}/reinstate` | Admin reinstates property (Suspended → Published) |
| **POST** | `/api/properties/{propertyId}/images/reorder` | Reorder property images |
| **DELETE** | `/api/properties/{propertyId}/images/{imageId}` | Remove property image (Host only) |
| **POST** | `/api/properties/{propertyId}/images/bulk` | Bulk add images to property (Host only) |
| **POST** | `/api/properties/{propertyId}/images` | Add image to property (Host only, Server-side upload) |
| **DELETE** | `/api/properties/{propertyId}/availability/{availabilityId}` | Remove property availability/blocked dates |
| **POST** | `/api/properties/{propertyId}/availability/block` | Block property dates (Calendar Management) |
| **GET** | `/api/properties/{propertyId}/reviews` |  |
| **POST** | `/api/properties/{propertyId}/reviews` |  |
| **GET** | `/api/properties/{propertyId}/basic-info` | Get basic information of a property (for internal microservice communication) |
| **GET** | `/api/properties/my` | Get all properties of the current host with pagination |
| **GET** | `/api/amenities` | Get all available amenities in the system |
| **POST** | `/api/properties` | Create a new property listing with images (Atomic) |
| **POST** | `/api/properties/{propertyId}/archive` | Host archives property (Published\|Suspended → Archived) |
| **POST** | `/api/properties/{propertyId}/approve` | Admin approve property (PendingReview → Published) |
