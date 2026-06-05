# API Gateway (YARP)

**API Gateway** acts as the single entry point for all client requests (Frontend, Mobile App). It routes HTTP traffic to the appropriate downstream microservices based on URL paths.

## 🧠 Concepts & Responsibilities
* **Reverse Proxy:** Built using Microsoft's YARP (Yet Another Reverse Proxy), offering high-performance asynchronous routing.
* **Authentication Offloading:** Validates JWT tokens at the edge. If the token is invalid, the request is rejected with a 401 Unauthorized before it even reaches the downstream microservices.
* **Rate Limiting:** Protects backend services from DDoS attacks or spamming by enforcing request quotas per IP.
* **CORS Management:** Centralized Cross-Origin Resource Sharing policy enforcement for the web frontend.

## 🗄️ Configuration

This service does not have a database. Its entire state is driven by configuration files (`appsettings.json`) which define Routes and Clusters.

| Path Prefix | Destination Cluster |
|-------------|---------------------|
| `/api/users/*` | `UserService` |
| `/api/properties/*` | `PropertyService` |
| `/api/search/*` | `SearchService` |
| `/api/bookings/*` | `BookingService` |
| `/api/payments/*` | `PaymentService` |
| `/api/chat/*` | `ChatService` |
| `/hubs/chat/*` | `ChatService` (WebSocket Proxying) |
