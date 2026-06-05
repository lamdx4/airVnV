import React, { useState } from 'react';
import { Icon } from '@iconify/react';
import { 
  Loading03Icon,
  Tick02Icon,
  NoteIcon,
  SentIcon
} from 'hugeicons-react';
import type { PropertyAmenity } from '../types';
import { useAvailableAmenities, useAddAmenity, useRemoveAmenity, useUpdateAmenityInfo } from '../hooks/useProperties';
import { toast } from 'sonner';
import { useTranslation } from 'react-i18next';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

interface AmenityManagerProps {
  propertyId: string;
  selectedAmenities: PropertyAmenity[];
}

export const AmenityManager: React.FC<AmenityManagerProps> = ({ propertyId, selectedAmenities }) => {
  const { t } = useTranslation();
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
      toast.error(t('amenitiesManager.updateFailed'));
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
      toast.success(t('amenitiesManager.noteSaved'));
    } catch (err) {
      toast.error(t('amenitiesManager.noteSaveFailed'));
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
                    {/* Amenity Toggle – dùng bare button vì cần custom visual state phức tạp
                        (ring, bg-rausch/5, border-2) không phải variant chuẩn của Button */}
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
                                    <Input
                                        autoFocus
                                        value={noteValue}
                                        onChange={(e) => setNoteValue(e.target.value)}
                                        placeholder={t('amenitiesManager.notePlaceholder')}
                                        className="flex-1 text-xs h-8 rounded-lg border-slate-200 focus-visible:ring-rausch focus-visible:border-rausch"
                                        onKeyDown={(e) => e.key === 'Enter' && handleSaveNote(amenity.id)}
                                    />
                                    <Button
                                        size="icon"
                                        onClick={() => handleSaveNote(amenity.id)}
                                        disabled={updateInfoMutation.isPending}
                                        className="h-8 w-8 bg-rausch hover:bg-rausch/90 text-white rounded-lg shrink-0"
                                    >
                                        {updateInfoMutation.isPending
                                          ? <Loading03Icon className="h-3 w-3 animate-spin" />
                                          : <SentIcon className="h-3 w-3" />
                                        }
                                    </Button>
                                </div>
                            ) : (
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => handleStartEdit(amenity.id, selected.additionalInfo || '')}
                                    className="h-auto px-0 py-0 text-[10px] font-bold text-slate-400 hover:text-rausch hover:bg-transparent gap-1"
                                >
                                    <NoteIcon className="h-3 w-3" />
                                    {selected.additionalInfo ? (
                                        <span className="truncate max-w-[150px] italic">"{selected.additionalInfo}"</span>
                                    ) : (
                                        <span className="uppercase tracking-widest">{t('amenitiesManager.addNote')}</span>
                                    )}
                                </Button>
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
