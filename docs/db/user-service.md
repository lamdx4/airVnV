# UserService — Database Schema

> Database: `userdb` (PostgreSQL)

## ER Diagram

```mermaid
erDiagram
    Users {
        uuid Id PK
        varchar Email UK "NOT NULL, max 255"
        text HashedPassword "nullable"
        text Role "NOT NULL — Guest|Host|Admin"
        text Status "NOT NULL — Active|Suspended|Banned"
        boolean IsVerified "NOT NULL"
        timestamptz CreatedAt "NOT NULL"
        timestamptz LastLoginAt "nullable"
        varchar SuspensionReason "nullable, max 500"
        varchar BanReason "nullable, max 500"
        bigint Version "NOT NULL — optimistic concurrency"
    }

    UserProfiles {
        uuid UserId PK|FK "NOT NULL"
        varchar FullName "NOT NULL, max 255"
        text AvatarUrl "nullable"
        text PhoneNumber "nullable"
        text Bio "nullable"
    }

    UserLogins {
        uuid Id PK "NOT NULL"
        uuid UserId FK "NOT NULL"
        text Provider "NOT NULL — Google|Facebook etc."
        text ProviderKey "NOT NULL — UK(Provider,ProviderKey)"
    }

    UserRefreshTokens {
        uuid Id PK "NOT NULL"
        uuid UserId FK "NOT NULL"
        text Token "NOT NULL"
        text UserAgent "nullable"
        text IpAddress "nullable"
        timestamptz ExpiresAt "NOT NULL"
        timestamptz CreatedAt "NOT NULL"
        timestamptz LoginAt "NOT NULL"
        timestamptz RevokedAt "nullable"
    }

    Users ||--|| UserProfiles : "has one (cascade)"
    Users ||--o{ UserLogins : "has many (cascade)"
    Users ||--o{ UserRefreshTokens : "has many (cascade)"
```

## Indexes

| Table | Index | Type | Notes |
|-------|-------|------|-------|
| Users | `IX_Users_Email` | UNIQUE | Email must be unique |
| UserLogins | `IX_UserLogins_Provider_ProviderKey` | UNIQUE | Composite unique on (Provider, ProviderKey) |

## Relationships

| From | To | Type | FK Column | On Delete |
|------|----|------|-----------|-----------|
| UserProfiles | Users | One-to-One | UserId | CASCADE |
| UserLogins | Users | Many-to-One | UserId | CASCADE |
| UserRefreshTokens | Users | Many-to-One | UserId | CASCADE |

## Notes

- All PKs are client-generated UUIDs (`ValueGeneratedNever`).
- Enums (`Role`, `Status`, `Provider`) stored as **string** in PostgreSQL.
- `Version` field used for optimistic concurrency control via `AggregateRoot` base class.
- MassTransit Outbox tables (`InboxState`, `OutboxMessage`, `OutboxState`) exist but are infrastructure-only.
