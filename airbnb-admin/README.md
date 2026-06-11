# AirVnV Admin Panel (airbnb-admin)

**AirVnV Admin Panel** is the operational backbone of the AirVnV platform. While the main web application (`airbnb-web`) is optimized for conversion (Guests & Hosts), this Next.js application is specifically designed for **Operations (Ops)**, **Moderation**, and **Financial Management**.

## 🎯 Implemented Features

The Admin Panel provides a comprehensive suite of management tools, fully integrated with the microservices backend:

1. **User Management (`/users`)**
   - View and manage Host and Guest accounts.
   - Monitor user status and account details.

2. **Property Moderation (`/properties`)**
   - Manage the Approval Queue for new listings.
   - Review comprehensive property details, amenities, and location.
   - Approve or Reject properties with specific reasoning (via `reject-property-dialog`).

3. **Finance & Payouts (`/payments`, `/payouts`, `/host-balances`)**
   - **Payments:** Track incoming cash flows from Guest bookings. Includes refund management capabilities.
   - **Payouts:** Reconcile completed bookings and manage payouts to Hosts.
   - **Host Balances:** Detailed ledger of host earnings and pending balances.

4. **Analytics & Reports (`/reports`)**
   - Comprehensive dashboard with charts and data tables.
   - Modules for System Overview, Property performance, and User statistics.
   - Dynamic date-range filtering for financial and operational metrics.

5. **Platform Settings (`/settings`)**
   - Global configuration for the platform (e.g., Platform Fee adjustments).

## 🎨 Design System

The Admin Panel adheres to the strict AirVnV design language defined in [`DESIGN-airbnb.md`](./DESIGN-airbnb.md):

* **Brand Color:** The singular, recognizable "Rausch" (`#ff385c`) is used exclusively for primary CTAs and critical brand moments.
* **Typography:** Clean, airy typography leveraging standard sans-serif stacks optimized for high data density.
* **Shape Language:** Soft borders. Cards use `14px` (`rounded-md`) and pills/inputs use fully rounded or `8px` (`rounded-sm`) borders. There are no hard corners.
* **Elevation:** Minimalist shadow usage. A single hover-float shadow tier is used; depth is otherwise achieved via whitespace and contrast against the pure white (`#ffffff`) canvas.

## 🛠️ Tech Stack

* **Framework:** Next.js 16 (App Router) + React 19
* **Language:** TypeScript
* **Styling:** Tailwind CSS v4 + Shadcn/UI (Customized with AirVnV Design Tokens)
* **State Management:** Zustand (Client State), TanStack Query v5 (Server State)
* **Data Visualization:** Recharts
* **Forms & Validation:** React Hook Form + Zod

## 🚀 Getting Started

### Prerequisites

* Node.js 18+ or Node.js 20+
* Ensure the backend `.NET Aspire AppHost` is running on your local machine to provide API endpoints.

### Installation & Run

1. Install dependencies:
   ```bash
   cd airbnb-admin
   npm install
   ```

2. Start the development server (runs on port `9999` by default):
   ```bash
   npm run dev
   ```

3. Open [http://localhost:9999](http://localhost:9999) in your browser.
