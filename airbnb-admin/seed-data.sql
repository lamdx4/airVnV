-- ============================================================
-- SEED DATA FOR AIRBNB ADMIN TESTING
-- Run against: userdb, propdb, bookdb, paydb (separate DBs)
-- ============================================================

-- ============================================================
-- PART 1: USERDB — Users + Profiles
-- ============================================================
-- Connect to: userdb

-- Admin (already exists from prior seed, use ON CONFLICT DO NOTHING)

-- Moderators
INSERT INTO "Users" ("Id", "Email", "HashedPassword", "Role", "CreatedAt", "Version")
VALUES
  ('a0000000-0000-0000-0000-000000000001', 'moderator@airbnb.com', 'Mod@123456', 'Moderator', NOW() - interval '90 days', 0),
  ('a0000000-0000-0000-0000-000000000002', 'sarah.mod@airbnb.com', 'Mod@123456', 'Moderator', NOW() - interval '60 days', 0)
ON CONFLICT ("Email") DO NOTHING;

INSERT INTO "UserProfiles" ("UserId", "FullName", "PhoneNumber", "AvatarUrl", "Bio")
VALUES
  ('a0000000-0000-0000-0000-000000000001', 'Tran Minh Duc', '+84901234567', NULL, 'Community moderator since 2025'),
  ('a0000000-0000-0000-0000-000000000002', 'Sarah Johnson', '+84987654321', NULL, 'Content review specialist')
ON CONFLICT ("UserId") DO NOTHING;

-- Hosts (8 hosts with various activity levels)
INSERT INTO "Users" ("Id", "Email", "HashedPassword", "Role", "CreatedAt", "Version")
VALUES
  ('b0000000-0000-0000-0000-000000000001', 'host.ngoc@gmail.com', 'Host@123456', 'User', NOW() - interval '365 days', 0),
  ('b0000000-0000-0000-0000-000000000002', 'host.linh@gmail.com', 'Host@123456', 'User', NOW() - interval '300 days', 0),
  ('b0000000-0000-0000-0000-000000000003', 'host.tuan@gmail.com', 'Host@123456', 'User', NOW() - interval '250 days', 0),
  ('b0000000-0000-0000-0000-000000000004', 'host.mai@gmail.com', 'Host@123456', 'User', NOW() - interval '200 days', 0),
  ('b0000000-0000-0000-0000-000000000005', 'host.khoa@gmail.com', 'Host@123456', 'User', NOW() - interval '180 days', 0),
  ('b0000000-0000-0000-0000-000000000006', 'host.ha@gmail.com', 'Host@123456', 'User', NOW() - interval '150 days', 0),
  ('b0000000-0000-0000-0000-000000000007', 'host.david@gmail.com', 'Host@123456', 'User', NOW() - interval '120 days', 0),
  ('b0000000-0000-0000-0000-000000000008', 'host.emma@gmail.com', 'Host@123456', 'User', NOW() - interval '90 days', 0)
ON CONFLICT ("Email") DO NOTHING;

INSERT INTO "UserProfiles" ("UserId", "FullName", "PhoneNumber", "AvatarUrl", "Bio")
VALUES
  ('b0000000-0000-0000-0000-000000000001', 'Nguyen Thi Ngoc', '+84911111111', NULL, 'Superhost with 5 years experience in Da Lat'),
  ('b0000000-0000-0000-0000-000000000002', 'Pham Thi Linh', '+84922222222', NULL, 'Interior designer turned host in HCMC'),
  ('b0000000-0000-0000-0000-000000000003', 'Le Van Tuan', '+84933333333', NULL, 'Beach resort owner in Nha Trang'),
  ('b0000000-0000-0000-0000-000000000004', 'Mai Anh Thu', '+84944444444', NULL, 'Heritage home host in Hoi An'),
  ('b0000000-0000-0000-0000-000000000005', 'Vo Thanh Khoa', '+84955555555', NULL, 'Mountain lodge operator in Sapa'),
  ('b0000000-0000-0000-0000-000000000006', 'Tran Thi Ha', '+84966666666', NULL, 'City apartment host in Hanoi'),
  ('b0000000-0000-0000-0000-000000000007', 'David Chen', '+84977777777', NULL, 'Digital nomad host in Da Nang'),
  ('b0000000-0000-0000-0000-000000000008', 'Emma Wilson', '+84988888888', NULL, 'Eco-resort host in Phu Quoc')
ON CONFLICT ("UserId") DO NOTHING;

-- Guests (10 guests)
INSERT INTO "Users" ("Id", "Email", "HashedPassword", "Role", "CreatedAt", "Version")
VALUES
  ('c0000000-0000-0000-0000-000000000001', 'guest.anh@gmail.com', 'Guest@123456', 'User', NOW() - interval '100 days', 0),
  ('c0000000-0000-0000-0000-000000000002', 'guest.binh@gmail.com', 'Guest@123456', 'User', NOW() - interval '90 days', 0),
  ('c0000000-0000-0000-0000-000000000003', 'guest.chi@gmail.com', 'Guest@123456', 'User', NOW() - interval '80 days', 0),
  ('c0000000-0000-0000-0000-000000000004', 'guest.dung@gmail.com', 'Guest@123456', 'User', NOW() - interval '70 days', 0),
  ('c0000000-0000-0000-0000-000000000005', 'guest.hoang@gmail.com', 'Guest@123456', 'User', NOW() - interval '60 days', 0),
  ('c0000000-0000-0000-0000-000000000006', 'guest.khanh@gmail.com', 'Guest@123456', 'User', NOW() - interval '50 days', 0),
  ('c0000000-0000-0000-0000-000000000007', 'guest.lam@gmail.com', 'Guest@123456', 'User', NOW() - interval '40 days', 0),
  ('c0000000-0000-0000-0000-000000000008', 'guest.nam@gmail.com', 'Guest@123456', 'User', NOW() - interval '30 days', 0),
  ('c0000000-0000-0000-0000-000000000009', 'guest.phuc@gmail.com', 'Guest@123456', 'User', NOW() - interval '20 days', 0),
  ('c0000000-0000-0000-0000-000000000010', 'guest.trang@gmail.com', 'Guest@123456', 'User', NOW() - interval '10 days', 0)
ON CONFLICT ("Email") DO NOTHING;

INSERT INTO "UserProfiles" ("UserId", "FullName", "PhoneNumber", "AvatarUrl", "Bio")
VALUES
  ('c0000000-0000-0000-0000-000000000001', 'Nguyen Van Anh', '+84911100001', NULL, 'Frequent traveler, loves beach resorts'),
  ('c0000000-0000-0000-0000-000000000002', 'Tran Binh', '+84911100002', NULL, 'Business traveler'),
  ('c0000000-0000-0000-0000-000000000003', 'Le Chi', '+84911100003', NULL, 'Backpacker and foodie'),
  ('c0000000-0000-0000-0000-000000000004', 'Pham Dung', '+84911100004', NULL, 'Family vacation planner'),
  ('c0000000-0000-0000-0000-000000000005', 'Hoang Minh', '+84911100005', NULL, 'Solo traveler, photographer'),
  ('c0000000-0000-0000-0000-000000000006', 'Khanh Ly', '+84911100006', NULL, 'Couple traveler with partner'),
  ('c0000000-0000-0000-0000-000000000007', 'Lam Son', '+84911100007', NULL, 'Adventure seeker'),
  ('c0000000-0000-0000-0000-000000000008', 'Nam Nhat', '+84911100008', NULL, 'Budget traveler'),
  ('c0000000-0000-0000-0000-000000000009', 'Phuc Nguyen', '+84911100009', NULL, 'Luxury traveler'),
  ('c0000000-0000-0000-0000-000000000010', 'Trang Thi', '+84911100010', NULL, 'Group trip organizer')
ON CONFLICT ("UserId") DO NOTHING;


-- ============================================================
-- PART 2: PROPDB — Properties, Images, Amenities, Availability, Reviews
-- ============================================================
-- Connect to: propdb

-- Properties (20 properties covering all statuses and types)
INSERT INTO "Properties" (
  "Id", "HostId", "Title", "Description", "Slug",
  "Latitude", "Longitude", "CountryCode", "Admin1Code", "Admin2Code",
  "DisplayAddress", "AddressRaw", "HouseRules",
  "Status", "BookingMode", "Type",
  "SuspensionReason", "RejectionReason",
  "ReviewCount", "AverageRating",
  "pricing_base_price", "pricing_cleaning_fee", "pricing_currency_code", "pricing_service_fee", "pricing_weekend_premium_percent",
  "capacity_guest_count", "capacity_bedroom_count", "capacity_bed_count", "capacity_bathroom_count",
  "CreatedAt", "UpdatedAt", "Version"
)
VALUES
  -- 1. Published Apartment in HCMC (popular, high rating)
  ('10000000-0000-0000-0000-000000000001', 'b0000000-0000-0000-0000-000000000002',
   'Modern Studio in District 1 Heart', 'Beautiful modern studio apartment right in the center of HCMC. Walking distance to Ben Thanh Market and Notre Dame Cathedral.', 'modern-studio-district-1-hcmc',
   10.7769, 106.7009, 'VN', 'SG', 'Quận 1',
   'District 1, Ho Chi Minh City', '{"StreetAddress":"123 Nguyen Hue Blvd","Unit":"Apt 12A","PostalCode":"70000","SubDivisions":{"City":"Ho Chi Minh","District":"District 1"},"Notes":{"Public":"Near Ben Thanh market","Internal":"Gate code: 1234"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["No shoes inside","Quiet after 10pm"]}',
   2, 'InstantBook', 1, NULL, NULL,
   24, 4.8,
   45.00, 10.00, 'USD', 5.00, 15,
   2, 1, 1, 1,
   NOW() - interval '200 days', NOW() - interval '5 days', 0),

  -- 2. Published Villa in Da Lat (premium)
  ('10000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000001',
   'Pine Hill Villa with Garden & Firepit', 'Escape to the highlands in this beautiful 3-bedroom villa surrounded by pine trees. Perfect for family retreats.', 'pine-hill-villa-da-lat',
   11.9404, 108.4583, 'VN', 'LD', 'Thành phố Đà Lạt',
   'Da Lat City, Lam Dong', '{"StreetAddress":"45 Tran Hung Dao St","PostalCode":"66100","SubDivisions":{"City":"Da Lat","District":"Ward 10"},"Notes":{"Public":"5 min to Da Lat Market","Internal":""}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"14:00:00","CheckOutTime":"12:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["No fireworks"]}',
   2, 'RequestToBook', 3, NULL, NULL,
   18, 4.9,
   120.00, 25.00, 'USD', 12.00, 20,
   8, 3, 3, 2,
   NOW() - interval '300 days', NOW() - interval '3 days', 0),

  -- 3. Pending Review - New listing waiting for approval
  ('10000000-0000-0000-0000-000000000003', 'b0000000-0000-0000-0000-000000000004',
   'Heritage Townhouse in Ancient Town', 'A lovingly restored 200-year-old townhouse in the heart of Hoi An Ancient Town. Traditional architecture with modern comforts.', 'heritage-townhouse-hoi-an',
   15.8801, 108.3380, 'VN', 'QN', 'Thành phố Hội An',
   'Hoi An Ancient Town, Quang Nam', '{"StreetAddress":"78 Nguyen Thai Hoc St","PostalCode":"51000","SubDivisions":{"City":"Hoi An","District":"Minh An"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["Remove shoes at entrance","No cooking after 9pm"]}',
   1, 'InstantBook', 2, NULL, NULL,
   0, 0,
   85.00, 15.00, 'USD', 8.00, 10,
   6, 2, 2, 2,
   NOW() - interval '2 days', NULL, 0),

  -- 4. Pending Review - Apartment in Hanoi
  ('10000000-0000-0000-0000-000000000004', 'b0000000-0000-0000-0000-000000000006',
   'Cozy Apartment near Old Quarter', 'Nicely decorated apartment just 5 minutes walk from Hoan Kiem Lake and the Old Quarter. Great for couples.', 'cozy-apartment-old-quarter-hanoi',
   21.0278, 105.8342, 'VN', 'HN', 'Quận Hoàn Kiếm',
   'Hoan Kiem District, Hanoi', '{"StreetAddress":"22 Hang Bac St","Unit":"3F","PostalCode":"10000","SubDivisions":{"City":"Ha Noi","District":"Hoan Kiem"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":[]}',
   1, 'RequestToBook', 1, NULL, NULL,
   0, 0,
   38.00, 8.00, 'USD', 4.00, 10,
   3, 1, 1, 1,
   NOW() - interval '1 day', NULL, 0),

  -- 5. Pending Review - Homestay in Sapa
  ('10000000-0000-0000-0000-000000000005', 'b0000000-0000-0000-0000-000000000005',
   'Mountain View Homestay with Terrace', 'Wake up to stunning mountain views in this authentic Hmong-style homestay. Terrace overlooks Muong Hoa valley.', 'mountain-view-homestay-sapa',
   22.3365, 103.8438, 'VN', 'LCH', 'Sa Pa',
   'Sapa Town, Lao Cai', '{"StreetAddress":"Cau May Road","PostalCode":"33000","SubDivisions":{"City":"Sa Pa","District":"Sa Pa"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"13:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["Respect local culture"]}',
   1, 'InstantBook', 4, NULL, NULL,
   0, 0,
   28.00, 5.00, 'USD', 3.00, 0,
   4, 2, 2, 1,
   NOW() - interval '5 hours', NULL, 0),

  -- 6. Rejected - Inappropriate content
  ('10000000-0000-0000-0000-000000000006', 'b0000000-0000-0000-0000-000000000007',
   'SUPER CHEAP PARTY HOUSE!!!', 'come party all night loud music allowed no rules just fun', 'super-cheap-party-house',
   10.7626, 106.6602, 'VN', 'SG', 'Quận Bình Thạnh',
   'Binh Thanh District, HCMC', '{"StreetAddress":"999 Unknown St"}',
   '{"AllowPets":true,"AllowSmoking":true,"AllowEvents":true,"CheckInTime":"00:00:00","CheckOutTime":"23:59:00","FlexibleCheckIn":true,"FlexibleCheckOut":true}',
   5, 'InstantBook', 1,
   NULL, 'Listing violates community standards: noisy party house with no proper house rules. Description is insufficient and misleading. Photos appear to be stock images not of actual property.',
   0, 0,
   15.00, 0.00, 'USD', 2.00, 0,
   10, 4, 4, 3,
   NOW() - interval '15 days', NOW() - interval '12 days', 0),

  -- 7. Rejected - Incomplete listing
  ('10000000-0000-0000-0000-000000000007', 'b0000000-0000-0000-0000-000000000008',
   'Beach Hut', 'nice place', 'beach-hut-phu-quoc',
   10.0, 104.0, 'VN', 'KG', 'Phú Quốc',
   'Phu Quoc Island', '{"StreetAddress":"Long Beach"}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   5, 'InstantBook', 4,
   NULL, 'Description is too short (minimum 50 characters). No amenities specified. Cover image is missing. Address information is incomplete.',
   0, 0,
   20.00, 0.00, 'USD', 2.00, 0,
   2, 1, 1, 1,
   NOW() - interval '20 days', NOW() - interval '18 days', 0),

  -- 8. Suspended - Safety violation
  ('10000000-0000-0000-0000-000000000008', 'b0000000-0000-0000-0000-000000000003',
   'Beachfront Resort in Nha Trang', 'Stunning beachfront resort with private access to the bay. 5 rooms with ocean view.', 'beachfront-resort-nha-trang',
   12.2388, 109.1416, 'VN', 'KH', 'Thành phố Nha Trang',
   'Nha Trang City, Khanh Hoa', '{"StreetAddress":"56 Tran Phu Blvd","PostalCode":"65000","SubDivisions":{"City":"Nha Trang","District":"Vinh Nguyen"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":[]}',
   3, 'InstantBook', 6,
   'Safety concerns reported by guests: unstable balcony railing on 2nd floor. Property suspended until structural inspection is completed and repairs are made.',
   NULL,
   8, 4.2,
   95.00, 20.00, 'USD', 10.00, 15,
   12, 5, 5, 4,
   NOW() - interval '180 days', NOW() - interval '7 days', 0),

  -- 9. Suspended - Legal dispute
  ('10000000-0000-0000-0000-000000000009', 'b0000000-0000-0000-0000-000000000001',
   'Da Lat Dream Cabin near Xuan Huong Lake', 'Charming wooden cabin with fireplace, walking distance to Xuan Huong Lake and Night Market.', 'da-lat-dream-cabin-xuan-huong',
   11.9454, 108.4414, 'VN', 'LD', 'Thành phố Đà Lạt',
   'Da Lat City Center, Lam Dong', '{"StreetAddress":"12 Bui Thi Xuan St","PostalCode":"66100","SubDivisions":{"City":"Da Lat","District":"Ward 1"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":false,"CustomRules":["No open fire except fireplace"]}',
   3, 'RequestToBook', 2,
   'Under investigation for ownership dispute. Host has been notified to provide documentation.',
   NULL,
   5, 4.5,
   65.00, 12.00, 'USD', 7.00, 10,
   4, 2, 2, 1,
   NOW() - interval '150 days', NOW() - interval '14 days', 0),

  -- 10. Draft - Host started but didn't finish
  ('10000000-0000-0000-0000-000000000010', 'b0000000-0000-0000-0000-000000000005',
   'Sapa Trekker Lodge', 'A comfortable lodge for trekkers exploring the rice terraces.', 'sapa-trekker-lodge',
   22.3406, 103.8490, 'VN', 'LCH', 'Sa Pa',
   'Sapa Town, Lao Cai', '{"StreetAddress":"Fanxipang Rd","PostalCode":"33000"}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   0, 'InstantBook', 4, NULL, NULL,
   0, 0,
   22.00, 5.00, 'USD', 2.00, 0,
   6, 3, 3, 2,
   NOW() - interval '45 days', NOW() - interval '40 days', 0),

  -- 11. Draft - Another incomplete
  ('10000000-0000-0000-0000-000000000011', 'b0000000-0000-0000-0000-000000000006',
   'Hanoi Old Quarter Apartment', 'Draft listing - still uploading photos', 'hanoi-old-quarter-apt-draft',
   21.0350, 105.8525, 'VN', 'HN', 'Quận Ba Đình',
   'Ba Dinh District, Hanoi', '{"StreetAddress":"TBD"}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   0, 'RequestToBook', 1, NULL, NULL,
   0, 0,
   30.00, 8.00, 'USD', 3.00, 5,
   2, 1, 1, 1,
   NOW() - interval '30 days', NOW() - interval '28 days', 0),

  -- 12. Published House in Hoi An
  ('10000000-0000-0000-0000-000000000012', 'b0000000-0000-0000-0000-000000000004',
   'Riverside House with Private Garden', 'Traditional Vietnamese house with private garden, right on the Thu Bon River. Watch boats pass from your porch.', 'riverside-house-hoi-an',
   15.8770, 108.3350, 'VN', 'QN', 'Thành phố Hội An',
   'Cam An Ward, Hoi An', '{"StreetAddress":"15 An Bang Beach Rd","PostalCode":"51000","SubDivisions":{"City":"Hoi An","District":"Cam An"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"15:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["Respect neighbors","No motorbike in garden"]}',
   2, 'InstantBook', 2, NULL, NULL,
   12, 4.7,
   72.00, 15.00, 'USD', 7.00, 10,
   5, 2, 2, 2,
   NOW() - interval '250 days', NOW() - interval '2 days', 0),

  -- 13. Published Homestay in Mekong Delta
  ('10000000-0000-0000-0000-000000000013', 'b0000000-0000-0000-0000-000000000001',
   'Mekong Delta Farmhouse Experience', 'Live like a local on a working farm in the Mekong Delta. Bike through fruit orchards and cook traditional dishes.', 'mekong-delta-farmhouse',
   10.2500, 105.9700, 'VN', 'AG', 'Châu Thành',
   'Chau Thanh, An Giang', '{"StreetAddress":"Locals Only Rd","PostalCode":"88000","SubDivisions":{"Province":"An Giang","District":"Chau Thanh"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"12:00:00","CheckOutTime":"10:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["Join family breakfast","Help with garden optional"]}',
   2, 'InstantBook', 4, NULL, NULL,
   9, 4.6,
   25.00, 0.00, 'USD', 3.00, 0,
   4, 2, 2, 1,
   NOW() - interval '220 days', NOW() - interval '10 days', 0),

  -- 14. Published Hotel in Da Nang
  ('10000000-0000-0000-0000-000000000014', 'b0000000-0000-0000-0000-000000000007',
   'My Khe Beach Hotel Suite', 'Premium suite in a boutique hotel right on My Khe Beach. Infinity pool, spa, and rooftop bar included.', 'my-khe-beach-hotel-suite',
   16.0544, 108.2422, 'VN', 'DN', 'Quận Sơn Trà',
   'Son Tra District, Da Nang', '{"StreetAddress":"200 Vo Nguyen Giap St","PostalCode":"55000","SubDivisions":{"City":"Da Nang","District":"Son Tra"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"12:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   2, 'InstantBook', 5, NULL, NULL,
   31, 4.4,
   88.00, 0.00, 'USD', 9.00, 25,
   2, 1, 1, 1,
   NOW() - interval '160 days', NOW() - interval '1 day', 0),

  -- 15. Published Resort in Phu Quoc
  ('10000000-0000-0000-0000-000000000015', 'b0000000-0000-0000-0000-000000000008',
   'Sunset Cove Eco Resort', 'Sustainable luxury resort on the west coast of Phu Quoc. Watch spectacular sunsets from your private deck.', 'sunset-cove-eco-resort-phu-quoc',
   10.1900, 103.9600, 'VN', 'KG', 'Phú Quốc',
   'Duong To, Phu Quoc Island', '{"StreetAddress":"Long Beach Rd Km 12","PostalCode":"92000","SubDivisions":{"Island":"Phu Quoc","Commune":"Duong To"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["Eco-friendly products only","No single-use plastic"]}',
   2, 'RequestToBook', 6, NULL, NULL,
   42, 4.9,
   180.00, 30.00, 'USD', 18.00, 30,
   2, 1, 1, 1,
   NOW() - interval '120 days', NOW() - interval '4 days', 0),

  -- 16. Archived - Host no longer active
  ('10000000-0000-0000-0000-000000000016', 'b0000000-0000-0000-0000-000000000003',
   'City Center Apartment Nha Trang', 'Convenient apartment near Nha Trang center. Walking distance to night market.', 'city-center-apartment-nha-trang',
   12.2541, 109.1893, 'VN', 'KH', 'Thành phố Nha Trang',
   'Nha Trang City, Khanh Hoa', '{"StreetAddress":"22 Yet Kieu St","PostalCode":"65000"}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   4, 'InstantBook', 1, NULL, NULL,
   3, 3.8,
   32.00, 8.00, 'USD', 3.00, 5,
   3, 1, 1, 1,
   NOW() - interval '400 days', NOW() - interval '60 days', 0),

  -- 17. Published Apartment (new host, first listing)
  ('10000000-0000-0000-0000-000000000017', 'b0000000-0000-0000-0000-000000000007',
   'Charming Studio with Balcony in Da Nang', 'Bright and airy studio with a balcony overlooking the Han River. Perfect for digital nomads.', 'charming-studio-balcony-da-nang',
   16.0678, 108.2208, 'VN', 'DN', 'Quận Hải Châu',
   'Hai Chau District, Da Nang', '{"StreetAddress":"88 Bach Dang St","Unit":"5F","PostalCode":"55000","SubDivisions":{"City":"Da Nang","District":"Hai Chau"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["No cooking strong-smelling food"]}',
   2, 'InstantBook', 1, NULL, NULL,
   2, 4.0,
   35.00, 8.00, 'USD', 4.00, 10,
   2, 1, 1, 1,
   NOW() - interval '60 days', NOW() - interval '6 days', 0),

  -- 18. Pending Review - Villa in Vung Tau
  ('10000000-0000-0000-0000-000000000018', 'b0000000-0000-0000-0000-000000000002',
   'Ocean View Villa in Vung Tau', 'Brand new 4-bedroom villa with panoramic ocean views. Private pool and rooftop BBQ area.', 'ocean-view-villa-vung-tau',
   10.3450, 107.0850, 'VN', 'BR', 'Thành phố Vũng Tàu',
   'Vung Tau City, Ba Ria-Vung Tau', '{"StreetAddress":"30 Thuy Van St","PostalCode":"79000","SubDivisions":{"City":"Vung Tau","District":"Ward 2"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"15:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["Pool hours 7am-9pm"]}',
   1, 'InstantBook', 3, NULL, NULL,
   0, 0,
   150.00, 35.00, 'USD', 15.00, 20,
   10, 4, 4, 3,
   NOW() - interval '10 hours', NULL, 0),

  -- 19. Published Homestay in Hue
  ('10000000-0000-0000-0000-000000000019', 'b0000000-0000-0000-0000-000000000004',
   'Imperial Garden Homestay Hue', 'Traditional Vietnamese courtyard house near the Imperial City. Daily breakfast included.', 'imperial-garden-homestay-hue',
   16.4637, 107.5900, 'VN', 'TTH', 'Thành phố Huế',
   'Phu Hoi Ward, Hue City', '{"StreetAddress":"55 Pham Ngu Lao St","PostalCode":"53000","SubDivisions":{"City":"Hue","District":"Phu Hoi"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"13:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["Remove shoes indoors","Quiet hours 10pm-6am"]}',
   2, 'InstantBook', 4, NULL, NULL,
   15, 4.7,
   32.00, 5.00, 'USD', 3.00, 0,
   6, 3, 3, 2,
   NOW() - interval '190 days', NOW() - interval '8 days', 0),

  -- 20. Published Apartment - budget in HCMC
  ('10000000-0000-0000-0000-000000000020', 'b0000000-0000-0000-0000-000000000006',
   'Budget Friendly Room in District 7', 'Clean and comfortable private room in a shared house. Great for solo travelers on a budget. Kitchen access included.', 'budget-room-district-7-hcmc',
   10.7327, 106.7196, 'VN', 'SG', 'Quận 7',
   'District 7, Ho Chi Minh City', '{"StreetAddress":"18 Nguyen Huu Tho St","Unit":"Room B","PostalCode":"70000","SubDivisions":{"City":"Ho Chi Minh","District":"District 7"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["No guests after 10pm","Clean kitchen after use"]}',
   2, 'InstantBook', 1, NULL, NULL,
   7, 3.9,
   18.00, 3.00, 'USD', 2.00, 0,
   1, 1, 1, 1,
   NOW() - interval '100 days', NOW() - interval '12 days', 0)
ON CONFLICT ("Slug") DO NOTHING;


-- Property Images (cover images for published/suspended properties)
INSERT INTO "PropertyImages" ("Id", "PropertyId", "PublicId", "Url", "Type", "DisplayOrder", "UploadedBy")
VALUES
  ('20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'seed/hcmc-studio', 'https://picsum.photos/seed/hcmc-studio/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000002'),
  ('20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'seed/dalat-villa', 'https://picsum.photos/seed/dalat-villa/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000001'),
  ('20000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000008', 'seed/nhatrang-resort', 'https://picsum.photos/seed/nhatrang-resort/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000003'),
  ('20000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000009', 'seed/dalat-cabin', 'https://picsum.photos/seed/dalat-cabin/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000001'),
  ('20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000012', 'seed/hoian-house', 'https://picsum.photos/seed/hoian-house/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000004'),
  ('20000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000013', 'seed/mekong-farm', 'https://picsum.photos/seed/mekong-farm/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000001'),
  ('20000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000014', 'seed/danang-hotel', 'https://picsum.photos/seed/danang-hotel/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000007'),
  ('20000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000015', 'seed/phuquoc-resort', 'https://picsum.photos/seed/phuquoc-resort/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000008'),
  ('20000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000017', 'seed/danang-studio', 'https://picsum.photos/seed/danang-studio/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000007'),
  ('20000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000019', 'seed/hue-homestay', 'https://picsum.photos/seed/hue-homestay/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000004'),
  ('20000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000020', 'seed/hcmc-budget', 'https://picsum.photos/seed/hcmc-budget/800/600', 'Cover', 0, 'b0000000-0000-0000-0000-000000000006')
ON CONFLICT DO NOTHING;


-- Property Amenities (link properties to existing amenity seed data)
INSERT INTO "PropertyAmenities" ("PropertyId", "AmenityId")
VALUES
  -- HCMC Studio: WiFi, TV, Kitchen, AC, Washing Machine
  ('10000000-0000-0000-0000-000000000001', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000001', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000001', 'c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1'),
  ('10000000-0000-0000-0000-000000000001', 'e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1'),
  ('10000000-0000-0000-0000-000000000001', 'd4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4'),
  -- Da Lat Villa: WiFi, TV, Kitchen, Pool, Parking, Heating, Fireplace
  ('10000000-0000-0000-0000-000000000002', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000002', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000002', 'c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1'),
  ('10000000-0000-0000-0000-000000000002', 'd2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2'),
  ('10000000-0000-0000-0000-000000000002', 'd1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1'),
  ('10000000-0000-0000-0000-000000000002', 'e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2'),
  -- Nha Trang Resort: WiFi, TV, Pool, Parking, Gym, AC
  ('10000000-0000-0000-0000-000000000008', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000008', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000008', 'd2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2'),
  ('10000000-0000-0000-0000-000000000008', 'd1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1'),
  ('10000000-0000-0000-0000-000000000008', 'd3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3'),
  ('10000000-0000-0000-0000-000000000008', 'e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1')
ON CONFLICT DO NOTHING;


-- Reviews (for published properties with rating > 0)
INSERT INTO "Reviews" ("Id", "PropertyId", "BookingId", "GuestId", "Rating", "Comment", "CreatedAt")
VALUES
  -- Da Lat Villa reviews
  ('30000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000001', 5, 'Absolutely magical! The villa is even more beautiful than the photos. Waking up to pine-scented air was incredible.', NOW() - interval '30 days'),
  ('30000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000003', 5, 'Perfect family retreat. Kids loved the garden and firepit. Host was incredibly thoughtful.', NOW() - interval '20 days'),
  ('30000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000003', 'c0000000-0000-0000-0000-000000000005', 5, 'One of the best stays we have ever had. The firepit under the stars was unforgettable.', NOW() - interval '10 days'),
  -- HCMC Studio reviews
  ('30000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000004', 'c0000000-0000-0000-0000-000000000002', 5, 'Perfect location and very clean. Walking distance to everything in District 1.', NOW() - interval '25 days'),
  ('30000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000005', 'c0000000-0000-0000-0000-000000000004', 4, 'Great studio, well equipped. A bit noisy at night but expected for D1 location.', NOW() - interval '15 days'),
  ('30000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000006', 'c0000000-0000-0000-0000-000000000006', 5, 'Excellent value! The host was super responsive and helpful.', NOW() - interval '5 days'),
  -- Hoi An House reviews
  ('30000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000012', '40000000-0000-0000-0000-000000000007', 'c0000000-0000-0000-0000-000000000007', 5, 'Such a peaceful spot by the river. The garden is gorgeous and the house is full of character.', NOW() - interval '35 days'),
  ('30000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000012', '40000000-0000-0000-0000-000000000008', 'c0000000-0000-0000-0000-000000000009', 4, 'Lovely traditional house. The porch by the river is the highlight. Would stay again!', NOW() - interval '8 days'),
  -- Nha Trang Resort review (suspended but had reviews before)
  ('30000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000008', '40000000-0000-0000-0000-000000000009', 'c0000000-0000-0000-0000-000000000008', 4, 'Great location and views. Pool was nice. Some maintenance issues though.', NOW() - interval '40 days'),
  -- Mekong Farm reviews
  ('30000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000013', '40000000-0000-0000-0000-000000000010', 'c0000000-0000-0000-0000-000000000001', 5, 'An authentic experience! The host family was so welcoming. Cooking class was a highlight.', NOW() - interval '22 days'),
  -- Da Nang Hotel review
  ('30000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000014', '40000000-0000-0000-0000-000000000011', 'c0000000-0000-0000-0000-000000000010', 4, 'Nice hotel suite with great beach access. Infinity pool is amazing.', NOW() - interval '12 days'),
  -- Phu Quoc Resort review
  ('30000000-0000-0000-0000-000000000012', '10000000-0000-0000-0000-000000000015', '40000000-0000-0000-0000-000000000012', 'c0000000-0000-0000-0000-000000000009', 5, 'Luxury eco-resort done right. Sunsets are unreal. Worth every penny.', NOW() - interval '18 days'),
  -- Hue Homestay review
  ('30000000-0000-0000-0000-000000000013', '10000000-0000-0000-0000-000000000019', '40000000-0000-0000-0000-000000000013', 'c0000000-0000-0000-0000-000000000003', 5, 'Beautiful courtyard house near the Imperial City. Breakfast was delicious and abundant.', NOW() - interval '28 days'),
  -- Budget HCMC review
  ('30000000-0000-0000-0000-000000000014', '10000000-0000-0000-0000-000000000020', '40000000-0000-0000-0000-000000000014', 'c0000000-0000-0000-0000-000000000008', 4, 'Great budget option. Clean and the host was helpful. Kitchen access is a big plus.', NOW() - interval '7 days')
ON CONFLICT ("BookingId") DO NOTHING;


-- ============================================================
-- PART 3: BOOKDB — Bookings
-- ============================================================
-- Connect to: bookdb

INSERT INTO "Bookings" (
  "Id", "PropertyId", "HostId", "GuestId", "CountryCode",
  "CheckIn", "CheckOut", "GuestCount", "NightCount",
  "BasePricePerNight", "CleaningFee", "ServiceFee", "TaxAmount", "TotalPrice", "CurrencyCode",
  "Status", "CancelledBy", "CreatedAt", "Version"
)
VALUES
  -- Confirmed bookings (past - already completed)
  ('40000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000001', 'VN',
   CURRENT_DATE - interval '35 days', CURRENT_DATE - interval '32 days', 4, 3,
   120.00, 25.00, 12.00, 14.76, 171.76, 'USD',
   'Confirmed', NULL, NOW() - interval '45 days', 0),

  ('40000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000003', 'VN',
   CURRENT_DATE - interval '25 days', CURRENT_DATE - interval '22 days', 6, 3,
   120.00, 25.00, 12.00, 14.76, 171.76, 'USD',
   'Confirmed', NULL, NOW() - interval '35 days', 0),

  ('40000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000005', 'VN',
   CURRENT_DATE - interval '15 days', CURRENT_DATE - interval '12 days', 5, 3,
   120.00, 25.00, 12.00, 14.76, 171.76, 'USD',
   'Confirmed', NULL, NOW() - interval '25 days', 0),

  -- HCMC Studio confirmed
  ('40000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001', 'b0000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000002', 'VN',
   CURRENT_DATE - interval '30 days', CURRENT_DATE - interval '28 days', 2, 2,
   45.00, 10.00, 5.00, 4.80, 64.80, 'USD',
   'Confirmed', NULL, NOW() - interval '40 days', 0),

  ('40000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', 'b0000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000004', 'VN',
   CURRENT_DATE - interval '20 days', CURRENT_DATE - interval '18 days', 2, 2,
   45.00, 10.00, 5.00, 4.80, 64.80, 'USD',
   'Confirmed', NULL, NOW() - interval '30 days', 0),

  -- Active confirmed booking (current stay)
  ('40000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000001', 'b0000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000006', 'VN',
   CURRENT_DATE - interval '1 day', CURRENT_DATE + interval '2 days', 2, 3,
   45.00, 10.00, 5.00, 4.80, 64.80, 'USD',
   'Confirmed', NULL, NOW() - interval '7 days', 0),

  -- Pending bookings (awaiting approval)
  ('40000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000012', 'b0000000-0000-0000-0000-000000000004', 'c0000000-0000-0000-0000-000000000007', 'VN',
   CURRENT_DATE + interval '7 days', CURRENT_DATE + interval '10 days', 4, 3,
   72.00, 15.00, 7.00, 7.14, 101.14, 'USD',
   'AwaitingApproval', NULL, NOW() - interval '2 days', 0),

  -- More confirmed past bookings for reviews
  ('40000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000012', 'b0000000-0000-0000-0000-000000000004', 'c0000000-0000-0000-0000-000000000009', 'VN',
   CURRENT_DATE - interval '12 days', CURRENT_DATE - interval '9 days', 3, 3,
   72.00, 15.00, 7.00, 7.14, 101.14, 'USD',
   'Confirmed', NULL, NOW() - interval '20 days', 0),

  ('40000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000008', 'b0000000-0000-0000-0000-000000000003', 'c0000000-0000-0000-0000-000000000008', 'VN',
   CURRENT_DATE - interval '50 days', CURRENT_DATE - interval '47 days', 4, 3,
   95.00, 20.00, 10.00, 9.45, 134.45, 'USD',
   'Confirmed', NULL, NOW() - interval '60 days', 0),

  ('40000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000013', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000001', 'VN',
   CURRENT_DATE - interval '28 days', CURRENT_DATE - interval '26 days', 2, 2,
   25.00, 0.00, 3.00, 2.12, 30.12, 'USD',
   'Confirmed', NULL, NOW() - interval '38 days', 0),

  ('40000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000014', 'b0000000-0000-0000-0000-000000000007', 'c0000000-0000-0000-0000-000000000010', 'VN',
   CURRENT_DATE - interval '18 days', CURRENT_DATE - interval '16 days', 2, 2,
   88.00, 0.00, 9.00, 7.48, 104.48, 'USD',
   'Confirmed', NULL, NOW() - interval '28 days', 0),

  ('40000000-0000-0000-0000-000000000012', '10000000-0000-0000-0000-000000000015', 'b0000000-0000-0000-0000-000000000008', 'c0000000-0000-0000-0000-000000000009', 'VN',
   CURRENT_DATE - interval '25 days', CURRENT_DATE - interval '22 days', 2, 3,
   180.00, 30.00, 18.00, 17.28, 245.28, 'USD',
   'Confirmed', NULL, NOW() - interval '35 days', 0),

  ('40000000-0000-0000-0000-000000000013', '10000000-0000-0000-0000-000000000019', 'b0000000-0000-0000-0000-000000000004', 'c0000000-0000-0000-0000-000000000003', 'VN',
   CURRENT_DATE - interval '35 days', CURRENT_DATE - interval '32 days', 3, 3,
   32.00, 5.00, 3.00, 3.00, 43.00, 'USD',
   'Confirmed', NULL, NOW() - interval '45 days', 0),

  ('40000000-0000-0000-0000-000000000014', '10000000-0000-0000-0000-000000000020', 'b0000000-0000-0000-0000-000000000006', 'c0000000-0000-0000-0000-000000000008', 'VN',
   CURRENT_DATE - interval '10 days', CURRENT_DATE - interval '8 days', 1, 2,
   18.00, 3.00, 2.00, 1.74, 24.74, 'USD',
   'Confirmed', NULL, NOW() - interval '18 days', 0),

  -- Cancelled bookings
  ('40000000-0000-0000-0000-000000000015', '10000000-0000-0000-0000-000000000002', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000004', 'VN',
   CURRENT_DATE + interval '5 days', CURRENT_DATE + interval '8 days', 3, 3,
   120.00, 25.00, 12.00, 14.76, 171.76, 'USD',
   'Cancelled', 'c0000000-0000-0000-0000-000000000004', NOW() - interval '10 days', 0),

  ('40000000-0000-0000-0000-000000000016', '10000000-0000-0000-0000-000000000001', 'b0000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000007', 'VN',
   CURRENT_DATE + interval '3 days', CURRENT_DATE + interval '5 days', 2, 2,
   45.00, 10.00, 5.00, 4.80, 64.80, 'USD',
   'Cancelled', 'b0000000-0000-0000-0000-000000000002', NOW() - interval '5 days', 0),

  -- New pending booking
  ('40000000-0000-0000-0000-000000000017', '10000000-0000-0000-0000-000000000017', 'b0000000-0000-0000-0000-000000000007', 'c0000000-0000-0000-0000-000000000002', 'VN',
   CURRENT_DATE + interval '10 days', CURRENT_DATE + interval '13 days', 2, 3,
   35.00, 8.00, 4.00, 3.54, 50.54, 'USD',
   'Pending', NULL, NOW() - interval '1 day', 0),

  -- Awaiting approval for Da Lat Cabin
  ('40000000-0000-0000-0000-000000000018', '10000000-0000-0000-0000-000000000009', 'b0000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000005', 'VN',
   CURRENT_DATE + interval '5 days', CURRENT_DATE + interval '8 days', 4, 3,
   65.00, 12.00, 7.00, 6.36, 90.36, 'USD',
   'AwaitingApproval', NULL, NOW() - interval '3 days', 0)
ON CONFLICT ("Id") DO NOTHING;


-- ============================================================
-- PART 4: PAYDB — Payments
-- ============================================================
-- Connect to: paydb

INSERT INTO "Payments" ("Id", "BookingId", "Amount", "Currency", "Status", "TransactionId", "PaymentUrl", "ExpiresAt", "CreatedAt", "Version")
VALUES
  -- Successful payments for confirmed bookings
  ('50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 171.76, 'USD', 'Success', 'TXN_001_HCMC', NULL, NULL, NOW() - interval '45 days', 0),
  ('50000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000002', 171.76, 'USD', 'Success', 'TXN_002_DALAT', NULL, NULL, NOW() - interval '35 days', 0),
  ('50000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000003', 171.76, 'USD', 'Success', 'TXN_003_DALAT2', NULL, NULL, NOW() - interval '25 days', 0),
  ('50000000-0000-0000-0000-000000000004', '40000000-0000-0000-0000-000000000004', 64.80, 'USD', 'Success', 'TXN_004_STUDIO', NULL, NULL, NOW() - interval '40 days', 0),
  ('50000000-0000-0000-0000-000000000005', '40000000-0000-0000-0000-000000000005', 64.80, 'USD', 'Success', 'TXN_005_STUDIO2', NULL, NULL, NOW() - interval '30 days', 0),
  ('50000000-0000-0000-0000-000000000006', '40000000-0000-0000-0000-000000000006', 64.80, 'USD', 'Success', 'TXN_006_ACTIVE', NULL, NULL, NOW() - interval '7 days', 0),
  ('50000000-0000-0000-0000-000000000007', '40000000-0000-0000-0000-000000000009', 134.45, 'USD', 'Success', 'TXN_009_RESORT', NULL, NULL, NOW() - interval '60 days', 0),
  ('50000000-0000-0000-0000-000000000008', '40000000-0000-0000-0000-000000000010', 30.12, 'USD', 'Success', 'TXN_010_FARM', NULL, NULL, NOW() - interval '38 days', 0),
  ('50000000-0000-0000-0000-000000000009', '40000000-0000-0000-0000-000000000011', 104.48, 'USD', 'Success', 'TXN_011_HOTEL', NULL, NULL, NOW() - interval '28 days', 0),
  ('50000000-0000-0000-0000-000000000010', '40000000-0000-0000-0000-000000000012', 245.28, 'USD', 'Success', 'TXN_012_ECORESORT', NULL, NULL, NOW() - interval '35 days', 0),
  ('50000000-0000-0000-0000-000000000011', '40000000-0000-0000-0000-000000000013', 43.00, 'USD', 'Success', 'TXN_013_HUE', NULL, NULL, NOW() - interval '45 days', 0),
  ('50000000-0000-0000-0000-000000000012', '40000000-0000-0000-0000-000000000014', 24.74, 'USD', 'Success', 'TXN_014_BUDGET', NULL, NULL, NOW() - interval '18 days', 0),
  -- Pending payment for awaiting approval booking
  ('50000000-0000-0000-0000-000000000013', '40000000-0000-0000-0000-000000000007', 101.14, 'USD', 'Pending', NULL, 'https://pay.example.com/107', NOW() + interval '1 day', NOW() - interval '2 days', 0),
  -- Failed payment
  ('50000000-0000-0000-0000-000000000014', '40000000-0000-0000-0000-000000000017', 50.54, 'USD', 'Failed', NULL, NULL, NULL, NOW() - interval '1 day', 0),
  -- Expired payment (cancelled booking)
  ('50000000-0000-0000-0000-000000000015', '40000000-0000-0000-0000-000000000015', 171.76, 'USD', 'Expired', NULL, NULL, NOW() - interval '8 days', NOW() - interval '10 days', 0),
  -- Hoi An house past confirmed
  ('50000000-0000-0000-0000-000000000016', '40000000-0000-0000-0000-000000000008', 101.14, 'USD', 'Success', 'TXN_008_HOIAN', NULL, NULL, NOW() - interval '20 days', 0)
ON CONFLICT ("Id") DO NOTHING;
