import type { UseFormRegister, FieldErrors, UseFormWatch, UseFormSetValue } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { ChevronRight, AlertCircle } from 'lucide-react';

interface BasicInfoSectionProps {
  register: UseFormRegister<any>;
  watch: UseFormWatch<any>;
  setValue: UseFormSetValue<any>;
  errors: FieldErrors<any>;
  onContinue: () => void;
}

export function BasicInfoSection({
  register,
  watch,
  setValue,
  errors,
  onContinue,
}: BasicInfoSectionProps) {
  const { t } = useTranslation();

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('basic.title')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">{t('basic.subtitle')}</p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-6">
        {/* Title */}
        <div className="space-y-2">
          <Label htmlFor="title" className="text-sm font-semibold text-slate-900">{t('basic.propertyTitle')}</Label>
          <Input
            id="title"
            {...register('title')}
            placeholder={t('basic.propertyTitlePlaceholder')}
            className="h-12 px-4 py-3 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 transition-all placeholder-slate-400"
          />
          {errors.title && (
            <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
              <AlertCircle className="h-4 w-4" /> {errors.title.message as string}
            </div>
          )}
        </div>

        {/* Description */}
        <div className="space-y-2">
          <Label htmlFor="description" className="text-sm font-semibold text-slate-900">{t('basic.propertyDescription')}</Label>
          <Textarea
            id="description"
            {...register('description')}
            placeholder={t('basic.propertyDescriptionPlaceholder')}
            rows={5}
            className="rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 p-4 transition-all placeholder-slate-400"
          />
          {errors.description && (
            <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
              <AlertCircle className="h-4 w-4" /> {errors.description.message as string}
            </div>
          )}
        </div>

        {/* Capacity Counters */}
        <div className="pt-4 border-t border-slate-100 space-y-4">
          <h3 className="text-md font-bold text-slate-900">{t('basic.capacity')}</h3>
          <div className="grid gap-4 sm:grid-cols-2">
            {[
              { label: t('basic.guests'), field: 'guestCount' },
              { label: t('basic.bedrooms'), field: 'bedroomCount' },
              { label: t('basic.beds'), field: 'bedCount' },
              { label: t('basic.bathrooms'), field: 'bathroomCount' },
            ].map(({ label, field }) => (
              <div key={field} className="flex items-center justify-between py-2 px-1 border-b border-slate-50 last:border-0">
                <Label className="text-base font-semibold text-slate-900">{label}</Label>
                <div className="flex items-center gap-4">
                  <Button 
                    type="button" 
                    variant="outline" 
                    size="icon"
                    className="h-9 w-9 rounded-full border-slate-300 hover:border-slate-900 transition-colors"
                    onClick={() => setValue(field, Math.max(1, (watch(field) || 0) - 1))}
                  >
                    -
                  </Button>
                  <span className="w-4 text-center font-semibold text-slate-900">{watch(field) || 0}</span>
                  <Button 
                    type="button" 
                    variant="outline" 
                    size="icon"
                    className="h-9 w-9 rounded-full border-slate-300 hover:border-slate-900 transition-colors"
                    onClick={() => setValue(field, (watch(field) || 0) + 1)}
                  >
                    +
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>

        <Button
          type="button"
          onClick={onContinue}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm flex items-center justify-center gap-2 mt-4"
        >
          {t('basic.continue')} <ChevronRight className="ml-2 h-4 w-4" />
        </Button>
      </CardContent>
    </Card>
  );
}
