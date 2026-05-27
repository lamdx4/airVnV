import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { AlertCircle } from 'lucide-react';
import { Loading03Icon } from 'hugeicons-react';
import type { Amenity } from '@/features/properties/types';
import { SafeIcon } from '@/components/common/SafeIcon';

interface AmenitiesSectionProps {
  isLoadingAmenities: boolean;
  isAmenitiesError: boolean;
  availableAmenities: Amenity[] | undefined;
  selectedAmenities: string[];
  setSelectedAmenities: React.Dispatch<React.SetStateAction<string[]>>;
  onContinue: () => void;
}

export function AmenitiesSection({
  isLoadingAmenities,
  isAmenitiesError,
  availableAmenities,
  selectedAmenities,
  setSelectedAmenities,
  onContinue,
}: AmenitiesSectionProps) {
  const { t } = useTranslation();

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('amenities.title')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">{t('amenities.subtitle')}</p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-6">
        {isLoadingAmenities ? (
          <div className="flex flex-col items-center justify-center py-12 gap-3">
            <Loading03Icon className="h-8 w-8 animate-spin text-pink-500" />
            <span className="text-xs font-semibold text-slate-400 uppercase tracking-widest">Finding Amenities...</span>
          </div>
        ) : isAmenitiesError ? (
          <div className="p-4 rounded-xl border border-red-200 bg-red-50/50 flex items-start gap-3">
            <AlertCircle className="h-5 w-5 text-red-600 mt-0.5 shrink-0" />
            <div className="space-y-1">
              <h4 className="text-sm font-bold text-red-900">Amenities Service Offline</h4>
              <p className="text-xs text-red-700 leading-relaxed">
                {t('amenities.errorFetch')}
              </p>
            </div>
          </div>
        ) : (
          <div className="grid gap-3 grid-cols-1 sm:grid-cols-2">
            {availableAmenities?.map((amenity: Amenity) => {
              const isSelected = selectedAmenities.includes(amenity.id);
              return (
                <label key={amenity.id} className={`flex items-center gap-4 p-4 rounded-lg border transition-all cursor-pointer ${
                  isSelected ? 'border-pink-500 bg-pink-50 ring-1 ring-pink-100' : 'border-slate-200 hover:bg-slate-50'
                }`}>
                  <Checkbox
                    checked={isSelected}
                    onCheckedChange={() => {
                      setSelectedAmenities(prev => 
                        isSelected ? prev.filter(id => id !== amenity.id) : [...prev, amenity.id]
                      );
                    }}
                    className="h-5 w-5 rounded-md border-slate-300 data-[state=checked]:bg-[#FF5A5F] data-[state=checked]:border-[#FF5A5F] transition-colors"
                  />
                  <div className="flex items-center gap-3">
                      <SafeIcon icon={amenity.iconCode} className={`text-xl ${isSelected ? 'text-pink-600' : 'text-slate-400'}`} />
                      <span className="text-sm font-semibold text-slate-900">{amenity.name}</span>
                  </div>
                </label>
              )
            })}
          </div>
        )}

        <Button
          type="button"
          onClick={onContinue}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm flex items-center justify-center gap-2 mt-4"
        >
          {t('amenities.continue')}
        </Button>
      </CardContent>
    </Card>
  );
}
