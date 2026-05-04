import React, { useState } from 'react';
import { Icon } from '@iconify/react';
import { 
  Loading03Icon,
  Tick02Icon,
  NoteIcon,
  SentIcon
} from 'hugeicons-react';
import type { PropertyAmenity } from '../types';
import { 
  useAvailableAmenities, 
  useAddAmenity, 
  useRemoveAmenity,
  useUpdateAmenityInfo 
} from '../hooks/useProperties';
import { toast } from 'sonner';

interface AmenityManagerProps {
  propertyId: string;
  selectedAmenities: PropertyAmenity[];
}

export const AmenityManager: React.FC<AmenityManagerProps> = ({ propertyId, selectedAmenities }) => {
  const { data: availableAmenities, isLoading } = useAvailableAmenities();
  const addMutation = useAddAmenity();
  const removeMutation = useRemoveAmenity();
  const updateInfoMutation = useUpdateAmenityInfo();

  const [editingId, setEditingId] = useState<string | null>(null);
  const [noteValue, setNoteValue] = useState('');

  const isSelected = (amenityId: string) => selectedAmenities.find(a => a.amenityId === amenityId);

  const handleToggle = async (amenityId: string) => {
    try {
      const selected = isSelected(amenityId);
      if (selected) {
        await removeMutation.mutateAsync({ propertyId, amenityId });
      } else {
        await addMutation.mutateAsync({ propertyId, amenityId });
      }
    } catch (err: any) {
      toast.error('Failed to update amenity');
    }
  };

  const handleStartEdit = (amenityId: string, currentNote: string) => {
    setEditingId(amenityId);
    setNoteValue(currentNote || '');
  };

  const handleSaveNote = async (amenityId: string) => {
    try {
      await updateInfoMutation.mutateAsync({ 
        propertyId, 
        amenityId, 
        additionalInfo: noteValue 
      });
      setEditingId(null);
      toast.success('Note saved');
    } catch (err) {
      toast.error('Failed to save note');
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
          <h3 className="text-lg font-bold text-hof capitalize border-l-4 border-rausch pl-4">{category}</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {availableAmenities?.filter(a => a.category === category).map(amenity => {
              const selected = isSelected(amenity.id);
              const iconName = amenity.iconCode?.startsWith('hugeicons:') 
                ? amenity.iconCode 
                : `hugeicons:${amenity.iconCode || 'tick-02'}`;

              return (
                <div key={amenity.id} className="space-y-2">
                    <button
                        onClick={() => handleToggle(amenity.id)}
                        className={`
                            w-full flex items-center gap-4 p-4 rounded-2xl border-2 transition-all text-left
                            ${selected 
                            ? 'border-rausch bg-rausch/5 text-rausch shadow-sm ring-1 ring-rausch/20' 
                            : 'border-slate-100 bg-white text-slate-600 hover:border-slate-300 hover:bg-slate-50'
                            }
                        `}
                    >
                        <div className={`${selected ? 'text-rausch' : 'text-slate-400'}`}>
                            <Icon icon={iconName} className="h-6 w-6" />
                        </div>
                        <div className="flex-1">
                            <p className="font-semibold text-sm">{amenity.name}</p>
                        </div>
                        {selected && <Tick02Icon className="h-4 w-4" />}
                    </button>

                    {/* Additional Info / Note Input */}
                    {selected && (
                        <div className="px-2">
                            {editingId === amenity.id ? (
                                <div className="flex items-center gap-2">
                                    <input 
                                        autoFocus
                                        value={noteValue}
                                        onChange={(e) => setNoteValue(e.target.value)}
                                        placeholder="Add details (e.g. Wifi pass...)"
                                        className="flex-1 text-xs p-2 rounded-lg border-2 border-slate-200 focus:border-rausch outline-none transition-all"
                                        onKeyDown={(e) => e.key === 'Enter' && handleSaveNote(amenity.id)}
                                    />
                                    <button 
                                        onClick={() => handleSaveNote(amenity.id)}
                                        className="p-2 bg-rausch text-white rounded-lg hover:bg-rausch-dark transition-colors"
                                    >
                                        <SentIcon className="h-4 w-4" />
                                    </button>
                                </div>
                            ) : (
                                <button 
                                    onClick={() => handleStartEdit(amenity.id, selected.additionalInfo || '')}
                                    className="flex items-center gap-2 text-[10px] font-bold text-slate-400 hover:text-rausch transition-colors group"
                                >
                                    <NoteIcon className="h-3 w-3" />
                                    {selected.additionalInfo ? (
                                        <span className="truncate max-w-[150px] italic">"{selected.additionalInfo}"</span>
                                    ) : (
                                        <span className="uppercase tracking-widest group-hover:underline">Add Note</span>
                                    )}
                                </button>
                            )}
                        </div>
                    )}
                </div>
              );
            })}
          </div>
        </div>
      ))}
    </div>
  );
};
