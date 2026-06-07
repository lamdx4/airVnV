# Project Structure — AirVnV

Tài liệu tổng hợp cấu trúc thư mục toàn bộ dự án, bao gồm Backend Microservices, Frontend Web, Admin UI và Shared Infrastructure.

---

## Tổng quan Repository

```
AirvnV/
├── src/                              # Backend microservices (.NET 8, C#)
├── airbnb-web/                        # React 19 frontend (Vite, Shadcn/UI)
├── airbnb-admin/                      # Next.js 16 admin UI (Shadcn/UI, TanStack Query)
├── tests/                             # Backend integration/unit tests (xUnit)
├── Airbnb.AppHost/                    # .NET Aspire orchestration
├── Airbnb.SharedKernel/               # Shared domain primitives (AggregateRoot, IDomainEvent)
├── Airbnb.ServiceDefaults/            # Common service configuration (ApiResponse<T>, health checks)
├── Airbnb.Infrastructure.Configurator/ # Shared infrastructure setup
├── Airbnb.Infrastructure.Media/       # Media handling infrastructure
├── docs/                              # Project documentation
├── scripts/                           # Utility scripts
├── specs/                             # Feature specifications
├── .agents/rules/                     # Backend, frontend, project rules
├── .github/workflows/                 # CI pipeline
└── Airbnb.slnx                        # Solution file
```

---

## 1. Backend Microservices (src/)

Mỗi service tuân thủ **cùng một cấu trúc canonical** — 3 tầng: `Domain/`, `Features/`, `Infrastructure/`.

### 1.1 Canonical Service Structure

```
ServiceRoot/
├── Domain/
│   ├── Entities/              # Satellite entities (Amenity, PropertyImage, Review...)
│   ├── Enums/                # Status, type enums (PropertyStatus, PropertyType...)
│   ├── ValueObjects/         # Immutable VOs saved as JSONB (Pricing, AddressRaw...)
│   ├── DomainEvents.cs       # Domain event definitions (past tense)
│   └── <Aggregate>.cs        # Aggregate Root — rich domain model with guarded state transitions
│
├── Features/                 # Vertical Slice: 1 folder = 1 use case
│   └── <UseCase>/
│       ├── Endpoint.cs        # Thin: await mediator.Send(req, ct)
│       ├── Request.cs         # Mediator.ICommand<T> (write) or Mediator.IQuery<T> (read)
│       ├── Response.cs        # Response DTO
│       ├── Handler.cs         # ICommandHandler or IQueryHandler — all business logic here
│       └── Validator.cs       # (optional) FastEndpoints.Validator<Request>
│
├── Infrastructure/
│   ├── Configurations/        # EF Core IEntityTypeConfiguration<T> (Fluent API)
│   ├── HttpClients/           # Cross-service HTTP clients (Refit/IHttpClientFactory)
│   ├── Messaging/             # MassTransit consumers, DomainEventPublisher, topic mapping
│   ├── PaymentGateways/       # (PaymentService only) IPaymentProvider, VnpayProvider, Resolver
│   ├── Saga/                  # (BookingService only) Automatonymous state machine
│   ├── Workers/               # Background workers
│   └── <DbContext>.cs         # EF Core DbContext + Migrations
│
├── Migrations/                # EF Core schema migrations
├── Properties/                # launchSettings.json
├── Program.cs                 # Composition root (services, middleware, Mediator, MassTransit)
├── GlobalUsings.cs            # Global using directives
├── <ServiceJsonContext>.cs    # [JsonSerializable] DTOs for Native AOT readiness
├── <Service>.csproj
└── appsettings.json
```

### 1.2 Chi tiết từng Service

#### Airbnb.PropertyService (26 use cases — richest domain)

```
Domain/
  Property.cs                     # Aggregate Root (Submit, Approve, Suspend, Archive, Reject)
  DomainEvents.cs                 # PropertyPublishedEvent, PropertySuspendedEvent, PropertyRejectedEvent
  Entities/
    Amenity.cs                    # Global amenity catalog
    PropertyAmenity.cs            # Property ↔ Amenity link
    PropertyAvailability.cs       # Calendar: date + price + status
    PropertyImage.cs              # Images with Factory Method (Create)
    Review.cs                     # Guest reviews
    AddressFieldConfig.cs         # Admin division config
  Enums/
    PropertyStatus.cs             # Draft, Pending, Active, Suspended, Archived, Rejected
    PropertyType.cs               # Apartment, House, Villa, Cabin, Cottage, Boat
    BookingMode.cs                # Instant, Request
    ImageType.cs                  # Cover, Gallery
  ValueObjects/
    Pricing.cs                    # BasePrice, WeekendPrice, WeeklyDiscount, CleaningFee
    AddressRaw.cs                 # CountryCode, City, StreetLine1, Lat, Lng...
    PropertyCapacity.cs           # MaxGuests, Bedrooms, Beds, Bathrooms
    HouseRules.cs                 # CheckIn, CheckOut, NoSmoking, NoPets...

Features/
  CreateProperty                  # Host creates draft listing
  UpdateProperty / UpdateLocation / UpdateAmenityInfo   # Partial updates
  SubmitProperty                  # Draft → Pending (submit for review)
  ApproveProperty                 # Pending → Active (admin approve)
  RejectProperty                  # Pending → Rejected (admin reject with reason)
  SuspendProperty                 # Active → Suspended (admin emergency)
  ArchiveProperty                 # Active → Archived (host archive)
  ReinstateProperty               # Archived → Active (host restore)
  DeleteProperty                  # Host soft-delete own listing
  AdminDeleteProperty             # Admin force-delete any listing
  GetProperty / GetPropertyBasicInfo   # Public detail & internal basic info
  GetMyProperties                 # Host's own listings
  GetAdminProperties              # Admin paginated list with filters
  GetAdminStats / GetRecentActivity  # Dashboard stats
  ManageAmenities / GetAvailableAmenities  # Amenity CRUD
  ManageAvailability              # Calendar management
  ManageImages                    # Image upload/reorder
  UpdateStatus                    # Generic status transition
  AddReview / GetReviews / UpdateReview / DeleteReview  # Review CRUD

Infrastructure/
  AppDbContext.cs + PropertyJsonContext.cs
  Configurations/ (6 files — one per entity)
  HttpClients/ BookingServiceClient.cs
  Messaging/ DomainEventPublisher.cs, PropertyTopics.cs
```

#### Airbnb.BookingService (11 use cases)

```
Domain/
  Booking.cs                      # Aggregate Root (Create, Approve, Reject, Cancel, Confirm)
  BookingEvents.cs                # BookingCreatedEvent, BookingConfirmedEvent, BookingCancelledEvent
  Enums/

Features/
  CreateBooking / ApproveBooking / RejectBooking / CancelBooking
  GetGuestBookings / GetHostBookings / GetBookingBasicInfo
  GetAdminStats / GetRevenueChart
  MasterData                      # Reference data for booking
  Consumers/
    PaymentSucceededConsumer.cs   # → Booking CONFIRMED (idempotent)
    PaymentFailedConsumer.cs      # → Booking CANCELLED + compensating TX
    BookingApprovalTimeoutConsumer.cs  # Auto-reject on timeout

Infrastructure/
  BookingDbContext.cs + BookingJsonContext.cs
  HttpClients/
  Messaging/
    BookingDomainEventPolicyExecutor.cs   # Dispatches domain events to outbox
    BookingIntegrationEventBridge.cs      # Bridge domain → integration events
    BookingIntegrationEventMapper.cs      # Maps event types to MassTransit topics
    DomainEventNotification.cs            # MediatR notification wrapper
  Saga/
    BookingStateMachine.cs        # Automatonymous: Pending → Confirmed/Cancelled
    BookingState.cs               # Saga state instance
    BookingSagaDbContext.cs       # Saga persistence DB
  Workers/
```

#### Airbnb.PaymentService (17 use cases)

```
Domain/
  Payment.cs                      # Aggregate Root (Create with Factory Method)
  Payout.cs                       # Host payout aggregate
  PayoutItem.cs                   # Individual payout line items per booking
  RefundRecord.cs                 # Refund tracking
  PlatformFeeConfig.cs            # Admin-configurable commission rates
  PaymentEvents.cs                # PaymentInitiatedEvent, PaymentSucceededEvent, PaymentFailedEvent

Features/
  InitiatePayment / ConfirmPayment / FailPayment / RefundPayment
  VnpayIpn                        # VNPay IPN callback handler
  GeneratePayouts / ExecutePayout / ApprovePayout / CancelPayout / RetryPayout
  GetAdminPayments / GetAdminPaymentDetail
  GetAdminPayouts / GetAdminPayoutDetail
  CreateAdminPlatformFee / GetAdminPlatformFeeCurrent / GetAdminPlatformFeeHistory

Infrastructure/
  PaymentDbContext.cs + PaymentJsonContext.cs + PaymentDbContextFactory.cs
  HttpClients/
  Messaging/
  PaymentGateways/
    IPaymentProvider.cs           # Strategy interface (Initiate, Verify, Refund)
    VnpayProvider.cs              # VNPay implementation
    PaymentProviderResolver.cs    # Runtime strategy selection by CountryCode
  VnpayLibrary.cs                 # VNPay hashing & signature utility
```

#### Airbnb.UserService (8 feature groups)

```
Domain/
  User.cs                         # Aggregate Root (Guest/Host/Admin roles, KYC, profile)
  Events/

Features/
  Login/                          # Email + password login
  RegisterUser/                   # Registration with role selection
  GoogleAuth/                     # Google OAuth login
  Profile/                        # Profile CRUD + avatar upload
  RefreshToken/                   # JWT refresh token flow
  Account/                        # Password change, account management
  Media/                          # Profile image upload
  Admin/                          # Admin user management endpoints

Infrastructure/
  UserDbContext.cs + UserJsonContext.cs + UserDbContextFactory.cs
  HttpClients/
  Messaging/
  Migrations/
```

#### Airbnb.ChatService (3 feature groups)

```
Domain/
  Conversation.cs                 # Aggregate (PropertyId, ReservationId, LastMessageAt)
  ConversationParticipant.cs      # Snapshot: DisplayName, AvatarUrl, LastReadMessageId
  Message.cs                      # UUIDv7 Id, SenderId?, MessageType (Text/Image/System)
  Enums.cs                        # ParticipantRole, MessageType

Features/
  Conversations/
    Create/                       # StartConversation (Inquiry)
    GetInbox/                     # Sorted by LastMessageAt DESC
    GetMessages/                  # Paginated chat history
    SendMessage/                  # Text or image message
    MarkAsRead/                   # Update LastReadMessageId (UUIDv7 comparison)
    Archive/                      # Per-participant archive toggle
  Consumers/
    BookingConfirmedEventConsumer.cs    # Auto system message + update ReservationId
    UserProfileUpdatedEventConsumer.cs  # Sync DisplayName/AvatarUrl snapshots
  Hubs/
    ChatHub.cs                    # SignalR real-time hub (Redis Backplane)

Infrastructure/
  AppDbContext.cs + AppDbContextFactory.cs
  HttpClients/
  Migrations/
```

#### Airbnb.SearchService (1 use case)

```
Domain/
  PropertyDoc.cs                  # Elasticsearch document (Id, Name, PricePerNight, AddressVO...)

Features/
  SearchProperties/               # Single search use case
    Endpoint.cs / Request.cs / Response.cs / Handler.cs

Infrastructure/
  CdcConsumer.cs                  # Consumes Kafka CDC topic → upserts into Elasticsearch
  SearchJsonContext.cs
```

#### Airbnb.Gateway (no domain/features — pure reverse proxy)

```
Airbnb.Gateway/
├── Program.cs                    # YARP configuration + JWT authentication
├── appsettings.json              # Route mappings to internal services
├── appsettings.Development.json
├── Airbnb.Gateway.csproj
└── Properties/
```

---

## 2. Shared Projects

### Airbnb.SharedKernel

```
Airbnb.SharedKernel/
├── Domain/
│   ├── AggregateRoot.cs          # Base entity with Raise()/ClearDomainEvents()
│   └── IDomainEvent.cs           # Marker interface for domain events
├── Events/                       # Shared event contracts cross-service
└── Infrastructure/               # Shared infrastructure helpers
```

### Airbnb.ServiceDefaults

```
Airbnb.ServiceDefaults/
└── ApiResponse<T>                # Unified envelope: Data, Success, ErrorCode, Timestamp
    + ExceptionHandlingMiddleware # Global catch → ApiResponse with Success=false
    + Health check defaults
    + OpenTelemetry setup
```

---

## 3. Frontend — airbnb-web (React 19, Vite, Shadcn/UI)

```
airbnb-web/
├── src/
│   ├── App.tsx                   # Router setup
│   ├── main.tsx                  # Entry point
│   ├── index.css / App.css       # Global styles (Tailwind)
│   ├── features/                 # Feature-sliced architecture
│   │   ├── account/              #   api/ types/ hooks/ components/ utils/
│   │   ├── admin/
│   │   ├── auth/
│   │   ├── booking/
│   │   ├── chat/
│   │   ├── media/
│   │   ├── payment/
│   │   ├── profile/
│   │   ├── properties/
│   │   ├── reviews/
│   │   └── search/
│   ├── components/               # Shared UI components
│   ├── hooks/                    # Shared hooks
│   ├── lib/                      # Axios client with interceptors
│   ├── pages/                    # Route page components
│   ├── store/                    # Zustand stores (auth, theme only)
│   └── assets/                   # Static assets
├── tests/                        # Playwright E2E specs
├── package.json
├── vite.config.ts
├── tsconfig.json
└── playwright.config.ts
```

**Feature convention** (each feature folder):

```
features/<feature>/
├── api/          # Axios calls (no state, no navigation)
├── types/        # DTO interfaces + Model interfaces
├── utils/        # Mappers (DTO→Model), validation schemas (Zod)
├── hooks/        # TanStack Query wrappers (useQuery, useMutation)
└── components/   # Feature-specific UI components
```

---

## 4. Admin UI — airbnb-admin (Next.js 16, Shadcn/UI, TanStack Query)

```
airbnb-admin/
├── src/
│   ├── app/                      # Next.js App Router
│   │   ├── (auth)/               # Unauthenticated route group
│   │   │   └── login/
│   │   ├── (admin)/              # Sidebar-protected route group
│   │   │   ├── layout.tsx        # AdminLayout (Sidebar + Header)
│   │   │   ├── dashboard/
│   │   │   ├── properties/
│   │   │   ├── bookings/
│   │   │   ├── payments/
│   │   │   ├── reviews/
│   │   │   ├── reports/
│   │   │   ├── settings/
│   │   │   └── users/
│   │   ├── globals.css
│   │   ├── layout.tsx            # Root layout (providers)
│   │   └── page.tsx              # Redirect to dashboard
│   ├── features/                 # Feature-sliced architecture
│   │   ├── dashboard/            #   api/ types/ hooks/ components/ utils/
│   │   ├── properties/
│   │   ├── bookings/
│   │   ├── payments/
│   │   ├── payouts/
│   │   ├── reviews/
│   │   ├── reports/
│   │   ├── settings/
│   │   └── users/
│   ├── components/
│   │   ├── layout/               # AdminSidebar, AdminHeader, Breadcrumbs
│   │   ├── common/               # DataTable, StatCard, ConfirmDialog, PageLoader
│   │   └── ui/                   # Shadcn/UI primitives
│   ├── lib/
│   │   ├── api/                  # Axios client (Bearer token, 401 refresh, ApiResponse unwrap)
│   │   ├── auth/                 # Admin auth API
│   │   └── utils/               # cn, formatCurrency, formatDate, error helpers
│   ├── config/                   # ROUTES, API_BASE_URL, sidebar nav items
│   ├── providers/                # QueryProvider, AuthHydrator + AuthGuard, ThemeProvider
│   ├── store/                    # Zustand stores (useAuthStore only)
│   └── types/                    # ApiResponse<T>, PaginatedResponse<T>, PaginationParams
├── package.json
├── next.config.ts
└── tsconfig.json
```

**Feature convention** (each feature folder — same as airbnb-web + barrel export):

```
features/<feature>/
├── api/          # Axios calls via @/lib/api (paths WITHOUT /api/ prefix)
├── types/        # DTO + Model types, enum label maps
├── utils/        # Mappers, status configs, validation (Zod)
├── hooks/        # TanStack Query with QUERY_KEYS constant
├── components/   # Feature-specific UI
└── index.ts      # Barrel: export * from api, hooks, components; export type * from types
```

---

## 5. Testing

```
tests/
├── Airbnb.PropertyService.Tests/     # DomainEventPublisher exhaustiveness test, entity tests
├── Airbnb.BookingService.Tests/      # Saga state machine tests, consumer tests
└── Airbnb.PaymentService.Tests/      # Payment provider tests, idempotency tests

airbnb-web/tests/                     # Playwright E2E specs (*.spec.ts)
```

---

## 6. Key Conventions Summary

| Layer | Convention |
|---|---|
| **Backend use case** | 1 folder = 1 use case: `Endpoint.cs`, `Request.cs`, `Response.cs`, `Handler.cs`, `Validator.cs` |
| **Command (write)** | `Request : Mediator.ICommand<Response>` + `ICommandHandler` — always fully-qualified |
| **Query (read)** | `Request : Mediator.IQuery<Response>` + `IQueryHandler` |
| **Endpoint** | Thin — only `await mediator.Send(req, ct)`, no business logic, no LINQ |
| **Response** | All wrapped in `ApiResponse<T>` envelope |
| **Errors** | `BusinessException(message, errorCode)` or `NotFoundException` — no try-catch, no manual wrapping |
| **Domain model** | Rich Domain — private setters, `static Create()`, guarded state transitions |
| **Events** | Domain Events → MassTransit Outbox → RabbitMQ. CDC → Kafka → Elasticsearch |
| **DTOs** | Must declare `[JsonSerializable]` on `JsonSerializerContext` (Native AOT) |
| **Frontend state** | Server state → TanStack Query. Global state → Zustand (auth/theme only). Local → useState |
| **Frontend types** | No `any`. DTO from API, Model for UI — mapper in `utils/` |
| **Admin API paths** | No `/api/` prefix — `baseURL` already ends with `/api` |
| **Database** | DB-per-microservice, no cross-DB queries, reference by Guid ID only |
