# Chat Service

**Chat Service** manages all real-time communication between Hosts and Guests. Unlike standard messaging apps, conversations here are strictly tied to a specific business context (a Property or a Booking).

## 🧠 Domain Concepts
* **Context-Bound Threads:** A conversation cannot exist in a vacuum. It must be linked to a `ContextId` (e.g., BookingId).
* **System Messages:** The service listens to RabbitMQ for events (like `BookingConfirmed` or `BookingCancelled`) and automatically injects immutable System Messages into the chat thread.
* **Real-Time Delivery:** Utilizes SignalR for WebSockets. A Redis Backplane is used to scale SignalR across multiple server instances.
* **Data Replication:** Since this service needs user names and avatars to display chat heads quickly, it subscribes to `UserProfileUpdatedEvent` via RabbitMQ and maintains a read-optimized copy of users in its own DB.

## 🗄️ Database Schema (PostgreSQL)
| Table Name | Description |
|------------|-------------|
| `Conversations` | The chat thread, including the ContextId, PropertyId, and LastMessageSnippet. |
| `Messages` | Individual messages (Text, Image, System Alerts), SenderId, and SentAt. |
| `Participants` | Link table tracking who is in the conversation and their LastRead watermark. |
| `Users` | (Replica) Local read-model of users synced via RabbitMQ to avoid querying UserService. |

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **GET** | `/api/chat/conversations` | Get all active conversations for the current user. |
| **GET** | `/api/chat/conversations/{id}/messages` | Get paginated messages for a thread. |
| **POST**| `/api/chat/conversations/{id}/messages` | Send a new text/image message. |
| **POST**| `/api/chat/conversations/{id}/read` | Update the read-receipt watermark. |

## 📡 SignalR Hubs
* `/hubs/chat` - Real-time WebSocket connection for receiving incoming messages instantly.
