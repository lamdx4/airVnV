INSERT INTO "UserProfiles" ("UserId", "FullName", "PhoneNumber", "AvatarUrl", "Bio")
VALUES
  ('a0000000-0000-0000-0000-000000000001', 'Tran Minh Duc', '+84901234567', NULL, 'Community moderator since 2025'),
  ('a0000000-0000-0000-0000-000000000002', 'Sarah Johnson', '+84987654321', NULL, 'Content review specialist'),
  ('b0000000-0000-0000-0000-000000000001', 'Nguyen Thi Ngoc', '+84911111111', NULL, 'Superhost with 5 years experience in Da Lat'),
  ('b0000000-0000-0000-0000-000000000002', 'Pham Thi Linh', '+84922222222', NULL, 'Interior designer turned host in HCMC'),
  ('b0000000-0000-0000-0000-000000000003', 'Le Van Tuan', '+84933333333', NULL, 'Beach resort owner in Nha Trang'),
  ('b0000000-0000-0000-0000-000000000004', 'Mai Anh Thu', '+84944444444', NULL, 'Heritage home host in Hoi An'),
  ('b0000000-0000-0000-0000-000000000005', 'Vo Thanh Khoa', '+84955555555', NULL, 'Mountain lodge operator in Sapa'),
  ('b0000000-0000-0000-0000-000000000006', 'Tran Thi Ha', '+84966666666', NULL, 'City apartment host in Hanoi'),
  ('b0000000-0000-0000-0000-000000000007', 'David Chen', '+84977777777', NULL, 'Digital nomad host in Da Nang'),
  ('b0000000-0000-0000-0000-000000000008', 'Emma Wilson', '+84988888888', NULL, 'Eco-resort host in Phu Quoc'),
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
