import type { UseFormRegister, FieldErrors, UseFormWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { ChevronRight, AlertCircle } from 'lucide-react';

interface PricingSectionProps {
  register: UseFormRegister<any>;
  watch: UseFormWatch<any>;
  errors: FieldErrors<any>;
  onContinue: () => void;
}

export function PricingSection({
  register,
  watch,
  errors,
  onContinue,
}: PricingSectionProps) {
  const { t } = useTranslation();
  const currencyCode = watch('currencyCode') || 'VND';
  const currencySymbol = currencyCode === 'VND' ? '₫' : '$';

  return (
    <Card className="rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow bg-white overflow-hidden">
      <CardHeader className="px-6 pt-6 pb-2 space-y-2">
        <h2 className="text-2xl font-semibold text-slate-900 tracking-tight">{t('pricing.title')}</h2>
        <p className="text-base font-normal text-slate-600 leading-relaxed">{t('pricing.subtitle')}</p>
      </CardHeader>
      <CardContent className="px-6 py-6 space-y-6">
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
          {/* Base Price */}
          <div className="space-y-2">
            <Label htmlFor="basePrice" className="text-sm font-semibold text-slate-900">
              {t('pricing.basePrice')} ({currencyCode})
            </Label>
            <div className="relative group">
              <span className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 font-semibold select-none">
                {currencySymbol}
              </span>
              <Input 
                id="basePrice"
                type="number"
                {...register('basePrice', { valueAsNumber: true })}
                className="h-12 pl-10 pr-4 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 placeholder-slate-400 font-medium"
                placeholder={currencySymbol === '₫' ? 'e.g. 500000' : 'e.g. 100'}
              />
            </div>
            {errors.basePrice && (
              <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
                <AlertCircle className="h-4 w-4" /> {errors.basePrice.message as string}
              </div>
            )}
          </div>

          {/* Cleaning Fee */}
          <div className="space-y-2">
            <Label htmlFor="cleaningFee" className="text-sm font-semibold text-slate-900">
              {t('pricing.cleaningFee')} ({currencyCode})
            </Label>
            <div className="relative group">
              <span className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 font-semibold select-none">
                {currencySymbol}
              </span>
              <Input 
                id="cleaningFee"
                type="number"
                {...register('cleaningFee', { valueAsNumber: true })}
                className="h-12 pl-10 pr-4 rounded-lg border border-slate-300 focus:border-slate-900 focus:ring-3 focus:ring-pink-100 placeholder-slate-400 font-medium"
                placeholder={currencySymbol === '₫' ? 'e.g. 50000' : 'e.g. 20'}
              />
            </div>
            {errors.cleaningFee && (
              <div className="flex items-center gap-1.5 text-sm font-medium text-red-600 mt-1">
                <AlertCircle className="h-4 w-4" /> {errors.cleaningFee.message as string}
              </div>
            )}
          </div>
        </div>

        <Button
          type="button"
          onClick={onContinue}
          className="w-full h-12 rounded-lg bg-[#FF5A5F] text-white font-semibold hover:bg-[#E04447] active:scale-95 transition-all shadow-sm flex items-center justify-center gap-2 mt-4"
        >
          {t('pricing.continue')} <ChevronRight className="ml-2 h-4 w-4" />
        </Button>
      </CardContent>
    </Card>
  );
}
