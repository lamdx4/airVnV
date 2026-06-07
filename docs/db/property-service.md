# PropertyService — Database Schema

> Database: `propdb` (PostgreSQL)

## ER Diagram

```mermaid
erDiagram
    properties {
        uuid Id PK
        uuid HostId FK "NOT NULL — ref to UserService.Users.Id"
        varchar Title "NOT NULL"
        varchar Description "NOT NULL"
        varchar CountryCode "NOT NULL"
        varchar Admin1Code "nullable"
        varchar Admin2Code "nullable"
        varchar DisplayAddress "NOT NULL"
        jsonb AddressRaw "NOT NULL, default {}"
        double Latitude "NOT NULL"
        double Longitude "NOT NULL"
        jsonb HouseRules "NOT NULL, default {}"
        varchar Slug "NOT NULL"
        integer Status "NOT NULL, default 0"
        varchar SuspensionReason "nullable"
        text RejectionReason "nullable"
        text BookingMode "NOT NULL"
        integer Type "NOT NULL, default 0 — PropertyType enum"
        numeric AverageRating "NOT NULL, default 0"
        integer ReviewCount "NOT NULL, default 0"
        timestamptz CreatedAt "NOT NULL"
        timestamptz UpdatedAt "nullable"
        bigint Version "NOT NULL — optimistic concurrency"
        integer capacity_bathroom_count "NOT NULL, default 0"
        integer capacity_bed_count "NOT NULL, default 0"
        integer capacity_bedroom_count "NOT NULL, default 0"
        integer capacity_guest_count "NOT NULL, default 0"
        numeric pricing_base_price "NOT NULL, default 0"
        numeric pricing_cleaning_fee "NOT NULL, default 0"
        varchar pricing_currency_code "NOT NULL"
        numeric pricing_service_fee "NOT NULL, default 0"
        numeric pricing_weekend_premium_percent "NOT NULL, default 0"
    }

    amenities {
        uuid Id PK
        varchar Name "NOT NULL"
        varchar Category "NOT NULL"
        varchar IconCode "nullable"
    }

    property_amenities {
        uuid PropertyId PK|FK "NOT NULL"
        uuid AmenityId PK|FK "NOT NULL"
        varchar AdditionalInfo "nullable"
    }

    property_images {
        uuid Id PK
        uuid PropertyId FK "NOT NULL"
        uuid UploadedBy FK "NOT NULL — ref to UserService.Users.Id"
        varchar Url "NOT NULL"
        text PublicId "NOT NULL — Cloudinary public ID"
        varchar Type "NOT NULL"
        integer DisplayOrder "NOT NULL"
    }

    property_availabilities {
        uuid Id PK
        uuid PropertyId FK "NOT NULL"
        date StartDate "NOT NULL"
        date EndDate "NOT NULL"
        integer Type "NOT NULL — AvailabilityType enum"
        varchar Note "nullable"
    }

    reviews {
        uuid Id PK
        uuid PropertyId FK "NOT NULL"
        uuid BookingId FK "NOT NULL — ref to BookingService.Bookings.Id"
        uuid GuestId FK "NOT NULL — ref to UserService.Users.Id"
        integer Rating "NOT NULL"
        varchar Comment "NOT NULL"
        timestamptz CreatedAt "NOT NULL"
    }

    properties ||--o{ property_amenities : "has many (cascade)"
    amenities ||--o{ property_amenities : "has many"
    properties ||--o{ property_images : "has many (cascade)"
    properties ||--o{ property_availabilities : "has many (cascade)"
    properties ||--o{ reviews : "has many (cascade)"
```

## Indexes

| Table | Index | Type | Notes |
|-------|-------|------|-------|
| properties | (implicit) | PK | on Id |
| property_amenities | (implicit) | PK | Composite PK (PropertyId, AmenityId) |
| property_images | (implicit) | PK | on Id |
| property_availabilities | (implicit) | PK | on Id |
| reviews | (implicit) | PK | on Id |

## Relationships

| From | To | Type | FK Column | On Delete |
|------|----|------|-----------|-----------|
| property_amenities | properties | Many-to-One | PropertyId | CASCADE |
| property_amenities | amenities | Many-to-One | AmenityId | — |
| property_images | properties | Many-to-One | PropertyId | CASCADE |
| property_availabilities | properties | Many-to-One | PropertyId | CASCADE |
| reviews | properties | Many-to-One | PropertyId | CASCADE |

## Cross-Service References (Logical)

| Table | Column | References | Service |
|-------|--------|-----------|---------|
| properties | HostId | Users.Id | UserService |
| property_images | UploadedBy | Users.Id | UserService |
| reviews | BookingId | Bookings.Id | BookingService |
| reviews | GuestId | Users.Id | UserService |

## Notes

- `property_amenities` is a **join table** (many-to-many) between `properties` and `amenities`.
- `AddressRaw` is JSONB storing structured address fields (varies by country).
- `HouseRules` is JSONB storing rule configuration (check-in time, pets, smoking, etc.).
- Pricing/capacity fields use **prefix notation** (`pricing_*`, `capacity_*`) — flattened from original owned entities.
- `Status` is integer enum: 0=Draft, 1=Active, 2=Suspended, 3=Inactive, 4=Rejected (check source for full list).
- `Type` is integer enum for `PropertyType`: 0=NotSet, etc.
- `BookingMode` text field for booking mode.
- MassTransit Outbox tables exist for integration events.
