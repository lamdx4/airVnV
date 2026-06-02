export interface ReportSummary {
  totalRevenue: number;
  totalBookings: number;
  averageBookingValue: number;
  occupancyRate: number;
  newUsers: number;
  newProperties: number;
}

export interface RevenueBreakdown {
  period: string;
  revenue: number;
  bookings: number;
  cancellations: number;
}

export interface TopProperty {
  id: string;
  title: string;
  revenue: number;
  bookings: number;
  occupancyRate: number;
}

export interface UserGrowthPoint {
  date: string;
  guests: number;
  hosts: number;
}
