# Aspire Postgres MCP — Setup & Usage Guide

## Tổng quan

Dự án sử dụng **Aspire Resource MCP Servers** để expose Postgres databases (userdb, propdb, bookdb, paydb, chatdb) dưới dạng MCP tools, cho phép AI agents query trực tiếp mà không cần quản lý connection strings thủ công.

## Những lỗi đã gặp & Nguyên nhân

| Lỗi | Nguyên nhân | Cách fix |
|-----|-------------|----------|
| `password authentication failed` trong `postgres___query` | Connection string trong `mcp.json` bị split thành nhiều args do ký tự đặc biệt (`{`, `(`, `.`) trong password | Dùng `WithPostgresMcp()` thay vì cấu hình thủ công |
| `ASPIREPOSTGRES001` build error | API `WithPostgresMcp()` là experimental | Thêm `#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001` |
| Aspire MCP container không start | Chưa cài `aspire` CLI | `dotnet tool install --global aspire.cli` |
| PowerShell break JSON args khi gọi `aspire mcp call` | PowerShell parse nội dung JSON thành nhiều arguments | Dùng `--%` (stop-parsing) trước `--input` |
| Dynamic tools không xuất hiện trong Factory Deferred Tools | Aspire proxy tools chưa được Factory discovery surface | Dùng CLI bridge: `aspire mcp call` |

## Setup (1 lần duy nhất)

### 1. Cài đặt Aspire CLI

```bash
dotnet tool install --global aspire.cli
```

### 2. Thêm `WithPostgresMcp()` vào AppHost

```csharp
// Airbnb.AppHost/AppHost.cs
#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("airbnb_pg_data")
    .WithEnvironment("POSTGRES_INITDB_ARGS", "-c wal_level=logical")
    .WithEndpoint("tcp", e => { e.Port = 5435; e.TargetPort = 5432; });

var userDb = postgres.AddDatabase("userdb").WithPostgresMcp();
var propDb = postgres.AddDatabase("propdb").WithPostgresMcp();
var bookDb = postgres.AddDatabase("bookdb").WithPostgresMcp();
var payDb  = postgres.AddDatabase("paydb").WithPostgresMcp();
var chatDb = postgres.AddDatabase("chatdb").WithPostgresMcp();
```

### 3. Cấu hình Factory MCP (`~/.factory/mcp.json`)

```json
{
  "mcpServers": {
    "aspire": {
      "type": "stdio",
      "command": "aspire",
      "args": ["agent", "mcp"]
    },
    "postgres-userdb": { "disabled": true },
    "postgres":        { "disabled": true },
    "postgres-chatdb": { "disabled": true },
    "postgres-paydb":  { "disabled": true }
  }
}
```

> Vô hiệu hóa các MCP server cũ (connection string thủ công), thay bằng Aspire proxy tự động.

### 4. Khởi động Aspire

```bash
cd Airbnb.AppHost
dotnet run
```

Aspire sẽ tự động spin up 5 MCP containers (`crystaldba/postgres-mcp:0.3.0`), mỗi cái connect đến đúng database.

## Sử dụng

### Liệt kê MCP tools có sẵn

```bash
aspire mcp tools
```

Output mẫu:

| Resource    | Tool                    | Mô tả                                    |
|-------------|-------------------------|-------------------------------------------|
| userdb-mcp  | execute_sql             | Execute any SQL query                     |
| userdb-mcp  | list_schemas            | List all schemas in the database          |
| userdb-mcp  | list_objects            | List objects in a schema                  |
| userdb-mcp  | get_object_details      | Show detailed info about a database object|
| userdb-mcp  | explain_query           | Explain the execution plan for a SQL query|
| userdb-mcp  | analyze_query_indexes   | Recommend optimal indexes for queries     |
| propdb-mcp  | execute_sql             | (tương tự)                               |
| bookdb-mcp  | execute_sql             | (tương tự)                                |
| paydb-mcp   | execute_sql             | (tương tự)                                |
| chatdb-mcp  | execute_sql             | (tương tự)                                |

### Query SQL qua CLI

**Lưu ý quan trọng:** Trong PowerShell, phải dùng `--%` (stop-parsing symbol) trước `--input` để tránh PowerShell break JSON thành nhiều arguments.

```bash
# Xem danh sách tables
aspire mcp call userdb-mcp execute_sql --% --input "{\"sql\": \"SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' ORDER BY table_name\"}"

# Query Users table
aspire mcp call userdb-mcp execute_sql --% --input "{\"sql\": \"SELECT \\\"Id\\\", \\\"Email\\\", \\\"Role\\\", \\\"Status\\\" FROM \\\"Users\\\" LIMIT 5\"}"

# Xem schema của table
aspire mcp call userdb-mcp get_object_details --% --input "{\"schema_name\": \"public\", \"object_name\": \"Users\"}"

# Explain query plan
aspire mcp call userdb-mcp explain_query --% --input "{\"sql\": \"SELECT * FROM \\\"Users\\\" WHERE \\\"Email\\\" = 'admin@airbnb.com'\"}"
```

### Qua Aspire MCP tools trong Droid session

Các tools có sẵn qua `aspire___` prefix:

| Tool | Mô tả |
|------|-------|
| `aspire___list_resources` | Liệt kê tất cả resources, state, health, endpoints |
| `aspire___list_apphosts` | Liệt kê running AppHosts |
| `aspire___list_console_logs` | Xem console logs của 1 resource |
| `aspire___list_structured_logs` | Xem structured logs (OTel) |
| `aspire___list_traces` | Xem distributed traces |
| `aspire___execute_resource_command` | Start/stop/restart resource |
| `aspire___doctor` | Kiểm tra Aspire environment |
| `aspire___refresh_tools` | Refresh danh sách tools sau khi thêm resource MCP |

### Kết hợp: dùng `aspire mcp call` trong Droid

Vì dynamic proxied tools chưa surface qua Factory Deferred Tools, dùng CLI bridge:

```
# Trong Droid session, request query:
aspire mcp call userdb-mcp execute_sql --% --input "{\"sql\": \"...\"}"
```

## Database resources trong project

| Database   | Resource Name | MCP Container   | Tables chính |
|------------|---------------|-----------------|--------------|
| userdb     | `userdb`      | `userdb-mcp`    | Users, UserProfiles, UserLogins, UserRefreshTokens, UserSuspensions, KycDocuments |
| propdb     | `propdb`      | `propdb-mcp`    | Properties, PropertyImages, PropertyPricing, PropertyAmenities |
| bookdb     | `bookdb`      | `bookdb-mcp`    | Bookings, BookingPayments, BookingStatusHistory |
| paydb      | `paydb`       | `paydb-mcp`     | Payments, PaymentRefunds, Transactions |
| chatdb     | `chatdb`      | `chatdb-mcp`    | Conversations, Messages, InboxState |

## Troubleshooting

| Vấn đề | Kiểm tra |
|---------|----------|
| MCP container không running | `aspire___list_resources` → xem state của `*-mcp` containers |
| `aspire` command not found | `dotnet tool install --global aspire.cli` |
| Build fail `ASPIREPOSTGRES001` | Thêm `#pragma warning disable ASPIREMCP001, ASPIREPOSTGRES001` đầu file |
| JSON parse error khi `aspire mcp call` | Đảm bảo dùng `--%` trước `--input` trong PowerShell |
| Port thay đổi mỗi lần restart | Bình thường — Aspire dynamic port, MCP proxy tự động follow |
| HTTPS dev cert warning | `aspire certs trust` |
| Docker version warning | Upgrade Docker Desktop >= 28.0.0 |

## Tham khảo

- [Aspire Resource MCP Servers](https://aspire.dev/get-started/resource-mcp-servers/)
- [Aspire MCP Server for AI Agents](https://aspire.dev/get-started/aspire-mcp-server/)
- [PostgreSQL Hosting Integration](https://aspire.dev/integrations/databases/postgres/postgres-host/)
- [ASPIREPOSTGRES001 diagnostic](https://aspire.dev/diagnostics/aspirepostgres001/)
