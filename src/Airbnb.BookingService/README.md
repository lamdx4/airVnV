# Booking Service

**Booking Service** is the engine that handles reservations. It is responsible for enforcing availability, locking dates to prevent double-booking, and managing the state transition of a booking lifecycle.

## 🧠 Domain Concepts
* **Concurrency & Double-Booking Prevention:** Employs database row-level locking or optimistic concurrency to ensure two users cannot book the exact same dates simultaneously.
* **Booking Lifecycle:** `Pending` (awaiting payment) -> `Confirmed` (payment success) -> `Cancelled` / `Completed`.
* **Outbox Pattern:** When a booking is confirmed, it writes a `BookingConfirmedEvent` to a local Outbox table in the same transaction. A background worker then publishes this to RabbitMQ, ensuring 100% consistency.

## 🗄️ Database Schema (PostgreSQL)

The primary tables in this microservice:

| Table Name | Description |
|------------|-------------|
| `BookingState` | Core metadata and storage for BookingState. |
| `Bookings` | Core metadata and storage for Bookings. |
| `InboxState` | Core metadata and storage for InboxState. |
| `OutboxMessage` | Core metadata and storage for OutboxMessage. |
| `OutboxState` | Core metadata and storage for OutboxState. |
| `ProcessedEvents` | Core metadata and storage for ProcessedEvents. |

### Entity Relationship Diagram (ERD)
```mermaid
erDiagram
    BOOKINGSTATE {
        uuid CorrelationId PK
        varchar(64) CurrentState 
        uuid BookingId 
        uuid GuestId 
        uuid PropertyId 
        numeric TotalPrice 
        varchar(3) CurrencyCode 
        timestamptz CreatedAt 
        timestamptz UpdatedAt 
        uuid ExpirationTokenId 
    }
    BOOKINGS {
        uuid Id PK
        uuid PropertyId 
        uuid HostId 
        uuid GuestId 
        date CheckIn 
        date CheckOut 
        integer GuestCount 
        integer NightCount 
        numeric BasePricePerNight 
        numeric CleaningFee 
        numeric ServiceFee 
        numeric TotalPrice 
        varchar(3) CurrencyCode 
        text Status 
        uuid CancelledBy 
        timestamptz CreatedAt 
        numeric TaxAmount 
        varchar(2) CountryCode 
        bigint Version 
    }
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
    PROCESSEDEVENTS {
        uuid EventId PK
        timestamptz ProcessedAt 
        text EventType 
    }

```

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **POST** | `/api/bookings` | Create a new booking (Pending status). |
| **GET**  | `/api/bookings/{id}` | Get details of a specific booking. |
| **GET**  | `/api/bookings/my-trips` | Get all bookings made by the current user. |
| **GET**  | `/api/bookings/my-reservations` | Get all bookings made on the host's properties. |
| **POST** | `/api/bookings/{id}/cancel` | Cancel an active booking (subject to refund rules). |
