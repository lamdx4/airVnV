# AirVnV Admin Panel (airbnb-admin)

**AirVnV Admin Panel** is the operational backbone of the AirVnV platform. While the main web application (`airbnb-web`) is optimized for conversion (Guests & Hosts), this application is specifically designed for **Operations (Ops)**, **Moderation**, and **Dispute Resolution**.

## 🎯 Core Objectives & Epics

Based on the Business Analysis ([Admin_BA.md](../docs/Admin_BA.md)), the Admin Panel encompasses 5 major epics:

1. **Epic A: User & Identity Management**
   - Monitor Host/Guest accounts.
   - Execute Suspensions/Bans for community standard violations.
2. **Epic B: Property Moderation**
   - Manage the Approval Queue for new listings.
   - Review property details, photos, and legal documents.
   - Approve, Reject, or Emergency-Suspend active listings.
3. **Epic C: Finance & Payout**
   - Track incoming cash flow (Pay-ins from Guests).
   - Reconcile completed bookings and trigger Payouts to Hosts.
   - Configure global Platform Fees.
4. **Epic D: Resolution Center**
   - Manage Support Tickets.
   - Force partial/full refunds bypassing host policies in severe dispute cases.
5. **Epic E: Analytics Dashboard**
   - Visualize Gross Merchandise Volume (GMV), Net Revenue, and Occupancy Rates.

## 🎨 Design System

The Admin Panel adheres to the strict AirVnV design language defined in [`DESIGN-airbnb.md`](./DESIGN-airbnb.md):

* **Brand Color:** The singular, recognizable "Rausch" (`#ff385c`) is used exclusively for primary CTAs and critical brand moments.
* **Typography:** `Airbnb Cereal VF` (Fallback: `Inter`). Display weights are kept modest (500-600) to maintain a clean, airy feel instead of the heavy enterprise look.
* **Shape Language:** Soft borders. Cards use `14px` (`rounded-md`) and pills/inputs use fully rounded or `8px` (`rounded-sm`) borders. There are no hard corners.
* **Elevation:** Minimalist shadow usage. A single hover-float shadow tier is used; depth is otherwise achieved via whitespace and contrast against the pure white (`#ffffff`) canvas.

## 🛠️ Tech Stack

* **Framework:** Next.js (App Router)
* **Language:** TypeScript
* **Styling:** Tailwind CSS + Shadcn/UI (Customized with AirVnV Design Tokens)
* **State Management:** Zustand (Client State), TanStack Query (Server State)

## 🚀 Getting Started

### Prerequisites

* Node.js 18+
* Ensure the backend `.NET Aspire AppHost` is running on your local machine to provide API endpoints.

### Installation & Run

1. Install dependencies:
   ```bash
   cd airbnb-admin
   npm install
   ```

2. Start the development server:
   ```bash
   npm run dev
   ```

3. Open [http://localhost:3000](http://localhost:3000) in your browser.

## 🗺️ Execution Roadmap

1. **Phase 1:** Dashboard Layout & Epic B (Property Moderation)
2. **Phase 2:** Epic A (User Management)
3. **Phase 3:** Epic E (Analytics & Reporting)
4. **Phase 4:** Epic C & D (Finance and Resolution Center) - Pending completion of Core Booking Engine.
