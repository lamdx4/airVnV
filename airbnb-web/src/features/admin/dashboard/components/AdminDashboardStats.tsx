import { TrendingUp, TrendingDown, Users, Home, Calendar, DollarSign } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import type { DashboardStats } from '../types';

interface AdminDashboardStatsProps {
  stats: DashboardStats | null;
  loading: boolean;
  error: string | null;
}

function StatCard({
  title,
  value,
  subtitle,
  icon: Icon,
  trend,
  isCurrency = false,
}: {
  title: string;
  value: number;
  subtitle?: string;
  icon: React.ElementType;
  trend?: 'up' | 'down';
  isCurrency?: boolean;
}) {
  const formatValue = isCurrency
    ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
    : new Intl.NumberFormat().format(value);

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        <Icon className="h-4 w-4 text-muted-foreground" />
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold">{formatValue}</div>
        {subtitle && (
          <p className="text-xs text-muted-foreground">
            {trend === 'up' && <TrendingUp className="inline h-3 w-3 text-green-500 mr-1" />}
            {trend === 'down' && <TrendingDown className="inline h-3 w-3 text-red-500 mr-1" />}
            {subtitle}
          </p>
        )}
      </CardContent>
    </Card>
  );
}

function ChartCard({
  title,
  data,
  loading,
}: {
  title: string;
  data: { date: string; value: number }[];
  loading: boolean;
}) {
  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>{title}</CardTitle>
        </CardHeader>
        <CardContent>
          <Skeleton className="h-40 w-full" />
        </CardContent>
      </Card>
    );
  }

  const maxValue = Math.max(...data.map((d) => d.value), 1);

  return (
    <Card>
      <CardHeader>
        <CardTitle>{title}</CardTitle>
        <CardDescription>Last 30 days</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="h-40 flex items-end gap-1">
          {data.map((item, index) => (
            <div
              key={index}
              className="flex-1 bg-primary/80 rounded-t transition-all hover:bg-primary"
              style={{ height: `${(item.value / maxValue) * 100}%` }}
              title={`${item.date}: ${item.value.toLocaleString()}`}
            />
          ))}
        </div>
        <div className="flex justify-between mt-2 text-xs text-muted-foreground">
          <span>{data[0]?.date || '-'}</span>
          <span>{data[data.length - 1]?.date || '-'}</span>
        </div>
      </CardContent>
    </Card>
  );
}

export function AdminDashboardStats({ stats, loading, error }: AdminDashboardStatsProps) {
  if (error) {
    return (
      <Card className="p-6">
        <p className="text-red-500">Error: {error}</p>
      </Card>
    );
  }

  if (loading || !stats) {
    return (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {[...Array(4)].map((_, i) => (
          <Card key={i}>
            <CardHeader className="pb-2">
              <Skeleton className="h-4 w-20" />
            </CardHeader>
            <CardContent>
              <Skeleton className="h-8 w-24" />
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Key Metrics Row */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Bookings"
          value={stats.totalBookings}
          subtitle={`${stats.newBookings} new this period`}
          icon={Calendar}
          trend={stats.newBookings > 0 ? 'up' : 'down'}
        />
        <StatCard
          title="Gross Revenue"
          value={stats.gmvVnd}
          subtitle={`${stats.averageOccupancyRate.toFixed(1)}% occupancy`}
          icon={DollarSign}
          trend="up"
          isCurrency
        />
        <StatCard
          title="Active Guests"
          value={stats.totalGuests}
          subtitle="Unique guests"
          icon={Users}
        />
        <StatCard
          title="Active Hosts"
          value={stats.totalHosts}
          subtitle="Property owners"
          icon={Home}
        />
      </div>

      {/* Charts Row */}
      <div className="grid gap-4 md:grid-cols-2">
        <ChartCard
          title="Booking Trends"
          data={stats.dailyStats.map((d) => ({
            date: d.date,
            value: d.bookingCount,
          }))}
          loading={loading}
        />
        <ChartCard
          title="Revenue Trends"
          data={stats.dailyStats.map((d) => ({
            date: d.date,
            value: d.revenue,
          }))}
          loading={loading}
        />
      </div>

      {/* Booking Status Summary */}
      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-green-600">Confirmed</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.confirmedBookings}</div>
            <p className="text-xs text-muted-foreground">
              {((stats.confirmedBookings / stats.totalBookings) * 100).toFixed(1)}% of total
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-yellow-600">New</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.newBookings}</div>
            <p className="text-xs text-muted-foreground">Pending processing</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-red-600">Cancelled</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.cancelledBookings}</div>
            <p className="text-xs text-muted-foreground">
              {((stats.cancelledBookings / stats.totalBookings) * 100).toFixed(1)}% of total
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
