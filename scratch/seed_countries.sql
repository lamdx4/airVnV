-- Clean up existing (if any)
DELETE FROM "Taxes" WHERE "CountryCode" IN ('VN', 'US');
DELETE FROM "PaymentGateways" WHERE "CountryCode" IN ('VN', 'US');
DELETE FROM "Countries" WHERE "Code" IN ('VN', 'US');

-- Seed Countries with camelCase JSONB keys
INSERT INTO "Countries" ("Code", "Name", "NativeCurrency", "IsSupported", "AddressFormConfig", "Version", "DefaultLatitude", "DefaultLongitude")
VALUES 
('VN', 'Vietnam', 'VND', true, '[
  { "id": "admin1", "label": "Tỉnh / Thành phố", "photonKeys": ["state", "city"], "isRequired": true, "type": "text" },
  { "id": "admin2", "label": "Quận / Huyện / Phường", "photonKeys": ["county", "district", "locality", "suburb", "town"], "isRequired": true, "type": "text" },
  { "id": "street", "label": "Số nhà, tên đường", "photonKeys": ["street", "name"], "isRequired": true, "type": "text" },
  { "id": "unit", "label": "Căn hộ / Số phòng (Nếu có)", "photonKeys": [], "isRequired": false, "type": "text" }
]'::jsonb, 0, 21.0285, 105.8542),
('US', 'United States', 'USD', true, '[
  { "id": "street", "label": "Street Address", "photonKeys": ["street", "name"], "isRequired": true, "type": "text" },
  { "id": "admin1", "label": "State", "photonKeys": ["state"], "isRequired": true, "type": "text" },
  { "id": "zipcode", "label": "Zip Code", "photonKeys": ["postcode"], "isRequired": true, "type": "text" },
  { "id": "unit", "label": "Apt, Suite, Bldg", "photonKeys": [], "isRequired": false, "type": "text" }
]'::jsonb, 0, 37.0902, -95.7129);

-- Seed Taxes (if schema needs them, use UUIDs for Id)
INSERT INTO "Taxes" ("Id", "CountryCode", "Type", "Rate", "IsActive", "Version")
VALUES
('00000000-0000-0000-0000-000000000001', 'VN', 'VAT', 0.10, true, 0),
('00000000-0000-0000-0000-000000000002', 'US', 'Sales Tax', 0.08, true, 0);

-- Seed Payment Gateways (Vietnamese VNPay and Stripe)
INSERT INTO "PaymentGateways" ("Id", "CountryCode", "Provider", "SupportedCurrencies", "IsActive", "Version")
VALUES
('00000000-0000-0000-0000-000000000003', 'VN', 'vnpay', ARRAY['VND'], true, 0),
('00000000-0000-0000-0000-000000000005', 'VN', 'Stripe', ARRAY['VND', 'USD'], true, 0),
('00000000-0000-0000-0000-000000000004', 'US', 'Stripe', ARRAY['USD'], true, 0);
