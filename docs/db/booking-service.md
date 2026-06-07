# BookingService — Database Schema

> Database: `bookdb` (PostgreSQL)

## ER Diagram

```mermaid
erDiagram
    Bookings {
        uuid Id PK
        uuid PropertyId FK "NOT NULL — ref to PropertyService.properties.Id"
        uuid HostId FK "NOT NULL — ref to UserService.Users.Id"
        uuid GuestId FK "NOT NULL — ref to UserService.Users.Id"
        varchar CountryCode "NOT NULL, max 2"
        text BookingMode "NOT NULL, max 20"
        date CheckIn "NOT NULL"
        date CheckOut "NOT NULL"
        integer GuestCount "NOT NULL"
        integer NightCount "NOT NULL"
        numeric BasePricePerNight "NOT NULL"
        numeric CleaningFee "NOT NULL"
        numeric ServiceFee "NOT NULL"
        numeric TaxAmount "NOT NULL"
        numeric TotalPrice "NOT NULL"
        varchar CurrencyCode "NOT NULL, max 3"
        text Status "NOT NULL — enum as string"
        uuid CancelledBy "nullable — ref to UserService.Users.Id"
        timestamptz CreatedAt "NOT NULL"
        bigint Version "NOT NULL — optimistic concurrency"
    }

    ProcessedEvents {
        uuid EventId PK "Idempotency key"
    }

    Bookings ||--o| ProcessedEvents : "tracked by"
```

## Indexes

| Table | Index | Type | Notes |
|-------|-------|------|-------|
| Bookings | `idx_bookings_property_dates` | Filtered Unique | On (PropertyId, CheckIn, CheckOut) WHERE Status != 'Cancelled' — prevents double booking |
| Bookings | `idx_bookings_guest_id` | B-Tree | Optimize GetGuestBookings queries |
| Bookings | `idx_bookings_host_id` | B-Tree | Optimize GetHostBookings queries |

## Cross-Service References (Logical)

| Table | Column | References | Service |
|-------|--------|-----------|---------|
| Bookings | PropertyId | properties.Id | PropertyService |
| Bookings | HostId | Users.Id | UserService |
| Bookings | GuestId | Users.Id | UserService |
| Bookings | CancelledBy | Users.Id | UserService |

## Saga Database

BookingService also uses a separate **Saga database** (`BookingSagaDbContext`) managed by MassTransit for the booking state machine:

```mermaid
erDiagram
    BookingState {
        uuid CorrelationId PK
        varchar CurrentState "max 64"
        uuid BookingId "indexed"
        uuid PropertyId
        uuid HostId
        uuid GuestId
        varchar CurrencyCode "max 3"
        varchar BookingMode "max 20"
        date CheckIn
        date CheckOut
        numeric TotalPrice
        text PaymentId
    }
```

- The Saga DB runs as a separate PostgreSQL database and manages the booking workflow state machine (Created → PaymentPending → Confirmed → Cancelled, etc.).
- MassTransit Outbox tables (`InboxState`, `OutboxMessage`, `OutboxState`) are shared with the main booking DB.

## Notes

- `Status` stored as string enum: Pending, Confirmed, Cancelled, Completed, etc.
- `ProcessedEvents` table ensures idempotency for integration events.
- No internal FK constraints — all references are cross-service (logical FKs only, per database-per-service pattern).
- Optimistic concurrency via `Version` field inherited from `AggregateRoot`.
