import React from 'react';
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { 
  DropdownMenu, 
  DropdownMenuContent, 
  DropdownMenuItem, 
  DropdownMenuTrigger,
  DropdownMenuSeparator
} from '@/components/ui/dropdown-menu';
import { 
  MoreHorizontalIcon, 
  PlusSignIcon, 
  ViewIcon, 
  PencilEdit01Icon, 
  Delete02Icon, 
  Archive02Icon, 
  SentIcon, 
  Image01Icon,
  Search01Icon,
  Cancel01Icon
} from 'hugeicons-react';
import { useMyProperties, useSubmitProperty, useArchiveProperty, useDeleteProperty } from '../hooks/useProperties';
import { CreatePropertyDialog } from './CreatePropertyDialog';
import { getStatusConfig } from '../utils/status';
import { PropertyStatus } from '../types';
import { format } from 'date-fns';
import { toast } from 'sonner';
import { Search as SearchIcon, X } from 'lucide-react';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export const HostPropertyDashboard: React.FC = () => {
  const [page, setPage] = React.useState(1);
  const [searchTerm, setSearchTerm] = React.useState('');
  const [debouncedSearch, setDebouncedSearch] = React.useState('');
  const [statusFilter, setStatusFilter] = React.useState<string>('all');
  
  const pageSize = 5;

  // Debounce search term
  React.useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchTerm);
      setPage(1); // Reset to page 1 when searching
    }, 500);
    return () => clearTimeout(timer);
  }, [searchTerm]);

  const { data, isLoading, error } = useMyProperties({ 
    pageNumber: page, 
    pageSize,
    searchTerm: debouncedSearch || undefined,
    status: statusFilter === 'all' ? undefined : parseInt(statusFilter)
  });
  const properties = data?.items;

  const submitMutation = useSubmitProperty();
  const archiveMutation = useArchiveProperty();
  const deleteMutation = useDeleteProperty();

  if (isLoading) return <div className="p-8 text-center">Loading your properties...</div>;
  if (error) return <div className="p-8 text-center text-red-500">Error loading properties</div>;

  const handleSubmit = async (id: string) => {
    try {
      await submitMutation.mutateAsync(id);
      toast.success('Property submitted for review!');
    } catch (err: any) {
      toast.error(err.message || 'Failed to submit property');
    }
  };

  const handleArchive = async (id: string) => {
    try {
      await archiveMutation.mutateAsync(id);
      toast.success('Property archived successfully');
    } catch (err: any) {
      toast.error(err.message || 'Failed to archive property');
    }
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Your Listings</h1>
          <p className="text-muted-foreground">Manage your properties and track their status.</p>
        </div>
        <CreatePropertyDialog />
      </div>

      <div className="flex flex-col md:flex-row gap-4 items-center justify-between bg-slate-50 p-4 rounded-xl border border-slate-100">
        <div className="relative w-full md:w-96">
          <Search01Icon className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input 
            placeholder="Search by title..." 
            className="pl-10 pr-10 bg-white"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          {searchTerm && (
            <button 
              onClick={() => setSearchTerm('')}
              className="absolute right-3 top-1/2 -translate-y-1/2 hover:text-rausch transition-colors"
            >
              <Cancel01Icon className="h-4 w-4 text-muted-foreground" />
            </button>
          )}
        </div>
        
        <div className="flex items-center gap-2 w-full md:w-auto">
          <span className="text-sm font-medium text-muted-foreground whitespace-nowrap">Status:</span>
          <Select 
            value={statusFilter} 
            onValueChange={(value) => {
              setStatusFilter(value);
              setPage(1);
            }}
          >
            <SelectTrigger className="w-full md:w-[180px] bg-white">
              <SelectValue placeholder="All Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Listings</SelectItem>
              <SelectItem value={PropertyStatus.Draft.toString()}>Draft</SelectItem>
              <SelectItem value={PropertyStatus.PendingReview.toString()}>Pending Review</SelectItem>
              <SelectItem value={PropertyStatus.Published.toString()}>Published</SelectItem>
              <SelectItem value={PropertyStatus.Suspended.toString()}>Suspended</SelectItem>
              <SelectItem value={PropertyStatus.Archived.toString()}>Archived</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="border rounded-xl bg-white shadow-sm overflow-hidden">
        <Table>
          <TableHeader className="bg-slate-50">
            <TableRow>
              <TableHead className="w-[300px]">Property</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Price</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Updated At</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {properties?.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-32 text-center text-muted-foreground">
                  You don't have any listings yet.
                </TableCell>
              </TableRow>
            ) : (
              properties?.map((property) => {
                const status = getStatusConfig(property.status);
                return (
                  <TableRow key={property.id} className="hover:bg-slate-50 transition-colors">
                    <TableCell className="font-medium">
                      <div className="flex items-center space-x-3">
                        <div className="w-16 h-12 rounded-md bg-slate-100 flex items-center justify-center overflow-hidden border">
                          {property.images.find(img => img.type === 0)?.url ? (
                            <img 
                              src={property.images.find(img => img.type === 0)?.url} 
                              alt={property.title}
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            <Image01Icon className="text-slate-400 h-6 w-6" />
                          )}
                        </div>
                        <div className="flex flex-col">
                          <span className="line-clamp-1">{property.title}</span>
                          <span className="text-xs text-muted-foreground">{property.capacity.guestCount} guests · {property.capacity.bedroomCount} BR</span>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline" className={`${status.color} px-2 py-0.5 font-medium border`}>
                        {status.label}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <span className="font-semibold">${property.pricing.basePrice}</span>
                      <span className="text-xs text-muted-foreground ml-1">/ night</span>
                    </TableCell>
                    <TableCell className="text-muted-foreground text-sm max-w-[150px] truncate">
                      {property.displayAddress}
                    </TableCell>
                    <TableCell className="text-sm text-muted-foreground">
                      {format(new Date(property.updatedAt || property.createdAt), 'MMM d, yyyy')}
                    </TableCell>
                    <TableCell className="text-right">
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" className="h-8 w-8 p-0">
                            <MoreHorizontalIcon className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end" className="w-48">
                          <DropdownMenuItem>
                            <ViewIcon className="mr-2 h-4 w-4" /> View Listing
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <PencilEdit01Icon className="mr-2 h-4 w-4" /> Edit Details
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <Image01Icon className="mr-2 h-4 w-4" /> Manage Images
                          </DropdownMenuItem>
                          
                          <DropdownMenuSeparator />
                          
                          {property.status === 0 && ( // Draft
                            <DropdownMenuItem 
                              onClick={() => handleSubmit(property.id)}
                              className="text-amber-600 focus:text-amber-600 focus:bg-amber-50"
                            >
                              <SentIcon className="mr-2 h-4 w-4" /> Submit for Review
                            </DropdownMenuItem>
                          )}
                          
                          {(property.status === 2 || property.status === 3) && ( // Published or Suspended
                            <DropdownMenuItem 
                              onClick={() => handleArchive(property.id)}
                              className="text-gray-600"
                            >
                              <Archive02Icon className="mr-2 h-4 w-4" /> Archive Listing
                            </DropdownMenuItem>
                          )}

                          <DropdownMenuItem className="text-rose-600 focus:text-rose-600 focus:bg-rose-50">
                            <Delete02Icon className="mr-2 h-4 w-4" /> Delete Listing
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                );
              })
            )}
          </TableBody>
        </Table>
      </div>

      {/* Pagination Controls */}
      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between px-2 py-4">
          <p className="text-sm text-muted-foreground">
            Showing <span className="font-medium">{(data.pageNumber - 1) * data.pageSize + 1}</span> to{' '}
            <span className="font-medium">
              {Math.min(data.pageNumber * data.pageSize, data.totalCount)}
            </span>{' '}
            of <span className="font-medium">{data.totalCount}</span> results
          </p>
          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={!data.hasPreviousPage}
            >
              Previous
            </Button>
            <div className="flex items-center gap-1">
              {Array.from({ length: data.totalPages }, (_, i) => i + 1).map((p) => (
                <Button
                  key={p}
                  variant={p === data.pageNumber ? 'default' : 'outline'}
                  size="sm"
                  className="w-8 h-8 p-0"
                  onClick={() => setPage(p)}
                >
                  {p}
                </Button>
              ))}
            </div>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
              disabled={!data.hasNextPage}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  );
};
