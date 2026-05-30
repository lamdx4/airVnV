# User Service

**User Service** manages identities, authentication (JWT), and user profiles for both Guests and Hosts on the AirVnV platform. It acts as the central source of truth for user data and broadcasts `UserProfileUpdatedEvent`s for other services to replicate basic profile info (like Display Name and Avatar).

## 🧠 Domain Concepts
* **Authentication & Authorization:** Issues short-lived JWT Access Tokens and long-lived Refresh Tokens. Implements Google OAuth login.
* **Roles:** Users can be standard Guests or upgraded to Hosts.
* **Profile Management:** Handles avatar uploads via secure pre-signed URLs (S3/Cloudinary) and basic profile metadata.

## 🗄️ Database Schema (PostgreSQL)

The primary tables in this microservice:

| Table Name | Description |
|------------|-------------|
| `InboxState` | Core metadata and storage for InboxState. |
| `OutboxMessage` | Core metadata and storage for OutboxMessage. |
| `OutboxState` | Core metadata and storage for OutboxState. |
| `UserLogins` | Core metadata and storage for UserLogins. |
| `UserProfiles` | Core metadata and storage for UserProfiles. |
| `UserRefreshTokens` | Core metadata and storage for UserRefreshTokens. |
| `Users` | Core metadata and storage for Users. |

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
    USERLOGINS {
        uuid Id PK
        uuid UserId FK
        text Provider 
        text ProviderKey 
    }
    USERPROFILES {
        uuid UserId PK,FK
        varchar(255) FullName 
        text AvatarUrl 
        text PhoneNumber 
        text Bio 
    }
    USERREFRESHTOKENS {
        uuid Id PK
        uuid UserId FK
        text Token 
        timestamptz ExpiresAt 
        timestamptz CreatedAt 
        timestamptz LoginAt 
        timestamptz RevokedAt 
        text IpAddress 
        text UserAgent 
    }
    USERS {
        uuid Id PK
        varchar(255) Email 
        text HashedPassword 
        text Role 
        timestamptz CreatedAt 
        bigint Version 
    }

    USERS ||--o{ USERLOGINS : "has"
    USERS ||--o{ USERPROFILES : "has"
    USERS ||--o{ USERREFRESHTOKENS : "has"
```

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **POST** | `/api/users/register` | Register a new user account. |
| **POST** | `/api/users/verify-email` | Verify email with OTP/Link. |
| **POST** | `/api/users/login` | Authenticate and get JWT. |
| **POST** | `/api/users/google-auth` | Authenticate using Google OAuth token. |
| **POST** | `/api/users/refresh-token` | Exchange refresh token for new JWT. |
| **GET**  | `/api/users/me` | Get current user's profile. |
| **PUT**  | `/api/users/me` | Update profile information. |
| **GET**  | `/api/users/{id}/public-profile` | Get public profile of a host/guest. |
| **GET**  | `/api/account/sessions` | List active sessions. |
| **POST** | `/api/account/sessions/revoke` | Revoke a specific refresh token session. |
| **POST** | `/api/account/change-password` | Update account password. |
