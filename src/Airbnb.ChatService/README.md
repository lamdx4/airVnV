# Chat Service

**Chat Service** manages all real-time communication between Hosts and Guests. Unlike standard messaging apps, conversations here are strictly tied to a specific business context (a Property or a Booking).

## 🧠 Domain Concepts
* **Context-Bound Threads:** A conversation cannot exist in a vacuum. It must be linked to a `ContextId` (e.g., BookingId).
* **System Messages:** The service listens to RabbitMQ for events (like `BookingConfirmed` or `BookingCancelled`) and automatically injects immutable System Messages into the chat thread.
* **Real-Time Delivery:** Utilizes SignalR for WebSockets. A Redis Backplane is used to scale SignalR across multiple server instances.
* **Data Replication:** Since this service needs user names and avatars to display chat heads quickly, it subscribes to `UserProfileUpdatedEvent` via RabbitMQ and maintains a read-optimized copy of users in its own DB.

## 🗄️ Database Schema (PostgreSQL)

The primary tables in this microservice:

| Table Name | Description |
|------------|-------------|
| `ConversationParticipants` | Core metadata and storage for ConversationParticipants. |
| `Conversations` | Core metadata and storage for Conversations. |
| `InboxState` | Core metadata and storage for InboxState. |
| `Messages` | Core metadata and storage for Messages. |
| `OutboxMessage` | Core metadata and storage for OutboxMessage. |
| `OutboxState` | Core metadata and storage for OutboxState. |

### Entity Relationship Diagram (ERD)
```mermaid
erDiagram
    CONVERSATIONPARTICIPANTS {
        uuid ConversationId PK,FK
        uuid UserId PK
        text Role 
        text DisplayName 
        text AvatarUrl 
        uuid LastReadMessageId 
        boolean IsArchived 
    }
    CONVERSATIONS {
        uuid Id PK
        uuid PropertyId 
        text PropertyTitle 
        uuid ReservationId 
        timestamptz LastMessageAt 
        timestamptz CreatedAt 
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
    MESSAGES {
        uuid Id PK
        uuid ConversationId FK
        uuid SenderId 
        text MessageType 
        text Content 
        timestamptz CreatedAt 
    }
    OUTBOXMESSAGE {
        bigint SequenceNumber PK
        timestamptz EnqueueTime 
        timestamptz SentTime 
        text Headers 
        text Properties 
        uuid InboxMessageId FK
        uuid InboxConsumerId FK
        uuid OutboxId FK
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

    CONVERSATIONS ||--o{ CONVERSATIONPARTICIPANTS : "has"
    CONVERSATIONS ||--o{ MESSAGES : "has"
    INBOXSTATE ||--o{ OUTBOXMESSAGE : "has"
    OUTBOXSTATE ||--o{ OUTBOXMESSAGE : "has"
```

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **GET** | `/api/chat/conversations` | Get all active conversations for the current user. |
| **GET** | `/api/chat/conversations/{id}/messages` | Get paginated messages for a thread. |
| **POST**| `/api/chat/conversations/{id}/messages` | Send a new text/image message. |
| **POST**| `/api/chat/conversations/{id}/read` | Update the read-receipt watermark. |

## 📡 SignalR Hubs
* `/hubs/chat` - Real-time WebSocket connection for receiving incoming messages instantly.
