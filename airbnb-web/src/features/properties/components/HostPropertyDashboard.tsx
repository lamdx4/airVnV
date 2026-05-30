import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  PlusSignIcon, 
  MoreHorizontalIcon, 
  ViewIcon, 
  Edit02Icon, 
  Delete02Icon,
  Search01Icon,
  FilterIcon,
  Home01Icon,
  ArrowRight02Icon, 
  ArrowLeft02Icon,
  Tick02Icon
} from 'hugeicons-react';
import { useMyProperties } from '../hooks/useProperties';
import { PropertyStatus } from '../types';
import { getStatusColor, getStatusText } from '../utils/status';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

export const HostPropertyDashboard: React.FC = () => {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const pageSize = 5;
  
  const { data, isLoading } = useMyProperties(page, pageSize);
  const { data: allData } = useMyProperties(1, 1000);
  
  const properties = data?.items || [];
  const totalCount = allData?.totalCount || data?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / pageSize);

  const allProperties = allData?.items || [];
  const activeCount = allProperties.filter(p => p.status === PropertyStatus.Published).length;
  const draftCount = allProperties.filter(p => p.status === PropertyStatus.Draft).length;

  if (isLoading) return <DashboardSkeleton />;

  return (
    <div className="max-w-7xl mx-auto space-y-8">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-hof">Your Listings</h1>
          <p className="text-slate-500 mt-1">Manage your properties and track their performance.</p>
        </div>
        <Button 
          onClick={() => navigate('/host/homes/new')}
          className="bg-black hover:bg-black/80 text-white rounded-2xl h-12 px-6 gap-2 font-bold shadow-lg shadow-rausch/20"
        >
          <PlusSignIcon className="h-5 w-5" />
          Create New Listing
        </Button>
      </div>

      {/* Stats Bar */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
         <StatCard label="Total Listings" value={totalCount.toString()} icon={Home01Icon} color="bg-blue-50 text-blue-600" />
         <StatCard label="Active" value={activeCount.toString()} icon={Tick02Icon} color="bg-green-50 text-green-600" />
         <StatCard label="Drafts" value={draftCount.toString()} icon={Edit02Icon} color="bg-slate-50 text-slate-600" />
      </div>

      {/* Filter & Search */}
      <div className="flex items-center gap-4 bg-white p-4 rounded-3xl border shadow-sm">
        <div className="flex-1 relative">
            <Search01Icon className="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
            <input 
                placeholder="Search by title or location..."
                className="w-full pl-12 pr-4 py-2 rounded-xl border-none focus:ring-2 ring-rausch/20 outline-none text-sm"
            />
        </div>
        <Button variant="outline" className="rounded-xl border-slate-200 gap-2">
            <FilterIcon className="h-4 w-4" />
            Filters
        </Button>
      </div>

      {/* Desktop Table / Mobile Cards */}
      <div className="bg-white rounded-[2rem] border shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50/50 border-bottom">
                <th className="px-6 py-4 text-[10px] font-bold uppercase text-slate-400 tracking-widest">Listing</th>
                <th className="px-6 py-4 text-[10px] font-bold uppercase text-slate-400 tracking-widest">Status</th>
                <th className="px-6 py-4 text-[10px] font-bold uppercase text-slate-400 tracking-widest">Price</th>
                <th className="px-6 py-4 text-[10px] font-bold uppercase text-slate-400 tracking-widest">Location</th>
                <th className="px-6 py-4 text-[10px] font-bold uppercase text-slate-400 tracking-widest text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {properties.map((property) => (
                <tr key={property.id} className="hover:bg-slate-50/50 transition-colors group">
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-4">
                      <div className="h-16 w-16 rounded-xl overflow-hidden border bg-slate-100 shrink-0">
                        <img 
                            src={property.coverImageUrl || 'https://placehold.co/100x100?text=No+Img'} 
                            className="h-full w-full object-cover" 
                            alt={property.title}
                        />
                      </div>
                      <div className="min-w-0">
                        <p className="font-bold text-hof truncate max-w-[240px] group-hover:text-rausch transition-colors">{property.title}</p>
                        <p className="text-xs text-slate-400 mt-1 truncate max-w-[200px]">{property.guestCount} guests • {property.bedroomCount} bedrooms</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <Badge className={`${getStatusColor(property.status)} border-none px-3 py-1`}>
                      {getStatusText(property.status)}
                    </Badge>
                  </td>
                  <td className="px-6 py-4">
                    <span className="font-bold text-hof">${property.basePrice}</span>
                    <span className="text-xs text-slate-400"> / night</span>
                  </td>
                  <td className="px-6 py-4">
                    <span className="text-sm text-slate-600 truncate max-w-[150px] inline-block">{property.displayAddress || 'Not set'}</span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-10 w-10 p-0 rounded-full hover:bg-white hover:shadow-md">
                          <MoreHorizontalIcon className="h-5 w-5 text-slate-400" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end" className="w-48 p-2 rounded-2xl shadow-xl border-slate-100">
                        <DropdownMenuItem onClick={() => navigate(`/host/homes/${property.id}/edit`)} className="rounded-xl gap-3 py-2 cursor-pointer">
                          <Edit02Icon className="h-4 w-4 text-blue-500" />
                          Edit Listing
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => navigate(`/properties/${property.id}?isPreview=true`)} className="rounded-xl gap-3 py-2 cursor-pointer">
                          <ViewIcon className="h-4 w-4 text-slate-500" />
                          View Live
                        </DropdownMenuItem>
                        <DropdownMenuItem className="rounded-xl gap-3 py-2 text-rausch cursor-pointer focus:text-rausch focus:bg-rausch/5">
                          <Delete02Icon className="h-4 w-4" />
                          Delete
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {properties.length === 0 && (
          <div className="py-20 text-center flex flex-col items-center gap-4">
            <div className="p-6 bg-slate-50 rounded-full">
              <Home01Icon className="h-12 w-12 text-slate-300" />
            </div>
            <div>
              <p className="text-xl font-bold text-hof">No listings found</p>
              <p className="text-slate-500">Get started by creating your first property listing.</p>
            </div>
            <Button 
                onClick={() => navigate('/host/homes/new')}
                className="bg-slate-900 text-white rounded-xl px-8 mt-2"
            >
                Create Listing
            </Button>
          </div>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="px-6 py-6 border-t flex items-center justify-between bg-slate-50/30">
            <p className="text-xs text-slate-500 font-medium">
              Showing <span className="font-bold text-hof">{(page - 1) * pageSize + 1}</span> to{' '}
              <span className="font-bold text-hof">{Math.min(page * pageSize, totalCount)}</span> of{' '}
              <span className="font-bold text-hof">{totalCount}</span> results
            </p>
            <div className="flex items-center gap-2">
              <Button 
                variant="outline" 
                size="sm" 
                className="rounded-lg h-9 w-9 p-0"
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
              >
                <ArrowLeft02Icon className="h-4 w-4" />
              </Button>
              {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
                <Button 
                    key={p}
                    variant={p === page ? 'default' : 'outline'}
                    size="sm"
                    className="rounded-lg h-9 w-9 p-0"
                    onClick={() => setPage(p)}
                >
                    {p}
                </Button>
              ))}
              <Button 
                variant="outline" 
                size="sm" 
                className="rounded-lg h-9 w-9 p-0"
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
              >
                <ArrowRight02Icon className="h-4 w-4" />
              </Button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

const StatCard: React.FC<{ label: string, value: string, icon: any, color: string }> = ({ label, value, icon: Icon, color }) => (
    <div className="bg-white p-6 rounded-3xl border shadow-sm flex items-center gap-4">
        <div className={`p-3 rounded-2xl ${color}`}>
            <Icon className="h-6 w-6" />
        </div>
        <div>
            <p className="text-xs font-bold uppercase text-slate-400 tracking-wider">{label}</p>
            <p className="text-2xl font-black text-hof mt-0.5">{value}</p>
        </div>
    </div>
);

const DashboardSkeleton = () => (
    <div className="max-w-7xl mx-auto space-y-8 animate-pulse">
        <div className="h-20 bg-slate-100 rounded-3xl" />
        <div className="grid grid-cols-3 gap-6">
            {[1, 2, 3].map(i => <div key={i} className="h-32 bg-slate-100 rounded-3xl" />)}
        </div>
        <div className="h-96 bg-slate-100 rounded-[2rem]" />
    </div>
);
