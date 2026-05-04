import { PropertyStatus } from '../types';

export const getStatusText = (status: PropertyStatus): string => {
  switch (status) {
    case PropertyStatus.Draft: return 'Draft';
    case PropertyStatus.PendingReview: return 'Pending Review';
    case PropertyStatus.Published: return 'Active';
    case PropertyStatus.Suspended: return 'Suspended';
    case PropertyStatus.Archived: return 'Archived';
    default: return 'Unknown';
  }
};

export const getStatusColor = (status: PropertyStatus): string => {
  switch (status) {
    case PropertyStatus.Draft: return 'bg-slate-100 text-slate-600';
    case PropertyStatus.PendingReview: return 'bg-amber-100 text-amber-600';
    case PropertyStatus.Published: return 'bg-green-100 text-green-600';
    case PropertyStatus.Suspended: return 'bg-red-100 text-red-600';
    case PropertyStatus.Archived: return 'bg-slate-200 text-slate-500';
    default: return 'bg-slate-100 text-slate-400';
  }
};
