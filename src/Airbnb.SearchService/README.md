# Search Service

**Search Service** is a highly optimized "Read Model" service designed to handle complex geo-spatial queries, filtering, and full-text search for properties. It completely bypasses the primary Postgres database to guarantee sub-millisecond response times under heavy load.

## 🧠 Domain Concepts

* **Geo-Spatial Search:** Properties are indexed with geographic coordinates. Users can search for properties within a specific radius of a location (e.g., "Properties within 5km of Hanoi").
* **Pre-Aggregation:** Calculating ratings or aggregating amenities on-the-fly is expensive. This service expects data to be pre-calculated and flattened into documents.
* **Eventual Consistency (CDC):** This service does NOT manage its own writes from users. Instead, it passively consumes a Change Data Capture (CDC) stream (via Debezium and Kafka) from the `PropertyService`'s Postgres database.

## 🗄️ Database Schema (Elasticsearch)

Instead of relational tables, this service uses **Elasticsearch Documents**.

| Index Name | Description |
|------------|-------------|
| `properties` | Flat document containing ID, Title, Geo-Point (Lat/Lng), Pricing, Pre-calculated Ratings, and Property Type. Optimized for rapid filtering and radius querying. |

## 🔌 API Endpoints (FastEndpoints)

| Method | Path | Description |
|--------|------|-------------|
| **GET** | `/api/search` | Search properties based on coordinates (lat/lng), radius, price range, property type, and dates. |

## 🔄 Background Workers

| Worker | Description |
|--------|-------------|
| `CdcConsumer` | Subscribes to the Kafka topic `airbnb.public.Properties` (populated by Debezium). Upserts or Deletes Elasticsearch documents whenever a Postgres record is modified. |
