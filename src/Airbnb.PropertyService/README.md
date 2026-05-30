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
| `properties` | Core metadata and storage for Properties. |
| `property_amenities` | Core metadata and storage for Property Amenities. |
| `property_availabilities` | Core metadata and storage for Property Availabilities. |
| `property_images` | Core metadata and storage for Property Images. |
| `reviews` | Core metadata and storage for Reviews. |

### Entity Relationship Diagram (ERD)
```mermaid
erDiagram
    INBOXSTATE {
        bigint Id PK
        uuid MessageId 
        uuid ConsumerId 
        uuid LockId 
        bytea RowVersion 
        timestamptz Received 
        integer ReceiveCount 
        timestamptz ExpirationTime 
        timestamptz Consumed 
        timestamptz Delivered 
        bigint LastSequenceNumber 
    }
    OUTBOXMESSAGE {
        bigint SequenceNumber PK
        timestamptz EnqueueTime 
        timestamptz SentTime 
        text Headers 
        text Properties 
        uuid InboxMessageId 
        uuid InboxConsumerId 
        uuid OutboxId 
        uuid MessageId 
        varchar(256) ContentType 
        text MessageType 
        text Body 
        uuid ConversationId 
        uuid CorrelationId 
        uuid InitiatorId 
        uuid RequestId 
        varchar(256) SourceAddress 
        varchar(256) DestinationAddress 
        varchar(256) ResponseAddress 
        varchar(256) FaultAddress 
        timestamptz ExpirationTime 
    }
    OUTBOXSTATE {
        uuid OutboxId PK
        uuid LockId 
        bytea RowVersion 
        timestamptz Created 
        timestamptz Delivered 
        bigint LastSequenceNumber 
    }
    AMENITIES {
        uuid Id PK
        varchar(100) Name 
        varchar(50) Category 
        varchar(50) IconCode 
    }
    PROPERTIES {
        uuid Id PK
        uuid HostId 
        varchar(255) Title 
        varchar(5000) Description 
        varchar(2) CountryCode 
        double_precision Latitude 
        double_precision Longitude 
        timestamptz CreatedAt 
        jsonb AddressRaw 
        varchar(100) Admin1Code 
        varchar(100) Admin2Code 
        varchar(500) DisplayAddress 
        jsonb HouseRules 
        varchar(300) Slug 
        integer Status 
        varchar(500) SuspensionReason 
        timestamptz UpdatedAt 
        bigint Version 
        integer capacity_bathroom_count 
        integer capacity_bed_count 
        integer capacity_bedroom_count 
        integer capacity_guest_count 
        numeric pricing_base_price 
        numeric pricing_cleaning_fee 
        varchar(3) pricing_currency_code 
        numeric pricing_service_fee 
        numeric pricing_weekend_premium_percent 
        numeric AverageRating 
        text BookingMode 
        integer ReviewCount 
        integer Type 
    }
    PROPERTY_AMENITIES {
        uuid PropertyId PK,FK
        uuid AmenityId PK
        varchar(200) AdditionalInfo 
    }
    PROPERTY_AVAILABILITIES {
        uuid Id PK
        uuid PropertyId FK
        date StartDate 
        date EndDate 
        integer Type 
        varchar(255) Note 
    }
    PROPERTY_IMAGES {
        uuid Id PK
        uuid PropertyId FK
        uuid UploadedBy 
        varchar(2048) Url 
        text PublicId 
        varchar(20) Type 
        integer DisplayOrder 
    }
    REVIEWS {
        uuid Id PK
        uuid PropertyId FK
        uuid BookingId 
        uuid GuestId 
        integer Rating 
        varchar(1000) Comment 
        timestamptz CreatedAt 
    }

    PROPERTIES ||--o{ PROPERTY_AMENITIES : "has"
    PROPERTIES ||--o{ PROPERTY_IMAGES : "has"
    PROPERTIES ||--o{ PROPERTY_AVAILABILITIES : "has"
    PROPERTIES ||--o{ REVIEWS : "has"
```

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
