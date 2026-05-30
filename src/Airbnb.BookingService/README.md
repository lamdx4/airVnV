# Booking Service

**Booking Service** is the engine that handles reservations. It is responsible for enforcing availability, locking dates to prevent double-booking, and managing the state transition of a booking lifecycle.

## 🧠 Domain Concepts
* **Concurrency & Double-Booking Prevention:** Employs database row-level locking or optimistic concurrency to ensure two users cannot book the exact same dates simultaneously.
* **Booking Lifecycle:** `Pending` (awaiting payment) -> `Confirmed` (payment success) -> `Cancelled` / `Completed`.
* **Outbox Pattern:** When a booking is confirmed, it writes a `BookingConfirmedEvent` to a local Outbox table in the same transaction. A background worker then publishes this to RabbitMQ, ensuring 100% consistency.

## 🗄️ Database Schema (PostgreSQL)
| Table Name | Description |
|------------|-------------|
| `Bookings` | The core aggregate tracking GuestId, PropertyId, CheckIn, CheckOut, TotalPrice, and Status. |
| `OutboxMessages` | Transactional outbox for RabbitMQ domain events. |

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **POST** | `/api/bookings` | Create a new booking (Pending status). |
| **GET**  | `/api/bookings/{id}` | Get details of a specific booking. |
| **GET**  | `/api/bookings/my-trips` | Get all bookings made by the current user. |
| **GET**  | `/api/bookings/my-reservations` | Get all bookings made on the host's properties. |
| **POST** | `/api/bookings/{id}/cancel` | Cancel an active booking (subject to refund rules). |
