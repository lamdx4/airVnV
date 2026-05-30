# User Service

**User Service** manages identities, authentication (JWT), and user profiles for both Guests and Hosts on the AirVnV platform. It acts as the central source of truth for user data and broadcasts `UserProfileUpdatedEvent`s for other services to replicate basic profile info (like Display Name and Avatar).

## 🧠 Domain Concepts
* **Authentication & Authorization:** Issues short-lived JWT Access Tokens and long-lived Refresh Tokens. Implements Google OAuth login.
* **Roles:** Users can be standard Guests or upgraded to Hosts.
* **Profile Management:** Handles avatar uploads via secure pre-signed URLs (S3/Cloudinary) and basic profile metadata.

## 🗄️ Database Schema (PostgreSQL)
| Table Name | Description |
|------------|-------------|
| `Users` | Core user identity (Email, PasswordHash, Roles, DisplayName, AvatarUrl). |
| `RefreshTokens` | Stores active refresh tokens to allow session revocation. |

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
