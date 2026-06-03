-- Properties (20 properties covering all statuses and types)
-- Table name is snake_case: properties, property_images, property_amenities, reviews
-- Column names are PascalCase

INSERT INTO "properties" (
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
   10.7769, 106.7009, 'VN', 'SG', 'Quan 1',
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
   11.9404, 108.4583, 'VN', 'LD', 'Thanh pho Da Lat',
   'Da Lat City, Lam Dong', '{"StreetAddress":"45 Tran Hung Dao St","PostalCode":"66100","SubDivisions":{"City":"Da Lat","District":"Ward 10"},"Notes":{"Public":"5 min to Da Lat Market","Internal":""}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":true,"CheckInTime":"14:00:00","CheckOutTime":"12:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["No fireworks"]}',
   2, 'RequestToBook', 3, NULL, NULL,
   18, 4.9,
   120.00, 25.00, 'USD', 12.00, 20,
   8, 3, 3, 2,
   NOW() - interval '300 days', NOW() - interval '3 days', 0),

  -- 3. Pending Review - Hoi An heritage
  ('10000000-0000-0000-0000-000000000003', 'b0000000-0000-0000-0000-000000000004',
   'Heritage Townhouse in Ancient Town', 'A lovingly restored 200-year-old townhouse in the heart of Hoi An Ancient Town. Traditional architecture with modern comforts.', 'heritage-townhouse-hoi-an',
   15.8801, 108.3380, 'VN', 'QN', 'Thanh pho Hoi An',
   'Hoi An Ancient Town, Quang Nam', '{"StreetAddress":"78 Nguyen Thai Hoc St","PostalCode":"51000","SubDivisions":{"City":"Hoi An","District":"Minh An"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["Remove shoes at entrance","No cooking after 9pm"]}',
   1, 'InstantBook', 2, NULL, NULL,
   0, 0,
   85.00, 15.00, 'USD', 8.00, 10,
   6, 2, 2, 2,
   NOW() - interval '2 days', NULL, 0),

  -- 4. Pending Review - Hanoi apartment
  ('10000000-0000-0000-0000-000000000004', 'b0000000-0000-0000-0000-000000000006',
   'Cozy Apartment near Old Quarter', 'Nicely decorated apartment just 5 minutes walk from Hoan Kiem Lake and the Old Quarter. Great for couples.', 'cozy-apartment-old-quarter-hanoi',
   21.0278, 105.8342, 'VN', 'HN', 'Quan Hoan Kiem',
   'Hoan Kiem District, Hanoi', '{"StreetAddress":"22 Hang Bac St","Unit":"3F","PostalCode":"10000","SubDivisions":{"City":"Ha Noi","District":"Hoan Kiem"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":[]}',
   1, 'RequestToBook', 1, NULL, NULL,
   0, 0,
   38.00, 8.00, 'USD', 4.00, 10,
   3, 1, 1, 1,
   NOW() - interval '1 day', NULL, 0),

  -- 5. Pending Review - Sapa homestay
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
   10.7626, 106.6602, 'VN', 'SG', 'Quan Binh Thanh',
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
   10.0, 104.0, 'VN', 'KG', 'Phu Quoc',
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
   12.2388, 109.1416, 'VN', 'KH', 'Thanh pho Nha Trang',
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
   11.9454, 108.4414, 'VN', 'LD', 'Thanh pho Da Lat',
   'Da Lat City Center, Lam Dong', '{"StreetAddress":"12 Bui Thi Xuan St","PostalCode":"66100","SubDivisions":{"City":"Da Lat","District":"Ward 1"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":false,"CustomRules":["No open fire except fireplace"]}',
   3, 'RequestToBook', 2,
   'Under investigation for ownership dispute. Host has been notified to provide documentation.',
   NULL,
   5, 4.5,
   65.00, 12.00, 'USD', 7.00, 10,
   4, 2, 2, 1,
   NOW() - interval '150 days', NOW() - interval '14 days', 0),

  -- 10. Draft - incomplete
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

  -- 11. Draft - another incomplete
  ('10000000-0000-0000-0000-000000000011', 'b0000000-0000-0000-0000-000000000006',
   'Hanoi Old Quarter Apartment', 'Draft listing - still uploading photos', 'hanoi-old-quarter-apt-draft',
   21.0350, 105.8525, 'VN', 'HN', 'Quan Ba Dinh',
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
   15.8770, 108.3350, 'VN', 'QN', 'Thanh pho Hoi An',
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
   10.2500, 105.9700, 'VN', 'AG', 'Chau Thanh',
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
   16.0544, 108.2422, 'VN', 'DN', 'Quan Son Tra',
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
   10.1900, 103.9600, 'VN', 'KG', 'Phu Quoc',
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
   12.2541, 109.1893, 'VN', 'KH', 'Thanh pho Nha Trang',
   'Nha Trang City, Khanh Hoa', '{"StreetAddress":"22 Yet Kieu St","PostalCode":"65000"}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false}',
   4, 'InstantBook', 1, NULL, NULL,
   3, 3.8,
   32.00, 8.00, 'USD', 3.00, 5,
   3, 1, 1, 1,
   NOW() - interval '400 days', NOW() - interval '60 days', 0),

  -- 17. Published Apartment Da Nang (new host)
  ('10000000-0000-0000-0000-000000000017', 'b0000000-0000-0000-0000-000000000007',
   'Charming Studio with Balcony in Da Nang', 'Bright and airy studio with a balcony overlooking the Han River. Perfect for digital nomads.', 'charming-studio-balcony-da-nang',
   16.0678, 108.2208, 'VN', 'DN', 'Quan Hai Chau',
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
   10.3450, 107.0850, 'VN', 'BR', 'Thanh pho Vung Tau',
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
   16.4637, 107.5900, 'VN', 'TTH', 'Thanh pho Hue',
   'Phu Hoi Ward, Hue City', '{"StreetAddress":"55 Pham Ngu Lao St","PostalCode":"53000","SubDivisions":{"City":"Hue","District":"Phu Hoi"}}',
   '{"AllowPets":true,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"13:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":true,"FlexibleCheckOut":true,"CustomRules":["Remove shoes indoors","Quiet hours 10pm-6am"]}',
   2, 'InstantBook', 4, NULL, NULL,
   15, 4.7,
   32.00, 5.00, 'USD', 3.00, 0,
   6, 3, 3, 2,
   NOW() - interval '190 days', NOW() - interval '8 days', 0),

  -- 20. Published budget apartment HCMC
  ('10000000-0000-0000-0000-000000000020', 'b0000000-0000-0000-0000-000000000006',
   'Budget Friendly Room in District 7', 'Clean and comfortable private room in a shared house. Great for solo travelers on a budget. Kitchen access included.', 'budget-room-district-7-hcmc',
   10.7327, 106.7196, 'VN', 'SG', 'Quan 7',
   'District 7, Ho Chi Minh City', '{"StreetAddress":"18 Nguyen Huu Tho St","Unit":"Room B","PostalCode":"70000","SubDivisions":{"City":"Ho Chi Minh","District":"District 7"}}',
   '{"AllowPets":false,"AllowSmoking":false,"AllowEvents":false,"CheckInTime":"14:00:00","CheckOutTime":"11:00:00","FlexibleCheckIn":false,"FlexibleCheckOut":false,"CustomRules":["No guests after 10pm","Clean kitchen after use"]}',
   2, 'InstantBook', 1, NULL, NULL,
   7, 3.9,
   18.00, 3.00, 'USD', 2.00, 0,
   1, 1, 1, 1,
   NOW() - interval '100 days', NOW() - interval '12 days', 0)
ON CONFLICT ("Slug") DO NOTHING;

-- Property Images
INSERT INTO "property_images" ("Id", "PropertyId", "PublicId", "Url", "Type", "DisplayOrder", "UploadedBy")
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

-- Property Amenities
INSERT INTO "property_amenities" ("PropertyId", "AmenityId")
VALUES
  ('10000000-0000-0000-0000-000000000001', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000001', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000001', 'c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1'),
  ('10000000-0000-0000-0000-000000000001', 'e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1'),
  ('10000000-0000-0000-0000-000000000001', 'd4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4'),
  ('10000000-0000-0000-0000-000000000002', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000002', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000002', 'c1c1c1c1-c1c1-c1c1-c1c1-c1c1c1c1c1c1'),
  ('10000000-0000-0000-0000-000000000002', 'd2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2'),
  ('10000000-0000-0000-0000-000000000002', 'd1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1'),
  ('10000000-0000-0000-0000-000000000002', 'e2e2e2e2-e2e2-e2e2-e2e2-e2e2e2e2e2e2'),
  ('10000000-0000-0000-0000-000000000008', 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1'),
  ('10000000-0000-0000-0000-000000000008', 'b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1'),
  ('10000000-0000-0000-0000-000000000008', 'd2d2d2d2-d2d2-d2d2-d2d2-d2d2d2d2d2d2'),
  ('10000000-0000-0000-0000-000000000008', 'd1d1d1d1-d1d1-d1d1-d1d1-d1d1d1d1d1d1'),
  ('10000000-0000-0000-0000-000000000008', 'd3d3d3d3-d3d3-d3d3-d3d3-d3d3d3d3d3d3'),
  ('10000000-0000-0000-0000-000000000008', 'e1e1e1e1-e1e1-e1e1-e1e1-e1e1e1e1e1e1')
ON CONFLICT DO NOTHING;

-- Reviews
INSERT INTO "reviews" ("Id", "PropertyId", "BookingId", "GuestId", "Rating", "Comment", "CreatedAt")
VALUES
  ('30000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000001', 'c0000000-0000-0000-0000-000000000001', 5, 'Absolutely magical! The villa is even more beautiful than the photos.', NOW() - interval '30 days'),
  ('30000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000002', 'c0000000-0000-0000-0000-000000000003', 5, 'Perfect family retreat. Kids loved the garden and firepit.', NOW() - interval '20 days'),
  ('30000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000003', 'c0000000-0000-0000-0000-000000000005', 5, 'One of the best stays ever. Firepit under stars was unforgettable.', NOW() - interval '10 days'),
  ('30000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000004', 'c0000000-0000-0000-0000-000000000002', 5, 'Perfect location and very clean. Walking distance to everything.', NOW() - interval '25 days'),
  ('30000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000005', 'c0000000-0000-0000-0000-000000000004', 4, 'Great studio, well equipped. A bit noisy at night but expected for D1.', NOW() - interval '15 days'),
  ('30000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000006', 'c0000000-0000-0000-0000-000000000006', 5, 'Excellent value! Host was super responsive.', NOW() - interval '5 days'),
  ('30000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000012', '40000000-0000-0000-0000-000000000007', 'c0000000-0000-0000-0000-000000000007', 5, 'Peaceful spot by the river. Garden is gorgeous.', NOW() - interval '35 days'),
  ('30000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000012', '40000000-0000-0000-0000-000000000008', 'c0000000-0000-0000-0000-000000000009', 4, 'Lovely traditional house. Porch by the river is the highlight.', NOW() - interval '8 days'),
  ('30000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000008', '40000000-0000-0000-0000-000000000009', 'c0000000-0000-0000-0000-000000000008', 4, 'Great location and views. Some maintenance issues.', NOW() - interval '40 days'),
  ('30000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000013', '40000000-0000-0000-0000-000000000010', 'c0000000-0000-0000-0000-000000000001', 5, 'Authentic experience! Cooking class was a highlight.', NOW() - interval '22 days'),
  ('30000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000014', '40000000-0000-0000-0000-000000000011', 'c0000000-0000-0000-0000-000000000010', 4, 'Nice hotel suite with great beach access. Infinity pool is amazing.', NOW() - interval '12 days'),
  ('30000000-0000-0000-0000-000000000012', '10000000-0000-0000-0000-000000000015', '40000000-0000-0000-0000-000000000012', 'c0000000-0000-0000-0000-000000000009', 5, 'Luxury eco-resort done right. Sunsets are unreal.', NOW() - interval '18 days'),
  ('30000000-0000-0000-0000-000000000013', '10000000-0000-0000-0000-000000000019', '40000000-0000-0000-0000-000000000013', 'c0000000-0000-0000-0000-000000000003', 5, 'Beautiful courtyard house near Imperial City. Breakfast was delicious.', NOW() - interval '28 days'),
  ('30000000-0000-0000-0000-000000000014', '10000000-0000-0000-0000-000000000020', '40000000-0000-0000-0000-000000000014', 'c0000000-0000-0000-0000-000000000008', 4, 'Great budget option. Clean and helpful host.', NOW() - interval '7 days')
ON CONFLICT ("BookingId") DO NOTHING;
