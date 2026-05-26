import { TrendingUp, TrendingDown, ArrowDownCircle, ArrowUpCircle, Percent, Receipt } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import type { DashboardFinance } from '../types';

interface AdminDashboardFinanceProps {
  finance: DashboardFinance | null;
  loading: boolean;
  error: string | null;
}

function FinanceStatCard({
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

function FinanceChartCard({
  title,
  data,
  loading,
  valueKey,
}: {
  title: string;
  data: { date: string; payIn: number; payOut: number; revenue: number }[];
  loading: boolean;
  valueKey: 'payIn' | 'payOut' | 'revenue';
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

  const maxValue = Math.max(...data.map((d) => d[valueKey]), 1);

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
              style={{ height: `${(item[valueKey] / maxValue) * 100}%` }}
              title={`${item.date}: ${item[valueKey].toLocaleString()}`}
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

export function AdminDashboardFinance({ finance, loading, error }: AdminDashboardFinanceProps) {
  if (error) {
    return (
      <Card className="p-6">
        <p className="text-red-500">Error: {error}</p>
      </Card>
    );
  }

  if (loading || !finance) {
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
      {/* Key Financial Metrics Row */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <FinanceStatCard
          title="Total Pay-In"
          value={finance.totalPayIn}
          subtitle="From guests"
          icon={ArrowDownCircle}
          trend="up"
          isCurrency
        />
        <FinanceStatCard
          title="Total Pay-Out"
          value={finance.totalPayOut}
          subtitle="To hosts"
          icon={ArrowUpCircle}
          trend="up"
          isCurrency
        />
        <FinanceStatCard
          title="Platform Revenue"
          value={finance.platformRevenue}
          subtitle="Platform fees"
          icon={Percent}
          trend="up"
          isCurrency
        />
        <FinanceStatCard
          title="Avg Transaction"
          value={finance.averageTransactionAmount}
          subtitle={`${finance.pendingPayoutCount} txns`}
          icon={Receipt}
          isCurrency
        />
      </div>

      {/* Charts Row */}
      <div className="grid gap-4 md:grid-cols-2">
        <FinanceChartCard
          title="Pay-In Trends"
          data={finance.dailyStats}
          loading={loading}
          valueKey="payIn"
        />
        <FinanceChartCard
          title="Revenue Trends"
          data={finance.dailyStats}
          loading={loading}
          valueKey="revenue"
        />
      </div>

      {/* Pending Payouts Summary */}
      <Card>
        <CardHeader>
          <CardTitle>Pending Payouts</CardTitle>
          <CardDescription>Amounts waiting for processing</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-between">
            <div>
              <div className="text-3xl font-bold">
                {new Intl.NumberFormat('vi-VN', {
                  style: 'currency',
                  currency: 'VND',
                }).format(finance.pendingPayouts)}
              </div>
              <p className="text-sm text-muted-foreground">
                {finance.pendingPayoutCount} transactions
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
