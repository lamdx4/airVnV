import React from 'react';
import { Icon } from '@iconify/react';
import { 
  Loading03Icon,
  Tick02Icon
} from 'hugeicons-react';
import { PropertyAmenity } from '../types';
import { useAvailableAmenities, useAddAmenity, useRemoveAmenity } from '../hooks/useProperties';
import { toast } from 'sonner';

interface AmenityManagerProps {
  propertyId: string;
  selectedAmenities: PropertyAmenity[];
}

export const AmenityManager: React.FC<AmenityManagerProps> = ({ propertyId, selectedAmenities }) => {
  const { data: availableAmenities, isLoading } = useAvailableAmenities();
  const addMutation = useAddAmenity();
  const removeMutation = useRemoveAmenity();

  const isSelected = (amenityId: string) => selectedAmenities.some(a => a.amenityId === amenityId);

  const handleToggle = async (amenityId: string) => {
    try {
      if (isSelected(amenityId)) {
        await removeMutation.mutateAsync({ propertyId, amenityId });
      } else {
        await addMutation.mutateAsync({ propertyId, amenityId });
      }
    } catch (err: any) {
      toast.error('Failed to update amenity');
    }
  };

  if (isLoading) return (
    <div className="flex justify-center py-12">
      <Loading03Icon className="h-8 w-8 animate-spin text-rausch" />
    </div>
  );

  const categories = Array.from(new Set(availableAmenities?.map(a => a.category) || []));

  return (
    <div className="space-y-8">
      {categories.map(category => (
        <div key={category} className="space-y-4">
          <h3 className="text-lg font-bold text-hof capitalize">{category}</h3>
          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            {availableAmenities?.filter(a => a.category === category).map(amenity => {
              const active = isSelected(amenity.id);
              // Đảm bảo icon name có prefix 'hugeicons:' nếu DB chỉ lưu mỗi suffix
              const iconName = amenity.iconCode?.startsWith('hugeicons:') 
                ? amenity.iconCode 
                : `hugeicons:${amenity.iconCode || 'tick-02'}`;

              return (
                <button
                  key={amenity.id}
                  onClick={() => handleToggle(amenity.id)}
                  className={`
                    flex items-center gap-4 p-4 rounded-2xl border-2 transition-all text-left
                    ${active 
                      ? 'border-rausch bg-rausch/5 text-rausch shadow-sm ring-1 ring-rausch/20' 
                      : 'border-slate-100 bg-white text-slate-600 hover:border-slate-300 hover:bg-slate-50'
                    }
                  `}
                >
                  <div className={`${active ? 'text-rausch' : 'text-slate-400'}`}>
                    <Icon icon={iconName} className="h-6 w-6" />
                  </div>
                  <div className="flex-1">
                    <p className="font-semibold text-sm">{amenity.name}</p>
                  </div>
                  {active && <Tick02Icon className="h-4 w-4" />}
                </button>
              );
            })}
          </div>
        </div>
      ))}

      {(availableAmenities?.length === 0) && (
         <div className="text-center py-12 text-muted-foreground bg-slate-50 rounded-3xl border-2 border-dashed">
            No amenities found in the system.
         </div>
      )}
    </div>
  );
};
