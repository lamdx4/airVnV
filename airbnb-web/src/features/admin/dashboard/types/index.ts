export interface DashboardStats {
  totalBookings: number;
  newBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  totalRevenue: number;
  gmvVnd: number;
  totalProperties: number;
  averageOccupancyRate: number;
  totalGuests: number;
  totalHosts: number;
  dailyStats: DailyStatItem[];
}

export interface DailyStatItem {
  date: string;
  bookingCount: number;
  revenue: number;
  newUsers: number;
}
