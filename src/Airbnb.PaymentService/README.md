# Payment Service

**Payment Service** handles all financial transactions, integrating with third-party payment gateways (like Stripe, VNPay, or Momo). It isolates all webhook processing and PCI-compliance concerns from the core business services.

## 🧠 Domain Concepts
* **Payment Intents:** Creating an intent locks in a transaction amount and generates a payment URL/token for the client.
* **Webhook Processing:** Asynchronously listens to callbacks from payment providers to verify signatures and update payment statuses.
* **Event-Driven Fulfillment:** Upon successful payment, it publishes a `PaymentCompletedEvent` to RabbitMQ. The BookingService listens to this to mark reservations as `Confirmed`.

## 🗄️ Database Schema (PostgreSQL)

The primary tables in this microservice:

| Table Name | Description |
|------------|-------------|
| `InboxState` | Core metadata and storage for InboxState. |
| `OutboxMessage` | Core metadata and storage for OutboxMessage. |
| `OutboxState` | Core metadata and storage for OutboxState. |
| `Payments` | Core metadata and storage for Payments. |

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

    Payments {
        Id uuid PK "not null"
        Version bigint "not null"
        Currency character_varying "not null"
        Amount numeric "not null"
        Status text "not null"
        CreatedAt timestamp_with_time_zone "not null"
        BookingId uuid "not null"
        PaymentUrl character_varying "null"
        TransactionId character_varying "null"
        ExpiresAt timestamp_with_time_zone "null"
    }
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

### `Payments`

- `PK_Payments`
- `ix_payments_booking_pending`

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **POST** | `/api/payments/create-intent` | Initialize a payment session for a specific booking. |
| **GET**  | `/api/payments/{id}` | Check the status of a payment. |
| **POST** | `/api/payments/webhooks/vnpay` | Webhook endpoint for VNPay callbacks. |
| **POST** | `/api/payments/webhooks/stripe` | Webhook endpoint for Stripe callbacks. |
