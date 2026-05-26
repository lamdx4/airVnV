import { useEffect, useState } from 'react';
import { useTransactionHistory } from '../hooks/useTransactionHistory';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Eye } from 'lucide-react';
import type { TransactionItem } from '../../types';

interface TransactionHistoryTableProps {
  initialParams?: {
    page?: number;
    pageSize?: number;
    status?: string;
    fromDate?: string;
    toDate?: string;
  };
}

const formatCurrency = (amount: number, currency = 'VND') =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency }).format(amount);

const formatDate = (dateStr: string) =>
  new Date(dateStr).toLocaleString('vi-VN');

const getStatusBadge = (status: string) => {
  const styles = {
    Success: 'bg-green-100 text-green-800',
    Pending: 'bg-yellow-100 text-yellow-800',
    Failed: 'bg-red-100 text-red-800',
    Expired: 'bg-gray-100 text-gray-800',
  };
  return (
    <Badge className={styles[status as keyof typeof styles] || 'bg-gray-100'}>
      {status}
    </Badge>
  );
};

export function TransactionHistoryTable({ initialParams }: TransactionHistoryTableProps) {
  const [page, setPage] = useState(initialParams?.page || 1);
  const pageSize = initialParams?.pageSize || 20;
  const [statusFilter, setStatusFilter] = useState(initialParams?.status || '');
  const [fromDate, setFromDate] = useState(initialParams?.fromDate || '');
  const [toDate, setToDate] = useState(initialParams?.toDate || '');

  const { data, loading, error, fetchTransactions } = useTransactionHistory();

  useEffect(() => {
    fetchTransactions({
      page,
      pageSize,
      status: statusFilter || undefined,
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
    });
  }, [page, pageSize, statusFilter, fromDate, toDate]);

  const handleSearch = () => {
    setPage(1);
    fetchTransactions({
      page: 1,
      pageSize,
      status: statusFilter || undefined,
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
    });
  };

  if (error) {
    return (
      <Card className="p-6">
        <p className="text-red-500">Error: {error}</p>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Summary Cards */}
      {data?.summary && (
        <div className="grid gap-4 md:grid-cols-3 lg:grid-cols-6">
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Total Pay-In</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold">{formatCurrency(data.summary.totalPayIn)}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Total Pay-Out</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold">{formatCurrency(data.summary.totalPayOut)}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Success</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold text-green-600">{data.summary.successTransactions}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Failed</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold text-red-600">{data.summary.failedTransactions}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Pending</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold text-yellow-600">{data.summary.pendingTransactions}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-xs text-muted-foreground">Total</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-lg font-bold">{data.summary.totalTransactions}</div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Filter Bar */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Transaction History</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-wrap gap-4">
            <div className="flex items-center gap-2">
              <label className="text-sm">Status:</label>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger className="w-32">
                  <SelectValue placeholder="All" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">All</SelectItem>
                  <SelectItem value="Success">Success</SelectItem>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Failed">Failed</SelectItem>
                  <SelectItem value="Expired">Expired</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-center gap-2">
              <label className="text-sm">From:</label>
              <Input
                type="date"
                value={fromDate}
                onChange={(e) => setFromDate(e.target.value)}
                className="w-40"
              />
            </div>
            <div className="flex items-center gap-2">
              <label className="text-sm">To:</label>
              <Input
                type="date"
                value={toDate}
                onChange={(e) => setToDate(e.target.value)}
                className="w-40"
              />
            </div>
            <Button onClick={handleSearch}>Filter</Button>
          </div>
        </CardContent>
      </Card>

      {/* Transactions Table */}
      <Card>
        <CardContent className="p-0">
          {!data || loading ? (
            <div className="p-6 space-y-4">
              {[...Array(5)].map((_, i) => (
                <Skeleton key={i} className="h-12 w-full" />
              ))}
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="border-b bg-muted/50">
                  <tr>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Transaction ID</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Amount</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Fee</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Net</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Status</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Date</th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-muted-foreground">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.length === 0 ? (
                    <tr>
                      <td colSpan={7} className="px-4 py-8 text-center text-muted-foreground">
                        No transactions found
                      </td>
                   　　</tr>
                  ) : (
                    data.items.map((txn: TransactionItem) => (
                      <tr key={txn.paymentId} className="border-b hover:bg-muted/30">
                        <td className="px-4 py-3 text-sm font-mono">
                          {txn.transactionId || txn.paymentId.slice(0, 8)}
                        </td>
                        <td className="px-4 py-3 text-sm font-medium">
                          {formatCurrency(txn.amount, txn.currency)}
                        </td>
                        <td className="px-4 py-3 text-sm text-muted-foreground">
                          {formatCurrency(txn.platformFee, txn.currency)}
                        </td>
                        <td className="px-4 py-3 text-sm">
                          {formatCurrency(txn.netAmount, txn.currency)}
                        </td>
                        <td className="px-4 py-3">
                          {getStatusBadge(txn.status)}
                        </td>
                        <td className="px-4 py-3 text-sm text-muted-foreground">
                          {formatDate(txn.createdAt)}
                        </td>
                        <td className="px-4 py-3">
                          <Button variant="ghost" size="sm">
                            <Eye className="h-4 w-4" />
                          </Button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Pagination */}
      {data && data.totalCount > 0 && (
        <div className="flex items-center justify-between">
          <div className="text-sm text-muted-foreground">
            Showing {(page - 1) * pageSize + 1} to {Math.min(page * pageSize, data.totalCount)} of {data.totalCount}
          </div>
          <div className="flex gap-2">
            <Button
              variant="outline"
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
            >
              Previous
            </Button>
            <Button
              variant="outline"
              onClick={() => setPage((p) => p + 1)}
              disabled={page * pageSize >= data.totalCount}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
