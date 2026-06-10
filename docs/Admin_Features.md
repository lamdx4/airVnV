# Admin Features — Tổng hợp

> Document này tổng hợp tất cả các feature liên quan đến Admin trong hệ thống airVnV.

---

## 🖥️ Frontend — `airbnb-admin/`

### Pages / Routes

| Route | Mô tả |
|-------|-------|
| `/users` | Quản lý người dùng |
| `/users/[id]` | Chi tiết người dùng |
| `/properties` | Quản lý property |
| `/properties/[id]` | Chi tiết property |
| `/bookings` | Quản lý đặt phòng |
| `/payments` | Quản lý thanh toán |
| `/payments/[id]` | Chi tiết thanh toán |
| `/payouts` | Quản lý payout |
| `/payouts/[id]` | Chi tiết payout |
| `/host-balances` | Quản lý số dư host |
| `/reports` | Báo cáo |
| `/settings` | Cài đặt platform |
| `/login` | Đăng nhập admin |

### Feature Modules (9 modules)

```
airbnb-admin/src/features/
├── users/          # API + hooks + components + types
├── properties/    # API + hooks + components + types
├── bookings/      # API + hooks + components + types
├── payments/      # API + hooks + components + types
├── payouts/       # API + hooks + components + types
├── host-balances/ # API + hooks + components + types
├── reports/       # API + hooks + components + types
├── reviews/       # API + hooks + components + types
└── settings/     # API + hooks + components + types
```

### Component Layout

```
airbnb-admin/src/components/
├── layout/
│   ├── admin-sidebar.tsx
│   ├── admin-header.tsx
│   └── index.ts
├── common/
└── ui/
```

---

## ⚙️ Backend — `src/`

### Airbnb.UserService — `src/Airbnb.UserService/Features/Admin/`

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `/api/admin/users` | GET | Danh sách users |
| `/api/admin/users/{id}` | GET | Chi tiết user |
| `/api/admin/users/{id}/suspend` | POST | Suspend user |
| `/api/admin/reports/user-growth` | GET | Báo cáo tăng trưởng user |
| `/api/admin/login` | POST | Admin login |

**Files:**
- `Features/Admin/AdminGroup.cs` — Group base route `/api/admin/users`
- `Features/Admin/GetUserDetail/` — Chi tiết user
- `Features/Admin/GetUsers/` — Danh sách users
- `Features/Admin/GetUserGrowthReport/` — Báo cáo tăng trưởng
- `Features/Admin/SuspendUser/` — Suspend user
- `Features/Login/AdminLogin/` — Admin login endpoint

---

### Airbnb.PaymentService — `src/Airbnb.PaymentService/Features/Admin/`

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `/api/admin/payments` | GET | Danh sách payments |
| `/api/admin/payments/{id}` | GET | Chi tiết payment |
| `/api/admin/payments/{id}/refund` | POST | Hoàn tiền payment |
| `/api/admin/payouts` | GET | Danh sách payouts |
| `/api/admin/payouts/{id}` | GET | Chi tiết payout |
| `/api/admin/payouts/{id}/approve` | POST | Approve payout |
| `/api/admin/payouts/{id}/mark-completed` | POST | Mark payout completed |
| `/api/admin/host-balances` | GET | Danh sách host balances |
| `/api/admin/host-balances/{id}` | GET | Chi tiết host balance |
| `/api/admin/host-balances/bootstrap` | POST | Bootstrap host balances |
| `/api/admin/settings` | GET | Lấy platform settings |
| `/api/admin/settings` | PUT | Cập nhật platform settings |
| `/api/admin/reports/revenue-overview` | GET | Tổng quan doanh thu |
| `/api/admin/reports/revenue-series` | GET | Chuỗi doanh thu |

**Sub-folders:**
- `Admin/ApprovePayout/`
- `Admin/BootstrapHostBalances/`
- `Admin/GetAdminPaymentDetail/`
- `Admin/GetAdminPayments/`
- `Admin/GetAdminPayoutDetail/`
- `Admin/GetAdminPayouts/`
- `Admin/GetHostBalanceDetail/`
- `Admin/GetHostBalances/`
- `Admin/GetPlatformSettings/`
- `Admin/MarkPayoutCompleted/`
- `Admin/RefundPayment/`
- `Admin/Reports/GetRevenueOverview/`
- `Admin/Reports/GetRevenueSeries/`
- `Admin/Reports/ReportBucketing.cs`
- `Admin/UpdatePlatformSettings/`

---

### Airbnb.PropertyService — `src/Airbnb.PropertyService/Features/Admin/`

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `/api/admin/properties/{id}` | GET | Chi tiết property |
| `/api/admin/reports/new-listings` | GET | Báo cáo listing mới |
| `/api/admin/reports/pending-backlog` | GET | Báo cáo backlog |
| `/api/admin/reports/price-distribution` | GET | Phân phối giá |
| `/api/admin/reports/status-funnel` | GET | Funnel trạng thái |
| `/api/admin/reports/type-distribution` | GET | Phân phối loại |

**Sub-folders:**
- `Admin/GetPropertyDetail/`
- `Admin/Reports/GetNewListings/`
- `Admin/Reports/GetPendingBacklog/`
- `Admin/Reports/GetPriceDistribution/`
- `Admin/Reports/GetStatusFunnel/`
- `Admin/Reports/GetTypeDistribution/`

---

## 📊 Tổng kết

| Phần | Số lượng |
|------|----------|
| Frontend feature modules | 9 |
| Frontend routes | 12 |
| Backend UserService endpoints | 5 |
| Backend PaymentService endpoints | 15 |
| Backend PropertyService endpoints | 7 |
| **Tổng backend endpoints** | **~27** |

---

## 🔗 Related Documents

- `docs/Admin_BA.md` — Business Analysis cho Admin
