# ChatService — Database Schema

> Database: `chatdb` (PostgreSQL)

## ER Diagram

```mermaid
erDiagram
    Conversations {
        uuid Id PK
        uuid PropertyId FK "NOT NULL — ref to PropertyService.properties.Id"
        text PropertyTitle "NOT NULL"
        uuid ReservationId FK "nullable — ref to BookingService.Bookings.Id"
        timestamptz LastMessageAt "NOT NULL"
        timestamptz CreatedAt "NOT NULL"
    }

    ConversationParticipants {
        uuid ConversationId PK|FK "NOT NULL — composite PK"
        uuid UserId PK "NOT NULL — composite PK, ref to UserService.Users.Id"
        text Role "NOT NULL — Host|Guest"
        text DisplayName "NOT NULL"
        text AvatarUrl "nullable"
        uuid LastReadMessageId "nullable"
        boolean IsArchived "NOT NULL"
    }

    Messages {
        uuid Id PK
        uuid ConversationId FK "NOT NULL"
        uuid SenderId FK "nullable — ref to UserService.Users.Id"
        text MessageType "NOT NULL — Text|Image|System"
        text Content "NOT NULL"
        timestamptz CreatedAt "NOT NULL"
    }

    Conversations ||--o{ ConversationParticipants : "has many (cascade)"
    Conversations ||--o{ Messages : "has many"
```

## Indexes

| Table | Index | Type | Notes |
|-------|-------|------|-------|
| Conversations | `uq_conversation_property_no_res` | Partial Unique | On PropertyId WHERE ReservationId IS NULL — one general chat per property |
| Conversations | `uq_conversation_property_res` | Partial Unique | On (PropertyId, ReservationId) WHERE ReservationId IS NOT NULL — one chat per reservation |
| ConversationParticipants | `idx_participants_user_id` | B-Tree | Optimize inbox loading (user's conversations) |
| Messages | `idx_messages_conversation_created` | B-Tree | On (ConversationId, CreatedAt DESC) — cursor-based pagination |

## Relationships (Internal FKs)

| From | To | Type | FK Column | On Delete |
|------|----|------|-----------|-----------|
| ConversationParticipants | Conversations | Many-to-One | ConversationId | CASCADE |
| Messages | Conversations | Many-to-One | ConversationId | — |

## Cross-Service References (Logical)

| Table | Column | References | Service |
|-------|--------|-----------|---------|
| Conversations | PropertyId | properties.Id | PropertyService |
| Conversations | ReservationId | Bookings.Id | BookingService |
| ConversationParticipants | UserId | Users.Id | UserService |
| Messages | SenderId | Users.Id | UserService |

## Notes

- `ConversationParticipants` uses a **composite primary key** (ConversationId, UserId).
- Two partial unique indexes on `Conversations` enforce business rules:
  - Without reservation: only 1 conversation per property (host-guest general chat).
  - With reservation: 1 conversation per property+reservation (booking-specific chat).
- `Messages.CreatedAt` indexed DESC for efficient cursor-based pagination (most recent first).
- `MessageType` stored as string: Text, Image, System.
- `Role` stored as string: Host, Guest.
- `LastReadMessageId` tracks read progress per participant for unread badge counts.
- MassTransit Outbox tables exist for integration events.
