# AirVnV Platform

AirVnV is a distributed property rental platform designed with a microservices architecture. It focuses on solving real-world challenges such as data synchronization, event-driven communication, and geo-spatial searching.

## System Architecture

The system utilizes an API Gateway pattern with asynchronous event processing and Change Data Capture (CDC) to maintain read-model consistency.

```mermaid
%%{init: {'theme': 'base', 'look': 'handDrawn'}}%%
graph TD
    Client["React Frontend"] --> GW["YARP Gateway"]

    GW --> SS["Search Service"]
    GW --> PS["Property Service"]
    GW --> BS["Booking Service"]
    GW --> Pay["Payment Service"]
    GW --> CS["Chat Service"]
    GW --> US["User Service"]

    %% Database-per-Microservice
    SS --> ES[("Elasticsearch")]
    PS --> DB_Prop[("Property DB")]
    BS --> DB_Book[("Booking DB")]
    Pay --> DB_Pay[("Payment DB")]
    CS --> DB_Chat[("Chat DB")]
    US --> DB_User[("User DB")]

    %% Async Data Flows
    DB_Prop -.->|Debezium CDC| Kafka{{"Kafka"}}
    Kafka -.->|Sync| SS
```

### Observability Architecture

```mermaid
%%{init: {'theme': 'base', 'look': 'handDrawn'}}%%
graph LR
    subgraph Services
        GW["Gateway"]
        PS["Property"]
        BS["Booking"]
        Pay["Payment"]
        CS["Chat"]
        US["User"]
        SS["Search"]
    end

    Services -->|OTLP| OC["OTel Collector"]

    OC -->|remote_write| Prom[("Prometheus")]
    OC -->|otlphttp| Loki[("Loki")]
    OC -->|otlp| Tempo[("Tempo")]

    Prom & Loki & Tempo --> Grafana["📊 Grafana\n:3000"]
```

## 🗺️ Source Map & Documentation

To understand the deeper business logic and architectural decisions, refer to the internal documentation mapped to each service:

| Microservice / Module | Source Code Path | Domain & Feature Documentation |
|-----------------------|------------------|--------------------------------|
| **API Gateway**       | [`src/Airbnb.Gateway`](./src/Airbnb.Gateway) | [Routing & Auth Specs](./src/Airbnb.Gateway/README.md) |
| **Property Service**  | [`src/Airbnb.PropertyService`](./src/Airbnb.PropertyService) | [Property Domain & Endpoints](./src/Airbnb.PropertyService/README.md) |
| **Search Service**    | [`src/Airbnb.SearchService`](./src/Airbnb.SearchService) | [Search Domain & Endpoints](./src/Airbnb.SearchService/README.md) |
| **Booking Service**   | [`src/Airbnb.BookingService`](./src/Airbnb.BookingService) | [Booking Domain & Endpoints](./src/Airbnb.BookingService/README.md) |
| **Payment Service**   | [`src/Airbnb.PaymentService`](./src/Airbnb.PaymentService) | [Payment Domain & Endpoints](./src/Airbnb.PaymentService/README.md) |
| **Chat Service**      | [`src/Airbnb.ChatService`](./src/Airbnb.ChatService) | [Chat Domain & Endpoints](./src/Airbnb.ChatService/README.md) |
| **User Service**      | [`src/Airbnb.UserService`](./src/Airbnb.UserService) | [Identity Domain & Endpoints](./src/Airbnb.UserService/README.md) |
| **React Frontend**    | [`airbnb-web/`](./airbnb-web) | [Frontend Architecture Rules](./.agents/rules/frontend.md) |
| **Admin Web Panel**   | [`airbnb-admin/`](./airbnb-admin) | [Admin Architecture](./airbnb-admin/README.md) 🔹 [Features & API Map](./docs/Admin_Features.md) 🔹 [BA Specs](./docs/Admin_BA.md) |
| **Engineering Rules** | `.agents/rules/` | [Backend Rules](./.agents/rules/backend.md) 🔹 [Project Rules](./.agents/rules/project.md) |

## Tech Stack

### Frontend
* **Core:** React 18, TypeScript, Vite
* **State Management:** TanStack Query (Server State), Zustand (Client State)
* **Styling & UI:** Tailwind CSS, Shadcn/UI

### Backend Services
* **Framework:** .NET 10, C#
* **Architecture:** Vertical Slice Architecture (VSA), CQRS (Mediator), REPR Pattern (FastEndpoints)
* **Distributed Patterns:** Event-Driven Architecture (EDA), Transactional Outbox, Saga Choreography
* **Orchestration:** .NET Aspire 13

### Infrastructure & Data
* **Databases:** PostgreSQL, Redis (Caching & SignalR Backplane)
* **Search Engine:** Elasticsearch
* **Message Brokers:** RabbitMQ (Domain Events), Apache Kafka (Data Streaming)
* **Change Data Capture:** Debezium

### Observability (Grafana LGTM Stack)
* **Collection:** OpenTelemetry Collector — single ingress point fan-outing Logs, Metrics & Traces
* **Logs:** Grafana Loki — structured log aggregation with 3-day retention
* **Traces:** Grafana Tempo — distributed tracing across all microservices with TraceID linking
* **Metrics:** Prometheus — time-series metrics scraped from OTel Collector
* **Dashboard:** Grafana — unified UI with pre-provisioned datasources, zero manual configuration
* **Protocol:** All services emit OTLP natively via `ServiceDefaults`; zero instrumentation code in business logic

## Core Features & Business Logic

### 1. Property Management (Property Service)
* **Listing Lifecycle:** Implements a strict state machine (`Draft` -> `PendingReview` -> `Published`) to control property visibility.
* **Pricing Engine:** Encapsulates complex logic for base prices, cleaning fees, and weekend premium multipliers within the domain model.
* **Categorization:** Standardizes property types (Apartments, Villas, Hotels) to enforce consistent data structures.

### 2. Search & Discovery (Search Service)
* **Geo-Spatial Search:** Leverages Elasticsearch `geo_point` to find properties within a specific coordinate radius.
* **Read-Model Optimization:** Search queries never hit the transactional DB. Data is synchronized in near real-time via CDC, guaranteeing O(1) response times for complex filters.
* **Hybrid Data Hydration:** Elasticsearch holds only lightweight text/geo data to ensure blindingly fast searches. Heavy media (like property image URLs) are hydrated dynamically on the frontend via a separate call to the Property Service `/bulk` endpoint.

### 3. Booking & Reservations (Booking Service)
* **Reservation Workflow:** Manages atomic state transitions for bookings (`Pending` -> `Confirmed` -> `Cancelled`).
* **Availability Enforcement:** Prevents double-booking through strict date blocking and concurrency controls.

### 4. Context-Based Communications (Chat Service)
* **Domain-Bound Threads:** Conversations are strictly tied to a `Property` or `Booking` context, eliminating arbitrary P2P messaging.
* **Event-Driven System Messages:** Listens to integration events (e.g., `BookingConfirmed` via RabbitMQ) to automatically inject un-editable system notifications into the chat thread.
* **WebRTC Signaling:** Provides a lightweight signaling mechanism via SignalR to negotiate P2P Audio/Video calls between hosts and guests without routing media streams through the backend.

### 5. Payment Processing (Payment Service)
* **Secure Transactions:** Manages payment intents and webhooks, integrating seamlessly with external payment gateways (e.g., Stripe, VNPay).
* **Asynchronous Fulfillment:** Emits `PaymentCompletedEvent` to RabbitMQ upon success, triggering downstream actions like booking confirmation and host notifications without stalling the client.

### 6. Observability & Monitoring
* **Zero-Touch Instrumentation:** All services are instrumented via the shared `ServiceDefaults` project using the OpenTelemetry SDK — no per-service boilerplate required.
* **Unified Telemetry Pipeline:** An OTel Collector aggregates Logs, Distributed Traces, and Metrics from every microservice, then fans out to the appropriate backend.
* **End-to-End Tracing:** A single HTTP request generates a correlated trace spanning Gateway → Service → DB, all navigable from Grafana Tempo. Clicking a TraceID in a Loki log jumps directly to the corresponding trace.
* **Production-Ready Budgets:** All 5 monitoring containers are constrained to ~128 MB RAM each via `GOGC=50` + `GOMAXPROCS=1`, bringing the full observability stack overhead to ~500 MB.

## Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Node.js 18+](https://nodejs.org/)
* Docker Desktop (Required for .NET Aspire to provision infrastructure containers)

### Running the Project

**1. Start the Backend & Observability Stack**
Navigate to the root and run the Aspire AppHost. This automatically provisions all infrastructure containers (Postgres, Redis, Kafka, RabbitMQ, Elasticsearch) **plus the full Grafana monitoring stack** (OTel Collector, Loki, Tempo, Prometheus, Grafana).
```bash
dotnet watch run --project Airbnb.AppHost
```

| Service | URL |
|---------|-----|
| Aspire Dashboard | `https://localhost:17xxx` (printed in console) |
| Grafana | `http://localhost:3000` (no login required in dev) |
| RabbitMQ UI | `http://localhost:15672` |
| Kafka UI | `http://localhost:8080` |

**2. Start the Frontend Application (Guest/Host)**
Open a new terminal session and start the React application.
```bash
cd airbnb-web
npm install
npm run dev
```

**3. Start the Admin Web Panel**
Open another terminal session and start the Next.js Admin application.
```bash
cd airbnb-admin
npm install
npm run dev
```
*Note: The Admin Panel will be available at `http://localhost:9999`.*

### Generating Docker Compose (from Aspire)

To generate a `docker-compose.yml` deployment from the .NET Aspire orchestration, use the [Aspirate](https://github.com/prom3theu5/aspirational-manifests) CLI tool:

```bash
cd Airbnb.AppHost
aspirate generate
```

*Note: This will parse the AppHost graph and automatically scaffold your `docker-compose.yml` and `.env` files.*
*Note: You must install the Aspirate CLI separately before running this command.*