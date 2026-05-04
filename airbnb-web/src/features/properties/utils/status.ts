import { PropertyStatus } from '../types';

export const getStatusConfig = (status: PropertyStatus) => {
  switch (status) {
    case PropertyStatus.Draft:
      return { label: 'Draft', color: 'bg-slate-100 text-slate-700 border-slate-200' };
    case PropertyStatus.PendingReview:
      return { label: 'Pending Review', color: 'bg-amber-100 text-amber-700 border-amber-200' };
    case PropertyStatus.Published:
      return { label: 'Published', color: 'bg-emerald-100 text-emerald-700 border-emerald-200' };
    case PropertyStatus.Suspended:
      return { label: 'Suspended', color: 'bg-rose-100 text-rose-700 border-rose-200' };
    case PropertyStatus.Archived:
      return { label: 'Archived', color: 'bg-gray-200 text-gray-800 border-gray-300' };
    default:
      return { label: 'Unknown', color: 'bg-gray-100 text-gray-600' };
  }
};
